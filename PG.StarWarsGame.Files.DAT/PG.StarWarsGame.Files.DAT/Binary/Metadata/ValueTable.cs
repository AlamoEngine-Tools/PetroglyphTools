// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.Binary;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal sealed class ValueTable : BinaryBase, IEnumerable<ValueTableRecord>
{
    private readonly IReadOnlyList<ValueTableRecord> _valueTableRecords;

    public ValueTableRecord this[int i] => _valueTableRecords[i];

    public ValueTable(IList<ValueTableRecord> valueTableRecords)
    {
        if (valueTableRecords is null) throw new ArgumentNullException(nameof(valueTableRecords));
        _valueTableRecords = valueTableRecords.ToList();
    }

    protected override int GetSizeCore()
    {
        return _valueTableRecords.Sum(r => r.Size);
    }

    protected override byte[] ToBytesCore()
    {
        var bytes = new List<byte>(Size);
        foreach (var valueTableRecord in _valueTableRecords)
            bytes.AddRange(valueTableRecord.Bytes);
        return bytes.ToArray();
    }

    public IEnumerator<ValueTableRecord> GetEnumerator()
    {
        return _valueTableRecords.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}