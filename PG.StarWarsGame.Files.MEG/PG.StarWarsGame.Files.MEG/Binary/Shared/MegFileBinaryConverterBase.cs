// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Utilities;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal sealed class MegFileBinaryConverterV1 : MegFileBinaryConverterBase<MegMetadata>
{
    public MegFileBinaryConverterV1(IServiceProvider services) : base(services)
    {
    }
}

internal abstract class MegFileBinaryConverterBase<TMegMetadata> : ServiceBase, IMegFileBinaryConverter where TMegMetadata : IMegFileMetadata
{
    protected MegFileBinaryConverterBase(IServiceProvider services) : base(services)
    {
    }

    public IMegFileMetadata FromHolder(IMegFile holder)
    {
        using var param = holder.CreateParam();
        var entries = holder.Content.Select(m => new MegFileDataEntryInfo(holder, m)).ToList();
        return FromModel(entries, param);
    }

    /// <inheritdoc/>
    public IMegFileMetadata FromModel(IReadOnlyList<MegFileDataEntryInfo> dataEntries, MegFileHolderParam fileParam)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public IMegFile ToHolder(MegFileHolderParam param, IMegFileMetadata model)
    {
        var files = new List<MegFileDataEntry>(model.Header.FileNumber);

        // According to the specification: 
        //  - The Meg's FileTable is sorted by CRC32.
        //  - It's not specified how or whether the FileNameTable is sorted.
        //  - The game merges FileTable entries (and takes the last entry for duplicates)
        //  --> In theory:  For file entries with the same file name (and thus same CRC32),
        //                  the game should use the last file.
        // 
        // Since an IMegFile expects a List<>, not a Collection<>, we have to preserve the order of the FileTable
        for (var i = 0; i < model.Header.FileNumber; i++)
        {
            var fileDescriptor = model.FileTable[i];
            var crc = fileDescriptor.Crc32;
            var fileOffset = fileDescriptor.FileOffset;
            var fileSize = fileDescriptor.FileSize;
            var fileNameIndex = fileDescriptor.FileNameIndex;
            var fileName = model.FileNameTable[fileNameIndex];
            files.Add(new MegFileDataEntry(crc, fileName, fileOffset, fileSize));
        }

        return new MegFileHolder(new MegArchive(files), param, Services);
    }
}