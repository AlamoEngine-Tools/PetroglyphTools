// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using AnakinRaW.CommonUtilities.Extensions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Hashing;
using PG.Commons.Services;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Utilities;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal abstract class ConstructingMegArchiveBuilderBase(IServiceProvider services) : ServiceBase(services), IConstructingMegArchiveBuilder
{
    private const uint MaxEntryFileSize4G = uint.MaxValue;

    internal virtual uint MaxEntryFileSize => MaxEntryFileSize4G;

    // TODO: Test encryption cases
    public IConstructingMegArchive BuildConstructingMegArchive(IEnumerable<MegFileDataEntryBuilderInfo> builderEntries)
    {
        if (builderEntries == null) 
            throw new ArgumentNullException(nameof(builderEntries));
        
        var binaryInformation = GetBinaryInformation(builderEntries);

        var currentOffset = Convert.ToUInt32(binaryInformation.MetadataSize);

        var entries = new List<VirtualMegDataEntryReference>();

        foreach (var entry in Crc32Utilities.SortByCrc32(binaryInformation.Entries))
        {
            var dataEntryLocation = new MegDataEntryLocation(currentOffset, entry.Sizes.DataSize);
            var dataEntry = new MegDataEntry(entry.FilePath, entry.Crc32, dataEntryLocation, entry.Encrypted, entry.OriginalFilePath);
            
            entries.Add(new VirtualMegDataEntryReference(dataEntry, entry.Origin));
            
            currentOffset += entry.Sizes.BinarySize;
        }

        return new ConstructingMegArchive(entries, binaryInformation.MegFileVersion, binaryInformation.Encrypted);
    }

    protected abstract MegFileVersion FileVersion { get; }

    protected abstract int GetFileDescriptorSize(bool entryGetsEncrypted);

    protected abstract int GetHeaderSize();

    protected virtual int GetActualFileNameTableSize(int fileNameTableSize, bool megGetsEncrypted)
    {
        return fileNameTableSize;
    }

    protected virtual uint GetBinarySize(uint dataSize, bool entryGetsEncrypted)
    {
        return dataSize;
    }

    private MegFileBinaryInformation GetBinaryInformation(IEnumerable<MegFileDataEntryBuilderInfo> builderEntries)
    {
        var metadataSize = GetHeaderSize();

        var encryptMeg = false;
        var fileNameTableSize = 0;
        var fileTableSize = 0;


        var entryInfoList = new List<MegDataEntryBinaryInformation>();
        var checksumService = Services.GetRequiredService<ICrc32HashingService>();

        var megEncoding = MegFileConstants.MegDataEntryPathEncoding;

        foreach (var builderInfo in builderEntries)
        {
            if (builderInfo.Encrypted)
                encryptMeg = true;

            fileNameTableSize += MegFileNameTableRecord.GetRecordSize(builderInfo.FilePath);
            fileTableSize += GetFileDescriptorSize(builderInfo.Encrypted);

            var itemInfo = CreateBinaryInformation(builderInfo, megEncoding, checksumService);
            entryInfoList.Add(itemInfo);
        }


        metadataSize += GetActualFileNameTableSize(fileNameTableSize, encryptMeg);
        metadataSize += fileTableSize;

        return new MegFileBinaryInformation(metadataSize, FileVersion, encryptMeg, entryInfoList);
    }

    private MegDataEntryBinaryInformation CreateBinaryInformation(MegFileDataEntryBuilderInfo builderInfo, Encoding encoding, ICrc32HashingService crc32HashingService)
    {
        var originalFilePath = builderInfo.FilePath;

        var maxBytes = encoding.GetByteCountPG(originalFilePath.Length);
        var pathBytesBuffer = maxBytes > 256 ? new byte[maxBytes] : stackalloc byte[maxBytes];

        // Encoding the paths as ASCII has the potential of creating PG/Windows illegal file names due to the replacement character '?'. 
        // Extracting such files, without their name changed, will cause an exception.
        // However, we don't check such things here, as it's not the problem of this library, but for the calling code.
        var pathBytes = encoding.GetBytesReadOnly(originalFilePath.AsSpan(), pathBytesBuffer);

        var encodedFilePath = encoding.GetString(pathBytes);
        MegFilePathUtilities.ValidateFilePathCharacterLength(encodedFilePath);

        var crc = crc32HashingService.GetCrc32(pathBytes);

        var dataSizes = GetDataSize(builderInfo);

        return new MegDataEntryBinaryInformation(
            crc,
            encodedFilePath,
            dataSizes,
            builderInfo.Encrypted,
            originalFilePath,
            builderInfo.OriginInfo);
    }

    private MegDataEntrySize GetDataSize(MegFileDataEntryBuilderInfo builderInfo)
    {
        uint dataSize;
        if (builderInfo.Size.HasValue)
            dataSize = builderInfo.Size.Value;
        else
        {
            Debug.Assert(builderInfo.OriginInfo.IsLocalFile, "Expected OriginInfo to point to a local file!");
            
            var filePath = builderInfo.OriginInfo.FilePath;
            var fileSize = FileSystem.FileInfo.New(filePath!).Length;

            if (fileSize > MaxEntryFileSize)
                MegThrowHelper.ThrowDataEntryExceeds4GigabyteException(FileSystem.Path.GetFullPath(filePath!));

            dataSize = (uint) fileSize;
        }

        var binarySize = GetBinarySize(dataSize, builderInfo.Encrypted);

        if (binarySize < dataSize)
            throw new InvalidOperationException(
                $"Binary data size {binarySize} cannot be smaller than actual data size {dataSize}. Integer Overflow?");

        return new MegDataEntrySize(dataSize, binarySize);
    }
}