// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AnakinRaW.CommonUtilities.Extensions;
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

            var valueEncoding = DatFileConstants.TextValueEncoding;

            for (var i = 0; i < header.RecordCount; i++)
            {
                var bytes = valueEncoding.GetByteCountPG((int)indexTable[i].ValueLength);
                var value = reader.ReadString(bytes, valueEncoding);
                valueRecords.Add(new ValueTableRecord(value));
            }

            var valueTable = new BinaryTable<ValueTableRecord>(valueRecords);
            var keyRecords = new List<KeyTableRecord>();

            // NB: We use Latin1 encoding here, so that we can stay compatible any binary compatible .DAT file
            var extendedKeyEncoding = DatFileConstants.TextKeyEncoding_Latin1;
            var normalKeyEncoding = DatFileConstants.TextKeyEncoding;

            for (var i = 0; i < header.RecordCount; i++)
            {
                var byteSize = normalKeyEncoding.GetByteCountPG((int)indexTable[i].KeyLength);

                var originalKey = reader.ReadString(byteSize, extendedKeyEncoding);
                var key = normalKeyEncoding.EncodeString(originalKey);

                keyRecords.Add(new KeyTableRecord(key, originalKey));
            }

            var keyTable = new BinaryTable<KeyTableRecord>(keyRecords);

            return new DatBinaryFile(header, indexTable, valueTable, keyTable);
        }
        catch (EndOfStreamException)
        {
            throw new BinaryCorruptedException("Unable to read .DAT binary.");
        }
    }

    public DatFileType PeekFileType(Stream byteStream)
    {
        if (byteStream == null)
            throw new ArgumentNullException(nameof(byteStream));

        using var reader = new BinaryReader(byteStream, Encoding.Default, true);

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