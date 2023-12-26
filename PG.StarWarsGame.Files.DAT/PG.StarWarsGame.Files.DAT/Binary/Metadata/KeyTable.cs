// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.Binary;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal sealed class KeyTable : BinaryBase, IKeyTable
{
    private readonly IReadOnlyList<KeyTableRecord> _keyTableRecords;

    public int Count => _keyTableRecords.Count;

    public KeyTableRecord this[int index] => _keyTableRecords[index];

    public KeyTable(IList<KeyTableRecord> keyTableRecords)
    {
        if (keyTableRecords is null) throw new ArgumentNullException(nameof(keyTableRecords));
        _keyTableRecords = keyTableRecords.ToList();
    }

    protected override int GetSizeCore()
    {
        return _keyTableRecords.Sum(r => r.Size);
    }

    protected override byte[] ToBytesCore()
    {
        var bytes = new List<byte>(Size);
        foreach (var keyTableRecord in _keyTableRecords)
            bytes.AddRange(keyTableRecord.Bytes);
        return bytes.ToArray();
    }

    public IEnumerator<KeyTableRecord> GetEnumerator()
    {
        return _keyTableRecords.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}