// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using JetBrains.Annotations;
using PG.Commons.Util;
using PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition;
using PG.StarWarsGame.Files.DAT.Commons.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]

namespace PG.StarWarsGame.Files.DAT.Binary.File.Builder
{
    internal abstract class AbstractDatFileBuilder
    {
        private int m_currentOffset;
        private const int HEADER_STARTING_OFFSET = 0;
        private const int INDEX_TABLE_STARTING_OFFSET = 4;

        protected AbstractDatFileBuilder()
        {
            m_currentOffset = INDEX_TABLE_STARTING_OFFSET;
        }

        [NotNull]
        protected DatFile FromBytesInternal([NotNull] byte[] bytes)
        {
            DatHeader header = BuildDatHeaderFromBytesInternal(bytes);
            IndexTable indexTable = BuildIndexTableFromBytesInternal(bytes, header.KeyCount);
            ValueTable valueTable = BuildValueTableFromBytesInternal(bytes, indexTable, header.KeyCount);
            KeyTable keyTable = BuildKeyTableFromBytesInternal(bytes, indexTable, header.KeyCount);
            return new DatFile(header, indexTable, valueTable, keyTable);
        }

        [NotNull]
        private DatHeader BuildDatHeaderFromBytesInternal([NotNull] byte[] bytes)
        {
            return new DatHeader(BitConverter.ToUInt32(bytes, HEADER_STARTING_OFFSET));
        }

        [NotNull]
        private IndexTable BuildIndexTableFromBytesInternal([NotNull] byte[] bytes, uint keyCount)
        {
            List<IndexTableRecord> indexTable = new();
            for (int i = 0; i < keyCount; i++)
            {
                uint crc32 = BitConverter.ToUInt32(bytes, m_currentOffset);
                m_currentOffset += sizeof(uint);
                uint valueLenght = BitConverter.ToUInt32(bytes, m_currentOffset);
                m_currentOffset += sizeof(uint);
                uint keyLength = BitConverter.ToUInt32(bytes, m_currentOffset);
                m_currentOffset += sizeof(uint);
                IndexTableRecord indexTableRecord = new(crc32, keyLength, valueLenght);
                indexTable.Add(indexTableRecord);
            }

            return new IndexTable(indexTable);
        }

        [NotNull]
        private ValueTable BuildValueTableFromBytesInternal([NotNull] byte[] bytes, [NotNull] IndexTable indexTable,
            uint keyCount)
        {
            List<ValueTableRecord> valueTableRecords = new();

            for (int i = 0; i < keyCount; i++)
            {
                long valueLength = indexTable.IndexTableRecords[i]?.ValueLength ??
                                   throw new IndexAndValueTableOutOfSyncException(
                                       $"Building the DAT file failed at offset {m_currentOffset} due to an invalid IndexTableRecord at position {i}");
                ValueTableRecord valueTableRecord = new(bytes, m_currentOffset, valueLength);
                m_currentOffset += valueTableRecord.Size;
                valueTableRecords.Add(valueTableRecord);
            }

            return new ValueTable(valueTableRecords);
        }

        [NotNull]
        private KeyTable BuildKeyTableFromBytesInternal([NotNull] byte[] bytes, [NotNull] IndexTable indexTable,
            uint keyCount)
        {
            List<KeyTableRecord> keyTableRecords = new();

            for (int i = 0; i < keyCount; i++)
            {
                long keyLength = indexTable.IndexTableRecords[i]?.KeyLength ??
                                 throw new IndexAndValueTableOutOfSyncException(
                                     $"Building the DAT file failed at offset {m_currentOffset} due to an invalid IndexTableRecord at position {i}");
                KeyTableRecord keyTableRecord = new(bytes, m_currentOffset, keyLength);
                m_currentOffset += keyTableRecord.Size;
                keyTableRecords.Add(keyTableRecord);
            }

            return new KeyTable(keyTableRecords);
        }

        [NotNull]
        protected DatFile FromHolderInternal([NotNull] List<Tuple<string, string>> content)
        {
            DatHeader datHeader = new(Convert.ToUInt32(content.Count));
            List<IndexTableRecord> indexTableRecords = new();
            List<ValueTableRecord> valueTableRecords = new();
            List<KeyTableRecord> keyTableRecords = new();

            foreach (Tuple<string, string> keyValuePair in content)
            {
                if (null == keyValuePair)
                {
                    continue;
                }

                string key = keyValuePair.Item1?.Replace("\0", string.Empty) ?? string.Empty;
                string value = keyValuePair.Item2;

                if (StringUtility.IsNullEmptyOrWhiteSpace(key))
                {
                    throw new DatFileContentInvalidException(
                        $"A key may never be null. Provided key-value-pair {keyValuePair.Item1}:{keyValuePair.Item2}");
                }

                if (null == value)
                {
                    value = "";
                }

                ValueTableRecord valueTableRecord = new(value);
                KeyTableRecord keyTableRecord = new(key);
                IndexTableRecord indexTableRecord = new(ChecksumUtility.GetChecksum(key),
                    Convert.ToUInt32(key.Length), Convert.ToUInt32(value.Length));

                valueTableRecords.Add(valueTableRecord);
                keyTableRecords.Add(keyTableRecord);
                indexTableRecords.Add(indexTableRecord);
            }

            IndexTable indexTable = new(indexTableRecords);
            ValueTable valueTable = new(valueTableRecords);
            KeyTable keyTable = new(keyTableRecords);
            return new DatFile(datHeader, indexTable, valueTable, keyTable);
        }
    }
}
