// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.Binary;

namespace PG.StarWarsGame.Files.MEG.Binary.Shared.Metadata;

internal class MegFileNameTable : BinaryBase, IFileNameTable, IEnumerable<MegFileNameTableRecord>
{
    private readonly IReadOnlyList<MegFileNameTableRecord> _megFileNameTableRecords;

    public MegFileNameTableRecord this[int i] => _megFileNameTableRecords[i];

    string IFileNameTable.this[int i] => _megFileNameTableRecords[i].FileName;

    public MegFileNameTable(IList<MegFileNameTableRecord> megFileNameTableRecords)
    {
        if (megFileNameTableRecords is null)
            throw new ArgumentNullException(nameof(megFileNameTableRecords));
        if (megFileNameTableRecords.Count == 0)
            throw new ArgumentException("FileNameTable must not be empty.");
        _megFileNameTableRecords = megFileNameTableRecords.ToList();
    }

    protected override int GetSizeCore()
    {
        return _megFileNameTableRecords.Sum(megFileNameTableRecord => megFileNameTableRecord.Size);
    }

    protected override byte[] ToBytesCore()
    {
        var bytes = new List<byte>(Size);
        foreach (var megFileNameTableRecord in _megFileNameTableRecords)
            bytes.AddRange(megFileNameTableRecord.Bytes);
        return bytes.ToArray();
    }

    IEnumerator<string> IEnumerable<string>.GetEnumerator()
    {
        return _megFileNameTableRecords.Select(x => x.FileName).GetEnumerator();
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