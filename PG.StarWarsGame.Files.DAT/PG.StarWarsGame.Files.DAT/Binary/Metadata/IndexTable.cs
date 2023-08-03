// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.Binary;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal sealed class IndexTable : BinaryBase, IIndexTable, IEnumerable<IndexTableRecord>
{
    private readonly IReadOnlyList<IndexTableRecord> _indexTableRecords;

    public IndexTableRecord this[int i] => _indexTableRecords[i];
    IDatRecordDescriptor IIndexTable.this[int i] => this[i];

    public IndexTable(IList<IndexTableRecord> indexTableRecords)
    {
        if (indexTableRecords is null) throw new ArgumentNullException(nameof(indexTableRecords));

        _indexTableRecords = indexTableRecords.ToList();
    }

    protected override int GetSizeCore()
    {
        return _indexTableRecords.Sum(indexTableRecord => indexTableRecord.Size);
    }

    protected override byte[] ToBytesCore()
    {
        var bytes = new List<byte>(Size);
        foreach (IndexTableRecord? record in _indexTableRecords) bytes.AddRange(record.Bytes);
        return bytes.ToArray();
    }

    IEnumerator<IDatRecordDescriptor> IEnumerable<IDatRecordDescriptor>.GetEnumerator()
    {
        return _indexTableRecords.Cast<IDatRecordDescriptor>().GetEnumerator();
    }

    public IEnumerator<IndexTableRecord> GetEnumerator()
    {
        return _indexTableRecords.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}