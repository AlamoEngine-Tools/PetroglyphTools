// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.Commons.Files;
using PG.Commons.Services;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using PG.StarWarsGame.Files.DAT.Common.Exceptions;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Binary;

internal class DatFileConverter : ServiceBase, IDatFileConverter
{
    public DatFileConverter(IServiceProvider services) : base(services)
    {
    }

    public IDatFileMetadata FromHolder(IDatFile holder)
    {
        var header = new DatHeader((uint)holder.Content.Count);
        var indexRecords = new List<IndexTableRecord>();
        var values = new List<ValueTableRecord>();
        var keys = new List<KeyTableRecord>();
        Crc32? currentCrc = null;
        var isSorted = true;
        for (var i = 0; i < holder.Content.Count; i++)
        {
            string value = (string.IsNullOrWhiteSpace(holder.Content[i].Value) ? "" : holder.Content[i].Value) ?? "";
            string key = holder.Content[i].Key.Replace("\0", string.Empty);

            var valueRecord = new ValueTableRecord(value);
            var keyRecord = new KeyTableRecord(key);
            var indexRecord =
                new IndexTableRecord(
                    ChecksumService.Instance.GetChecksum(keyRecord.Key, DatFileConstants.TextKeyEncoding),
                    (uint)keyRecord.Key.Length, (uint)valueRecord.Value.Length);
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

        if (isSorted && (holder.Order != DatFileType.OrderedByCrc32))
        {
            throw new CorruptDatFileException(
                $"The provided holder appears to be sorted, but claims to be {holder.Order}.");
        }

        if (!isSorted && (holder.Order != DatFileType.NotOrdered))
        {
            throw new CorruptDatFileException(
                $"The provided holder appears to be unsorted, but claims to be {holder.Order}.");
        }

        return new DatFile(header, new IndexTable(indexRecords), new ValueTable(values), new KeyTable(keys));
    }

    public IDatFile ToHolder(IFileHolderParam param, IDatFileMetadata model)
    {
        if (param is not DatFileHolderParam)
        {
            throw new ArgumentException(
                $"The provided parameter {param} is incompatible with the desired holder type {nameof(IDatFile)}.",
                nameof(param));
        }

        var param0 = (DatFileHolderParam)param;

        var isSorted = true;
        Crc32 currentCrc = model.IndexTable[0].Crc32;
        for (var i = 1; i < model.RecordNumber; i++)
        {
            if (model.IndexTable[i].Crc32 > currentCrc)
            {
                currentCrc = model.IndexTable[i].Crc32;
            }
            else
            {
                isSorted = false;
                break;
            }
        }

        var datFileContent = new List<DatFileEntry>();

        for (var i = 0; i < model.RecordNumber; i++)
        {
            datFileContent.Add(new DatFileEntry(model.KeyTable[i], model.ValueTable[i]));
        }

        param0.Order ??= isSorted ? DatFileType.OrderedByCrc32 : DatFileType.NotOrdered;

        return new DatFileHolder(datFileContent.AsReadOnly(), param0, Services);
    }
}