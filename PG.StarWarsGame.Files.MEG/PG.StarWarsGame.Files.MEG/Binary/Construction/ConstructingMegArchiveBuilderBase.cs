// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace PG.StarWarsGame.Files.MEG.Binary;

internal abstract class ConstructingMegArchiveBuilderBase(IServiceProvider services) : ServiceBase(services), IConstructingMegArchiveBuilder
{
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
            var dataEntry = new MegDataEntry(entry.FilePath, entry.Crc32, dataEntryLocation, entry.Encrypted);
            
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
        var checksumService = Services.GetRequiredService<IChecksumService>();

        foreach (var builderInfo in builderEntries)
        {
            if (builderInfo.Encrypted)
                encryptMeg = true;

            fileNameTableSize += MegFileNameTableRecord.GetRecordSize(builderInfo.FilePath);
            fileTableSize += GetFileDescriptorSize(builderInfo.Encrypted);

            // We do *not* re-encode builderInfo.FilePath here to ASCII, so we can store the original value
            // in MegFileNameTableRecord. MegFileNameTableRecord performs the final encoding and specification checks.
            var crc = checksumService.GetChecksum(builderInfo.FilePath, MegFileConstants.MegContentFileNameEncoding);
            var dataSizes = GetDataSize(builderInfo);

            var itemInfo = new MegDataEntryBinaryInformation(
                crc,
                builderInfo.FilePath,
                dataSizes,
                builderInfo.Encrypted,
                builderInfo.OriginInfo);

            entryInfoList.Add(itemInfo);
        }


        metadataSize += GetActualFileNameTableSize(fileNameTableSize, encryptMeg);
        metadataSize += fileTableSize;

        return new MegFileBinaryInformation(metadataSize, FileVersion, encryptMeg, entryInfoList);
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

            if (fileSize > uint.MaxValue)
                ThrowHelper.ThrowDataEntryExceeds4GigabyteException(FileSystem.Path.GetFullPath(filePath!));

            dataSize = (uint) fileSize;
        }

        var binarySize = GetBinarySize(dataSize, builderInfo.Encrypted);

        if (binarySize < dataSize)
            throw new InvalidOperationException(
                $"Binary data size {binarySize} cannot be smaller than actual data size {dataSize}. Integer Overflow?");

        return new MegDataEntrySize(dataSize, binarySize);
    }
}