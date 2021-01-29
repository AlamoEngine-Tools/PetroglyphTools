using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using PG.Commons.Util;
using PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition;
using PG.StarWarsGame.Files.DAT.Commons.Exceptions;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]
namespace PG.StarWarsGame.Files.DAT.Binary.File.Builder
{
    internal abstract class ADatFileBuilder
    {
        private int m_currentOffset;
        private const int HEADER_STARTING_OFFSET = 0;
        private const int INDEX_TABLE_STARTING_OFFSET = 4;

        protected ADatFileBuilder()
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
            List<IndexTableRecord> indexTable = new List<IndexTableRecord>();
            for (int i = 0; i < keyCount; i++)
            {
                uint crc32 = BitConverter.ToUInt32(bytes, m_currentOffset);
                m_currentOffset += sizeof(uint);
                uint valueLenght = BitConverter.ToUInt32(bytes, m_currentOffset);
                m_currentOffset += sizeof(uint);
                uint keyLength = BitConverter.ToUInt32(bytes, m_currentOffset);
                m_currentOffset += sizeof(uint);
                IndexTableRecord indexTableRecord = new IndexTableRecord(crc32, keyLength, valueLenght);
                indexTable.Add(indexTableRecord);
            }

            return new IndexTable(indexTable);
        }

        [NotNull]
        private ValueTable BuildValueTableFromBytesInternal([NotNull] byte[] bytes, [NotNull] IndexTable indexTable,
            uint keyCount)
        {
            List<ValueTableRecord> valueTableRecords = new List<ValueTableRecord>();

            for (int i = 0; i < keyCount; i++)
            {
                long valueLength = indexTable.IndexTableRecords[i]?.ValueLength ?? throw new IndexAndValueTableOutOfSyncException(
                    $"Building the DAT file failed at offset {m_currentOffset} due to an invalid IndexTableRecord at position {i}");
                ValueTableRecord valueTableRecord = new ValueTableRecord(bytes, m_currentOffset, valueLength);
                m_currentOffset += valueTableRecord.Size;
                valueTableRecords.Add(valueTableRecord);
            }

            return new ValueTable(valueTableRecords);
        }

        [NotNull]
        private KeyTable BuildKeyTableFromBytesInternal([NotNull] byte[] bytes, [NotNull] IndexTable indexTable,
            uint keyCount)
        {
            List<KeyTableRecord> keyTableRecords = new List<KeyTableRecord>();

            for (int i = 0; i < keyCount; i++)
            {
                long keyLength = indexTable.IndexTableRecords[i]?.KeyLength ??
                                 throw new IndexAndValueTableOutOfSyncException(
                                     $"Building the DAT file failed at offset {m_currentOffset} due to an invalid IndexTableRecord at position {i}");
                KeyTableRecord keyTableRecord = new KeyTableRecord(bytes, m_currentOffset, keyLength);
                m_currentOffset += keyTableRecord.Size;
                keyTableRecords.Add(keyTableRecord);
            }

            return new KeyTable(keyTableRecords);
        }

        [NotNull]
        protected DatFile FromHolderInternal([NotNull] List<Tuple<string, string>> content)
        {
            DatHeader datHeader = new DatHeader(Convert.ToUInt32(content.Count));
            List<IndexTableRecord> indexTableRecords = new List<IndexTableRecord>();
            List<ValueTableRecord> valueTableRecords = new List<ValueTableRecord>();
            List<KeyTableRecord> keyTableRecords = new List<KeyTableRecord>();

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

                ValueTableRecord valueTableRecord = new ValueTableRecord(value);
                KeyTableRecord keyTableRecord = new KeyTableRecord(key);
                IndexTableRecord indexTableRecord = new IndexTableRecord(ChecksumUtility.GetChecksum(key),
                    Convert.ToUInt32(key.Length), Convert.ToUInt32(value.Length));

                valueTableRecords.Add(valueTableRecord);
                keyTableRecords.Add(keyTableRecord);
                indexTableRecords.Add(indexTableRecord);
            }

            IndexTable indexTable = new IndexTable(indexTableRecords);
            ValueTable valueTable = new ValueTable(valueTableRecords);
            KeyTable keyTable = new KeyTable(keyTableRecords);
            return new DatFile(datHeader, indexTable, valueTable, keyTable);
        }
    }
}
