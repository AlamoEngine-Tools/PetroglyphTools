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
using PG.StarWarsGame.Files.MEG.Binary.Size;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Utilities;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal abstract class ConstructingMegArchiveBuilderBase(IServiceProvider services) 
    : ServiceBase(services), IConstructingMegArchiveBuilder
{
    internal virtual uint MaxEntryFileSize { get; } = MaxMegSizeProvider.GetMegMaxSize(MaxMegSizeMode.Binary).MaxEntrySize;
    internal virtual uint MaxFileSize { get; } = MaxMegSizeProvider.GetMegMaxSize(MaxMegSizeMode.Binary).MaxFileSize;

    protected abstract MegFileVersion FileVersion { get; }

    // TODO: Test encryption cases
    public IConstructingMegArchive BuildConstructingMegArchive(IEnumerable<MegDataEntryBuilderInfo> builderEntries)
    {
        if (builderEntries == null) 
            throw new ArgumentNullException(nameof(builderEntries));

        var calculator = Services.GetRequiredService<IMegBinaryServiceFactory>().GetMegSizeCalculator(FileVersion);

        var binaryInformation = GetBinaryInformation(builderEntries, calculator);

        long currentOffset = binaryInformation.MetadataSize;

        var entries = new List<VirtualMegDataEntryReference>();

        foreach (var entry in Crc32Utilities.SortByCrc32(binaryInformation.Entries))
        {
            var dataEntryLocation = new MegDataEntryLocation((uint)currentOffset, entry.Sizes.DataSize);
            var dataEntry = new MegDataEntry(entry.Path, entry.Crc32, dataEntryLocation, entry.Encrypted, entry.OriginalPath);
            
            entries.Add(new VirtualMegDataEntryReference(dataEntry, entry.Origin));
            currentOffset += entry.Sizes.BinarySize;
        }
        
        Debug.Assert(calculator.CurrentSize <= uint.MaxValue);
        Debug.Assert((long)calculator.CurrentSize == currentOffset);

        return new ConstructingMegArchive(entries, binaryInformation.MegFileVersion, (uint)calculator.CurrentSize, binaryInformation.Encrypted);
    }

    private MegFileBinaryInformation GetBinaryInformation(
        IEnumerable<MegDataEntryBuilderInfo> entries,
        IMegSizeCalculator calculator)
    {
        var encryptMeg = false;
        
        var entryInfoList = new List<MegDataEntryBinaryInformation>();
        var checksumService = Services.GetRequiredService<ICrc32HashingService>();
        var megEncoding = MegFileConstants.MegDataEntryPathEncoding;

        foreach (var entry in entries)
        {
            entryInfoList.Add(CreateEntryBinaryInformation(entry, megEncoding, checksumService));
            calculator.AddEntry(entry);
            if (calculator.CurrentSize > MaxFileSize)
                throw new MegSizeException("The to be constructed MEG file is too large.");

            if (entry.Encrypted)
                encryptMeg = true;
        }

        Debug.Assert(calculator.MetadataSize <= calculator.CurrentSize);

        return new MegFileBinaryInformation((uint)calculator.MetadataSize, FileVersion, encryptMeg, entryInfoList);
    }

    private MegDataEntryBinaryInformation CreateEntryBinaryInformation(
        MegDataEntryBuilderInfo builderInfo, 
        Encoding encoding, 
        ICrc32HashingService crc32HashingService)
    {
        var originalEntryPath = builderInfo.EntryPath;

        var maxBytes = encoding.GetByteCountPG(originalEntryPath.Length);
        var pathBytesBuffer = maxBytes > 256 ? new byte[maxBytes] : stackalloc byte[maxBytes];

        // Encoding the paths as ASCII has the potential of creating PG/Windows illegal file names due to the replacement character '?'. 
        // Extracting such files, without their name changed, will cause an exception.
        // However, we don't check such things here, as it's not the problem of this library, but for the calling code.
        var pathBytes = encoding.GetBytesReadOnly(originalEntryPath.AsSpan(), pathBytesBuffer);

        var encodedEntryPath = encoding.GetString(pathBytes);
        
        MegPathUtilities.ValidateEntryFileNameLength(encodedEntryPath);

        var crc = crc32HashingService.GetCrc32(pathBytes);

        var dataSizes = GetDataSize(builderInfo);

        return new MegDataEntryBinaryInformation(
            crc,
            encodedEntryPath,
            dataSizes,
            builderInfo.Encrypted,
            originalEntryPath,
            builderInfo.OriginInfo);
    }

    private MegDataEntrySize GetDataSize(MegDataEntryBuilderInfo dataEntryInfo)
    {
        // At this point we do not trust the size of the data entry anymore,
        // as it could have been changed since the builder info was created, so we need to refresh it.
        dataEntryInfo.RefreshSize();
        var binarySize = MegSizeCalculator.GetBinaryEntrySizeWithEncryption(dataEntryInfo);
        return binarySize > MaxEntryFileSize
            ? throw new MegEntrySizeException("Entry file size is larger than the allowed.") 
            : new MegDataEntrySize(dataEntryInfo.Size, (uint)binarySize);
    }
}