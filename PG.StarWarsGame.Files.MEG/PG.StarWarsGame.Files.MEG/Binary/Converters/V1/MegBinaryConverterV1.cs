// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Data.Archives;

namespace PG.StarWarsGame.Files.MEG.Binary.V1;

internal sealed class MegBinaryConverterV1(IServiceProvider services) : MegBinaryConverterBase<MegMetadata>(services)
{
    protected override MegMetadata ModelToBinaryCore(IMegArchive model)
    {
        var fileCount = (uint)model.Count;

        var header = new MegHeader(fileCount, fileCount);

        var fileNameTableEntries = new List<MegFileNameTableRecord>(model.Count);
        var fileTableEntries = new List<MegFileTableRecord>(model.Count);

        for (var i = 0; i < model.Count; i++)
        {
            var dataEntry = model[i];
            fileNameTableEntries.Add(new MegFileNameTableRecord(dataEntry.FilePath, dataEntry.OriginalFilePath));

            var binaryRecord = new MegFileTableRecord(
                dataEntry.Crc32, 
                (uint)i, 
                dataEntry.Location.Size,
                dataEntry.Location.Offset, 
                (uint)i);

            fileTableEntries.Add(binaryRecord);
        }

        var fileNameTable = new BinaryTable<MegFileNameTableRecord>(fileNameTableEntries);
        var fileTable = new MegFileTable(fileTableEntries);

        return new MegMetadata(header, fileNameTable, fileTable);
    }
}