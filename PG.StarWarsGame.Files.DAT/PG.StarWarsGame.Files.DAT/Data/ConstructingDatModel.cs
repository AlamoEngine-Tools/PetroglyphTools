// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AnakinRaW.CommonUtilities.Collections;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Data;

internal class ConstructingDatModel(IEnumerable<DatStringEntry> entries, DatFileType fileType) : IDatModel
{
    private readonly IList<DatStringEntry> _entries = fileType == DatFileType.NotOrdered ? entries.ToList() : Crc32Utilities.SortByCrc32(entries);

    public int Count => _entries.Count;

    public DatFileType KeySortOder { get; } = fileType;

    public DatStringEntry this[int index] => _entries[index];

    public IEnumerator<DatStringEntry> GetEnumerator()
    {
        return _entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }


    public ISet<string> Keys => throw new NotSupportedException();

    public ISet<Crc32> CrcKeys => throw new NotSupportedException();

    public bool ContainsKey(string key) => throw new NotSupportedException();

    public string GetValue(string key) => throw new NotSupportedException();

    public bool TryGetValue(string key, [NotNullWhen(true)] out string? value) => throw new NotSupportedException();

    public bool ContainsKey(Crc32 key) => throw new NotSupportedException();

    public ReadOnlyFrugalList<DatStringEntry> EntriesWithCrc(Crc32 key) => throw new NotSupportedException();

    public ReadOnlyFrugalList<DatStringEntry> EntriesWithKey(string key) => throw new NotSupportedException();

    public string GetValue(Crc32 key) => throw new NotSupportedException();

    public bool TryGetValue(Crc32 key, [NotNullWhen(true)] out string? value) => throw new NotSupportedException();
}