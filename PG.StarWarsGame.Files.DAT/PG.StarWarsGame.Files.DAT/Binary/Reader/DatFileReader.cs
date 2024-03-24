// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using PG.Commons.Binary;
using PG.Commons.Hashing;
using PG.Commons.Services;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Binary;

internal class DatFileReader(IServiceProvider services) : ServiceBase(services), IDatFileReader
{
    public DatBinaryFile ReadBinary(Stream byteStream)
    {
        if (byteStream == null) 
            throw new ArgumentNullException(nameof(byteStream));

        try
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

            var indexTable = new BinaryTable<IndexTableRecord>(indexTableRecords);
            var valueRecords = new List<ValueTableRecord>();
            for (var i = 0; i < header.RecordCount; i++)
            {
                var bytes = DatFileConstants.TextValueEncoding.GetByteCountPG((int)indexTable[i].ValueLength);
                var value = reader.ReadString(bytes, DatFileConstants.TextValueEncoding);
                valueRecords.Add(new ValueTableRecord(value));
            }

            var valueTable = new BinaryTable<ValueTableRecord>(valueRecords);
            var keyRecords = new List<KeyTableRecord>();
            for (var i = 0; i < header.RecordCount; i++)
            {
                var key = reader.ReadString((int)indexTable[i].KeyLength, DatFileConstants.TextKeyEncoding);
                keyRecords.Add(new KeyTableRecord(key));
            }

            var keyTable = new BinaryTable<KeyTableRecord>(keyRecords);

            return new DatBinaryFile(header, indexTable, valueTable, keyTable);
        }
        catch (EndOfStreamException)
        {
            throw new BinaryCorruptedException("Unable to read .DAT binary");
        }
    }

    public DatFileType PeekFileType(Stream byteStream)
    {
        if (byteStream == null)
            throw new ArgumentNullException(nameof(byteStream));

        using var reader = new BinaryReader(byteStream);

        try
        {
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
        catch (EndOfStreamException)
        {
            throw new BinaryCorruptedException("Unable to read stream as .DAT file");
        }
    }
}