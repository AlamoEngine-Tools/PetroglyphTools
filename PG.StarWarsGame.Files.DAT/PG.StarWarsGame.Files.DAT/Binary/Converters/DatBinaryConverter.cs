// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Hashing;
using PG.Commons.Services;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Binary;

internal class DatBinaryConverter(IServiceProvider services) : ServiceBase(services), IDatBinaryConverter
{
    public DatBinaryFile ModelToBinary(IDatModel model)
    {
        var checksumService = Services.GetRequiredService<ICrc32HashingService>();

        var numEntries = model.Count;
        var header = new DatHeader((uint)numEntries);
        var indexRecords = new List<IndexTableRecord>(numEntries);
        var values = new List<ValueTableRecord>(numEntries);
        var keys = new List<KeyTableRecord>(numEntries);
       
        var isSorted = true;

        var lastCrc = default(Crc32);
        foreach (var entry in model)
        {
            var value = entry.Value;
            var key = entry.Key;

            var keyChecksum = checksumService.GetCrc32(key, DatFileConstants.TextKeyEncoding);

            var valueRecord = new ValueTableRecord(value);
            var keyRecord = new KeyTableRecord(key, keyChecksum);
            var indexRecord = new IndexTableRecord(keyChecksum, (uint)keyRecord.Key.Length, (uint)valueRecord.Value.Length);
            
            values.Add(valueRecord);
            keys.Add(keyRecord);
            indexRecords.Add(indexRecord);

            if (keyChecksum < lastCrc)
                isSorted = false;

            lastCrc = keyChecksum;
        }
        
        if (model.KeySortOder == DatFileType.OrderedByCrc32 && !isSorted)
            throw new ArgumentException("MasterTextModel must be sorted.", nameof(model));

        return new DatBinaryFile(header, new IndexTable(indexRecords), new ValueTable(values), new KeyTable(keys));
    }

    public IDatModel BinaryToModel(DatBinaryFile binary)
    {
        if (binary == null) 
            throw new ArgumentNullException(nameof(binary));

        var datFileContent = new List<DatStringEntry>(binary.RecordNumber);

        for (var i = 0; i < binary.RecordNumber; i++)
        {
            var keyEntry = binary.KeyTable[i];
            var valueEntry = binary.ValueTable[i].Value;
            datFileContent.Add(new DatStringEntry(keyEntry.Key, keyEntry.Crc32, valueEntry));
        }

        return new DatModel(datFileContent);
    }
}