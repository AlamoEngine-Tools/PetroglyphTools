// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using AnakinRaW.CommonUtilities.Collections;
using PG.Commons.Data;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Data;

internal sealed class SortedDatModel : DatModel, ISortedDatModel
{
    private readonly IReadOnlyDictionary<Crc32, IndexRange> _crcToIndexMap;

    public override DatFileType KeySortOrder => DatFileType.OrderedByCrc32;


    public SortedDatModel(IEnumerable<DatStringEntry> entries) : base(entries)
    {
        _crcToIndexMap = Crc32Utilities.ListToCrcIndexRangeTable(Entries);
    }

    public override ReadOnlyFrugalList<DatStringEntry> EntriesWithCrc(Crc32 key)
    {
        return Crc32Utilities.ItemsWithCrc(key, Entries, _crcToIndexMap);
    }

    public IUnsortedDatModel ToUnsortedModel()
    {
        return new UnsortedDatModel(Entries);
    }
}