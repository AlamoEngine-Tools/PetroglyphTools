// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.Binary;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal sealed class KeyTable : BinaryBase, IKeyTable, IEnumerable<KeyTableRecord>
{
    private readonly IReadOnlyList<KeyTableRecord> _keyTableRecords;

    public KeyTableRecord this[int i] => _keyTableRecords[i];
    string IKeyTable.this[int i] => _keyTableRecords[i].Key;

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
        foreach (KeyTableRecord? keyTableRecord in _keyTableRecords)
            bytes.AddRange(keyTableRecord.Bytes);
        return bytes.ToArray();
    }

    IEnumerator<string> IEnumerable<string>.GetEnumerator()
    {
        return _keyTableRecords.Select(x => x.Key).GetEnumerator();
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