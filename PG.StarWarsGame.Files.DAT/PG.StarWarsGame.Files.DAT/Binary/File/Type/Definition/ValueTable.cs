// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PG.Commons.Binary;
using PG.Commons.Binary.File;

namespace PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition;

public sealed class ValueTable : IBinaryFile, ISizeable
{
    public List<ValueTableRecord> ValueTableRecords { get; }

    public ValueTable(List<ValueTableRecord> valueTableRecords)
    {
        ValueTableRecords = valueTableRecords;
    }

    public byte[] ToBytes()
    {
        var b = new List<byte>();
        foreach (var keyTableRecord in ValueTableRecords.Where(
                     keyTableRecord => keyTableRecord != null))
        {
            Debug.Assert(keyTableRecord != null, nameof(keyTableRecord) + " != null");
            b.AddRange(keyTableRecord.ToBytes());
        }

        return b.ToArray();
    }

    public int Size =>
        ValueTableRecords.Aggregate(0, (current, valueTableRecord) => current + (valueTableRecord?.Size ?? 0));
}
