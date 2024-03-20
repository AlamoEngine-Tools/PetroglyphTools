// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Binary;
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

        var header = new DatHeader((uint)model.Count);
        var indexRecords = new List<IndexTableRecord>();
        var values = new List<ValueTableRecord>();
        var keys = new List<KeyTableRecord>();
       
        Crc32? currentCrc = null;
        var isSorted = true;
        
        for (var i = 0; i < model.Count; i++)
        {
            var value = (string.IsNullOrWhiteSpace(model[i].Value) ? "" : model[i].Value) ?? "";
            var key = model[i].Key.Replace("\0", string.Empty);

            var keyChecksum = checksumService.GetCrc32(key, DatFileConstants.TextKeyEncoding);

            var valueRecord = new ValueTableRecord(value);
           
            var keyRecord = new KeyTableRecord(key, keyChecksum);
            
            var indexRecord = new IndexTableRecord(keyChecksum, (uint)keyRecord.Key.Length, (uint)valueRecord.Value.Length);
            
            if (i == 0)
            {
                currentCrc = indexRecord.Crc32;
            }
            else if (currentCrc < indexRecord.Crc32)
            {
                currentCrc = indexRecord.Crc32;
            }
            else
            {
                isSorted = false;
            }

            values.Add(valueRecord);
            keys.Add(keyRecord);
            indexRecords.Add(indexRecord);
        }

        if (isSorted && model.Order != DatFileType.OrderedByCrc32)
        {
            throw new BinaryCorruptedException(
                $"The provided holder appears to be sorted, but claims to be {model.Order}.");
        }

        if (!isSorted && model.Order != DatFileType.NotOrdered)
        {
            throw new BinaryCorruptedException(
                $"The provided holder appears to be unsorted, but claims to be {model.Order}.");
        }

        return new DatBinaryFile(header, new IndexTable(indexRecords), new ValueTable(values), new KeyTable(keys));
    }

    public IDatModel BinaryToModel(DatBinaryFile binary)
    {
        if (binary == null) 
            throw new ArgumentNullException(nameof(binary));

        var isSorted = true;
        var currentCrc = default(Crc32);

        for (var i = 0; i < binary.RecordNumber; i++)
        {
            if (binary.IndexTable[i].Crc32 > currentCrc)
            {
                currentCrc = binary.IndexTable[i].Crc32;
            }
            else
            {
                isSorted = false;
                break;
            }
        }

        var datFileContent = new List<DatFileEntry>();

        for (var i = 0; i < binary.RecordNumber; i++)
        {
            var keyEntry = binary.KeyTable[i];
            var valueEntry = binary.ValueTable[i].Value;
            datFileContent.Add(new DatFileEntry(keyEntry.Key, keyEntry.Crc32, valueEntry));
        }

        return new DatModel(datFileContent, isSorted ? DatFileType.OrderedByCrc32 : DatFileType.NotOrdered);
    }
}