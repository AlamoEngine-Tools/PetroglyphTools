// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;

internal class MegFileTable : BinaryBase, IMegFileTable, IEnumerable<MegFileContentTableRecord>
{
    private readonly IReadOnlyList<MegFileContentTableRecord> _megFileContentTableRecords;

    public MegFileContentTableRecord this[int i] => _megFileContentTableRecords[i];

    IMegFileDescriptor IBinaryTable<IMegFileDescriptor>.this[int i] => this[i];

    public int Count => _megFileContentTableRecords.Count;

    public MegFileTable(IList<MegFileContentTableRecord> megFileContentTableRecords)
    {
        if (megFileContentTableRecords is null)
            throw new ArgumentNullException(nameof(megFileContentTableRecords));
        if (megFileContentTableRecords.Count == 0)
            throw new ArgumentException("FileTable must not be empty.");
        _megFileContentTableRecords = megFileContentTableRecords.ToList();
    }

    protected override int GetSizeCore()
    {
        return _megFileContentTableRecords.Sum(megFileNameTableRecord => megFileNameTableRecord.Size);
    }

    protected override byte[] ToBytesCore()
    {
        var bytes = new List<byte>(Size);
        foreach (var megFileContentTableRecord in _megFileContentTableRecords)
            bytes.AddRange(megFileContentTableRecord.Bytes);
        return bytes.ToArray();
    }

    IEnumerator<IMegFileDescriptor> IEnumerable<IMegFileDescriptor>.GetEnumerator()
    {
        return _megFileContentTableRecords.Cast<IMegFileDescriptor>().GetEnumerator();
    }

    public IEnumerator<MegFileContentTableRecord> GetEnumerator()
    {
        return _megFileContentTableRecords.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}