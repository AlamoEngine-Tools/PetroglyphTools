// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.Binary;

namespace PG.StarWarsGame.Files.MEG.Binary.Metadata;

internal class MegFileNameTable : BinaryBase, IMegFileNameTable
{
    private readonly IReadOnlyList<MegFileNameTableRecord> _megFileNameTableRecords;

    public MegFileNameTableRecord this[int i] => _megFileNameTableRecords[i];

    public int Count => _megFileNameTableRecords.Count;

    public MegFileNameTable(IList<MegFileNameTableRecord> megFileNameTableRecords)
    {
        if (megFileNameTableRecords is null)
            throw new ArgumentNullException(nameof(megFileNameTableRecords));
        if (megFileNameTableRecords.Count == 0)
            _megFileNameTableRecords = Array.Empty<MegFileNameTableRecord>();
        else
            _megFileNameTableRecords = megFileNameTableRecords.ToList();
    }

    protected override int GetSizeCore()
    {
        if (_megFileNameTableRecords.Count == 0)
            return 0;
        if (_megFileNameTableRecords.Count == 1)
            return _megFileNameTableRecords[0].Size;
        return _megFileNameTableRecords.Sum(megFileNameTableRecord => megFileNameTableRecord.Size);
    }

    protected override byte[] ToBytesCore()
    {
        if (Size == 0)
            return Array.Empty<byte>();
        var bytes = new List<byte>(Size);
        foreach (var megFileNameTableRecord in _megFileNameTableRecords)
            bytes.AddRange(megFileNameTableRecord.Bytes);
        return bytes.ToArray();
    }

    IEnumerator<MegFileNameTableRecord> IEnumerable<MegFileNameTableRecord>.GetEnumerator()
    {
        return _megFileNameTableRecords.GetEnumerator();
    }

    public IEnumerator<MegFileNameTableRecord> GetEnumerator()
    {
        return _megFileNameTableRecords.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}