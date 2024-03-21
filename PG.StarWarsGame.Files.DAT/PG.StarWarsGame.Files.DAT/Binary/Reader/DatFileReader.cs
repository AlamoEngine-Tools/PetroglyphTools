// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Hashing;
using PG.Commons.Services;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Binary;

internal class DatFileReader : ServiceBase, IDatFileReader
{
    internal DatFileReader(IServiceProvider services) : base(services)
    {
    }

    public DatBinaryFile ReadBinary(Stream byteStream)
    {
        using var reader = new BinaryReader(byteStream);
        var header = new DatHeader(reader.ReadUInt32());

        var indexTableRecords = new List<IndexTableRecord>();
        for (var i = 0; i < header.RecordCount; i++)
        {
            var rawCrc32 = reader.ReadUInt32();
            var valueLength = reader.ReadUInt32();
            var keyLength = reader.ReadUInt32();
            indexTableRecords.Add(new IndexTableRecord(new Crc32(rawCrc32), keyLength, valueLength));
        }

        var indexTable = new IndexTable(indexTableRecords);
        var valueRecords = new List<ValueTableRecord>();
        for (var i = 0; i < header.RecordCount; i++)
        {
            var value = reader.ReadString((int)indexTable[i].ValueLength, DatFileConstants.TextValueEncoding);
            valueRecords.Add(new ValueTableRecord(value));
        }

        var valueTable = new ValueTable(valueRecords);
        var keyRecords = new List<KeyTableRecord>();
        var checksumService = Services.GetRequiredService<ICrc32HashingService>();
        for (var i = 0; i < header.RecordCount; i++)
        {
            var key = reader.ReadString((int)indexTable[i].KeyLength, DatFileConstants.TextKeyEncoding);
            var crc = checksumService.GetCrc32(key, DatFileConstants.TextKeyEncoding);
            keyRecords.Add(new KeyTableRecord(key, crc));
        }

        var keyTable = new KeyTable(keyRecords);

        return new DatBinaryFile(header, indexTable, valueTable, keyTable);
    }

    public DatFileType PeekFileType(Stream byteStream)
    {
        using var reader = new BinaryReader(byteStream);

        var numFiles = reader.ReadUInt32();
        
        var lastCrc = default(Crc32);

        for (var i = 0; i < numFiles; i++)
        {
            var currentCrc = new Crc32(reader.ReadUInt32());

            if (currentCrc < lastCrc)
                return DatFileType.NotOrdered;

            reader.ReadUInt32(); // Value Length
            reader.ReadUInt32(); // Key Length

            lastCrc = currentCrc;
        }

        return DatFileType.OrderedByCrc32;
    }
}