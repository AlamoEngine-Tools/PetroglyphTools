// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PG.Commons.Binary;
using PG.Commons.Binary.File;

namespace PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition;

internal sealed class IndexTable : IBinaryFile, ISizeable, IEquatable<IndexTable>
{
    public List<IndexTableRecord> IndexTableRecords { get; }

    public IndexTable(List<IndexTableRecord> indexTableRecords)
    {
        IndexTableRecords = indexTableRecords;
    }

    public byte[] ToBytes()
    {
        var b = new List<byte>();
        foreach (var indexTableRecord in IndexTableRecords.Where(indexTableRecord =>
                     indexTableRecord != null))
        {
            Debug.Assert(indexTableRecord != null, nameof(indexTableRecord) + " != null");
            b.AddRange(indexTableRecord.ToBytes());
        }

        return b.ToArray();
    }

    public int Size =>
        IndexTableRecords.Aggregate(0, (current, indexTableRecord) => current + (indexTableRecord?.Size ?? 0));

    #region Auto-Generated IEquatable<T> Implementation

    public bool Equals(IndexTable other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return IndexTableRecords.SequenceEqual(other.IndexTableRecords);
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is IndexTable other && Equals(other);
    }

    public override int GetHashCode()
    {
        return IndexTableRecords.GetHashCode();
    }

    public static bool operator ==(IndexTable left, IndexTable right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(IndexTable left, IndexTable right)
    {
        return !Equals(left, right);
    }

    #endregion
}
