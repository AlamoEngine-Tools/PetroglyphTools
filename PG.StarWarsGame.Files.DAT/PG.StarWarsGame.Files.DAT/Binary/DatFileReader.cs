// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Hashing;
using PG.Commons.Services;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Binary;

internal class DatFileReader : ServiceBase, IDatFileReader
{
    internal DatFileReader(IServiceProvider services) : base(services)
    {
    }

    public IDatFileMetadata ReadBinary(Stream byteStream)
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
            var valueBytes = reader.ReadBytes((int)indexTable[i].ValueLength);
            var value = DatFileConstants.TextValueEncoding.GetString(valueBytes);
            valueRecords.Add(new ValueTableRecord(value));
        }

        var valueTable = new ValueTable(valueRecords);
        var keyRecords = new List<KeyTableRecord>();
        var checksumService = Services.GetRequiredService<ICrc32HashingService>();
        for (var i = 0; i < header.RecordCount; i++)
        {
            var keyBytes = reader.ReadBytes((int)indexTable[i].KeyLength);
            var key = DatFileConstants.TextKeyEncoding.GetString(keyBytes);
            var crc = checksumService.GetCrc32(key, DatFileConstants.TextKeyEncoding);
            keyRecords.Add(new KeyTableRecord(key, crc));
        }

        var keyTable = new KeyTable(keyRecords);

        return new DatFile(header, indexTable, valueTable, keyTable);
    }

    public DatFileType PeekFileType(Stream byteStream)
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

        var fileType = DatFileType.OrderedByCrc32;
        Crc32? currentCrc = null;
        foreach (var record in indexTableRecords)
        {
            if (currentCrc is null)
            {
                currentCrc = record.Crc32;
                continue;
            }

            if (currentCrc < record.Crc32)
            {
                currentCrc = record.Crc32;
            }
            else
            {
                fileType = DatFileType.NotOrdered;
                break;
            }
        }

        return fileType;
    }
}