// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using AnakinRaW.CommonUtilities.Collections;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Data;

internal sealed class UnsortedDatModel(IList<DatStringEntry> entries) : DatModel(entries), IUnsortedDatModel
{
    public override DatFileType KeySortOder => DatFileType.NotOrdered;

    public override ReadOnlyFrugalList<DatStringEntry> EntriesWithCrc(Crc32 key)
    {
        if (!ContainsKey(key))
            return ReadOnlyFrugalList<DatStringEntry>.Empty;

        var list = new FrugalList<DatStringEntry>();
        foreach (var entry in Entries)
        {
            if (entry.Crc32 == key)
                list.Add(entry);
        }
        return list.AsReadOnly();
    }

    public ISortedDatModel ToSortedModel()
    {
        var sortedEntries = Crc32Utilities.SortByCrc32(Entries);
        return new SortedDatModel(sortedEntries);
    }
}