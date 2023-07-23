// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PG.Commons.Services;
using PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition;
using PG.StarWarsGame.Files.DAT.Commons.Exceptions;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]

namespace PG.StarWarsGame.Files.DAT.Binary.File.Builder;

internal abstract class AbstractDatFileBuilder
{
    private int m_currentOffset;
    private const int HEADER_STARTING_OFFSET = 0;
    private const int INDEX_TABLE_STARTING_OFFSET = 4;

    protected AbstractDatFileBuilder()
    {
        m_currentOffset = INDEX_TABLE_STARTING_OFFSET;
    }

    protected DatFile FromBytesInternal(byte[] bytes)
    {
        var header = BuildDatHeaderFromBytesInternal(bytes);
        var indexTable = BuildIndexTableFromBytesInternal(bytes, header.KeyCount);
        var valueTable = BuildValueTableFromBytesInternal(bytes, indexTable, header.KeyCount);
        var keyTable = BuildKeyTableFromBytesInternal(bytes, indexTable, header.KeyCount);
        return new DatFile(header, indexTable, valueTable, keyTable);
    }

    private DatHeader BuildDatHeaderFromBytesInternal(byte[] bytes)
    {
        return new DatHeader(BitConverter.ToUInt32(bytes, HEADER_STARTING_OFFSET));
    }

    private IndexTable BuildIndexTableFromBytesInternal(byte[] bytes, uint keyCount)
    {
        var indexTable = new List<IndexTableRecord>();
        for (var i = 0; i < keyCount; i++)
        {
            var crc32 = BitConverter.ToUInt32(bytes, m_currentOffset);
            m_currentOffset += sizeof(uint);
            var valueLenght = BitConverter.ToUInt32(bytes, m_currentOffset);
            m_currentOffset += sizeof(uint);
            var keyLength = BitConverter.ToUInt32(bytes, m_currentOffset);
            m_currentOffset += sizeof(uint);
            var indexTableRecord = new IndexTableRecord(crc32, keyLength, valueLenght);
            indexTable.Add(indexTableRecord);
        }

        return new IndexTable(indexTable);
    }

    private ValueTable BuildValueTableFromBytesInternal(byte[] bytes, IndexTable indexTable, uint keyCount)
    {
        var valueTableRecords = new List<ValueTableRecord>();

        for (var i = 0; i < keyCount; i++)
        {
            long valueLength = indexTable.IndexTableRecords[i]?.ValueLength ??
                               throw new IndexAndValueTableOutOfSyncException(
                                   $"Building the DAT file failed at offset {m_currentOffset} due to an invalid IndexTableRecord at position {i}");
            var valueTableRecord = new ValueTableRecord(bytes, m_currentOffset, valueLength);
            m_currentOffset += valueTableRecord.Size;
            valueTableRecords.Add(valueTableRecord);
        }

        return new ValueTable(valueTableRecords);
    }

    private KeyTable BuildKeyTableFromBytesInternal(byte[] bytes, IndexTable indexTable,
        uint keyCount)
    {
        var keyTableRecords = new List<KeyTableRecord>();

        for (var i = 0; i < keyCount; i++)
        {
            long keyLength = indexTable.IndexTableRecords[i]?.KeyLength ??
                             throw new IndexAndValueTableOutOfSyncException(
                                 $"Building the DAT file failed at offset {m_currentOffset} due to an invalid IndexTableRecord at position {i}");
            var keyTableRecord = new KeyTableRecord(bytes, m_currentOffset, keyLength);
            m_currentOffset += keyTableRecord.Size;
            keyTableRecords.Add(keyTableRecord);
        }

        return new KeyTable(keyTableRecords);
    }

    protected DatFile FromHolderInternal(List<Tuple<string, string>> content)
    {
        var datHeader = new DatHeader(Convert.ToUInt32(content.Count));
        var indexTableRecords = new List<IndexTableRecord>();
        var valueTableRecords = new List<ValueTableRecord>();
        var keyTableRecords = new List<KeyTableRecord>();

        foreach (var keyValuePair in content)
        {
            if (null == keyValuePair)
            {
                continue;
            }

            var key = keyValuePair.Item1?.Replace("\0", string.Empty) ?? string.Empty;
            var value = keyValuePair.Item2;

            if (StringUtility.IsNullEmptyOrWhiteSpace(key))
            {
                throw new DatFileContentInvalidException(
                    $"A key may never be null. Provided key-value-pair {keyValuePair.Item1}:{keyValuePair.Item2}");
            }

            if (null == value)
            {
                value = "";
            }

            var valueTableRecord = new ValueTableRecord(value);
            var keyTableRecord = new KeyTableRecord(key);
            var indexTableRecord = new IndexTableRecord(ChecksumService.GetChecksum(key),
                Convert.ToUInt32(key.Length), Convert.ToUInt32(value.Length));

            valueTableRecords.Add(valueTableRecord);
            keyTableRecords.Add(keyTableRecord);
            indexTableRecords.Add(indexTableRecord);
        }

        var indexTable = new IndexTable(indexTableRecords);
        var valueTable = new ValueTable(valueTableRecords);
        var keyTable = new KeyTable(keyTableRecords);
        return new DatFile(datHeader, indexTable, valueTable, keyTable);
    }
}
