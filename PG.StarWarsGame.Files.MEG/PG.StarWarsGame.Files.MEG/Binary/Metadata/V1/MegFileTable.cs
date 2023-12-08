// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;

internal class MegFileTable : BinaryBase, IMegFileTable, IEnumerable<MegFileTableRecord>
{
    private readonly IReadOnlyList<MegFileTableRecord> _megFileContentTableRecords;

    public MegFileTableRecord this[int i] => _megFileContentTableRecords[i];

    IMegFileDescriptor IBinaryTable<IMegFileDescriptor>.this[int i] => this[i];

    public int Count => _megFileContentTableRecords.Count;

    public MegFileTable(IList<MegFileTableRecord> megFileContentTableRecords)
    {
        if (megFileContentTableRecords is null)
            throw new ArgumentNullException(nameof(megFileContentTableRecords));
        _megFileContentTableRecords = megFileContentTableRecords.ToList();
    }

    protected override int GetSizeCore()
    {
        if (_megFileContentTableRecords.Count == 0)
            return 0;
        if (_megFileContentTableRecords.Count == 1)
            return _megFileContentTableRecords[0].Size;
        return _megFileContentTableRecords.Sum(megFileNameTableRecord => megFileNameTableRecord.Size);
    }

    protected override byte[] ToBytesCore()
    {
        if (Size == 0)
            return Array.Empty<byte>();
        var bytes = new List<byte>(Size);
        foreach (var megFileContentTableRecord in _megFileContentTableRecords)
            bytes.AddRange(megFileContentTableRecord.Bytes);
        return bytes.ToArray();
    }

    IEnumerator<IMegFileDescriptor> IEnumerable<IMegFileDescriptor>.GetEnumerator()
    {
        return _megFileContentTableRecords.Cast<IMegFileDescriptor>().GetEnumerator();
    }

    public IEnumerator<MegFileTableRecord> GetEnumerator()
    {
        return _megFileContentTableRecords.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}