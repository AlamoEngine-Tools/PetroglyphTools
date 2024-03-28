// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AnakinRaW.CommonUtilities.Collections;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Data;

internal abstract class DatModel : IDatModel
{
    protected readonly ReadOnlyCollection<DatStringEntry> Entries;

    private readonly IReadOnlyDictionary<string, string> _firstKeyValueDictionary;
    private readonly IReadOnlyDictionary<Crc32, string> _firstCrcKeyValueDictionary;


    public int Count => Entries.Count;

    public DatStringEntry this[int index] => Entries[index];

    public ISet<string> Keys => new HashSet<string>(_firstKeyValueDictionary.Keys);

    public ISet<Crc32> CrcKeys => new HashSet<Crc32>(_firstCrcKeyValueDictionary.Keys);

    public abstract DatFileType KeySortOder { get; }

    protected DatModel(IList<DatStringEntry> entries)
    {
        if (entries == null)
            throw new ArgumentNullException(nameof(entries));

        var copyList = new List<DatStringEntry>(entries.Count);

        var firstKeyValueDict = new Dictionary<string, string>();
        var firstCrcKeyValueDict = new Dictionary<Crc32, string>();

        var sorted = true;
        var lastCrc = default(Crc32);
        foreach (var entry in entries)
        {
            if (!firstCrcKeyValueDict.ContainsKey(entry.Crc32)) 
                firstCrcKeyValueDict.Add(entry.Crc32, entry.Value);
            if (!firstKeyValueDict.ContainsKey(entry.Key)) 
                firstKeyValueDict.Add(entry.Key, entry.Value);
            copyList.Add(entry);

            if (entry.Crc32 <  lastCrc)
                sorted = false;

            lastCrc = entry.Crc32;
        }

        if (this is ISortedDatModel && !sorted)
            throw new InvalidOperationException("Unable to create sorted DAT model from unsorted entries.");

        Entries = new ReadOnlyCollection<DatStringEntry>(copyList);
        _firstKeyValueDictionary = firstKeyValueDict;
        _firstCrcKeyValueDictionary = firstCrcKeyValueDict;
    }

    public bool ContainsKey(string key)
    {
        return Keys.Contains(key);
    }

    public string GetValue(string key)
    {
        return _firstKeyValueDictionary[key];
    }

    public bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
    {
        value = null;
        try
        {
            value = GetValue(key);
            return true;
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
    }

    public bool ContainsKey(Crc32 key)
    {
        return CrcKeys.Contains(key);
    }

    public abstract ReadOnlyFrugalList<DatStringEntry> EntriesWithCrc(Crc32 key);

    public ReadOnlyFrugalList<DatStringEntry> EntriesWithKey(string key)
    {
        if (!_firstKeyValueDictionary.ContainsKey(key))
            return ReadOnlyFrugalList<DatStringEntry>.Empty;

        var crc = Entries.First(e => e.Key.Equals(key)).Crc32;
        return EntriesWithCrc(crc);
    }

    public string GetValue(Crc32 key)
    {
        return _firstCrcKeyValueDictionary[key];
    }

    public bool TryGetValue(Crc32 key, [NotNullWhen(true)] out string? value)
    {
        value = null;
        try
        {
            value = GetValue(key);
            return true;
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
    }

    public IEnumerator<DatStringEntry> GetEnumerator()
    {
        return Entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}