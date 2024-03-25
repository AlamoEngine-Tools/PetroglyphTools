// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AnakinRaW.CommonUtilities.Collections;
using PG.Commons.DataTypes;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Data;

internal sealed class SortedDatModel : DatModel, ISortedDatModel
{
    private readonly IReadOnlyDictionary<Crc32, IndexRange> _crcToIndexMap;

    public override DatFileType KeySortOder => DatFileType.OrderedByCrc32;


    public SortedDatModel(IList<DatStringEntry> entries) : base(entries)
    {
        _crcToIndexMap = Crc32Utilities.ListToCrcIndexRangeTable(Entries);
    }

    public override ReadOnlyFrugalList<DatStringEntry> EntriesWithCrc(Crc32 crc)
    {
        return Crc32Utilities.ItemsWithCrc(crc, _crcToIndexMap!, Entries);
    }

    public IUnsortedDatModel ToUnsortedModel()
    {
        return new UnsortedDatModel(Entries);
    }
}

internal sealed class UnsortedDatModel(IList<DatStringEntry> entries) : DatModel(entries), IUnsortedDatModel
{
    public override DatFileType KeySortOder => DatFileType.NotOrdered;

    public override ReadOnlyFrugalList<DatStringEntry> EntriesWithCrc(Crc32 crc)
    {
        if (!ContainsKey(crc))
            return ReadOnlyFrugalList<DatStringEntry>.Empty;

        var list = new FrugalList<DatStringEntry>();
        foreach (var entry in Entries)
        {
            if (entry.Crc32 == crc)
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

internal abstract class DatModel : IDatModel
{
    protected readonly ReadOnlyCollection<DatStringEntry> Entries;

    private readonly IReadOnlyDictionary<string, string> _lastKeyValueDictionary;
    private readonly IReadOnlyDictionary<Crc32, string> _lastCrcKeyValueDictionary;


    public int Count => Entries.Count;

    public DatStringEntry this[int index] => Entries[index];

    public ISet<string> Keys => new HashSet<string>(_lastKeyValueDictionary.Keys);

    public ISet<Crc32> CrcKeys => new HashSet<Crc32>(_lastCrcKeyValueDictionary.Keys);

    public abstract DatFileType KeySortOder { get; }

    protected DatModel(IList<DatStringEntry> entries)
    {
        if (entries == null)
            throw new ArgumentNullException(nameof(entries));

        var copyList = new List<DatStringEntry>(entries.Count);

        var lastKeyValueDict = new Dictionary<string, string>();
        var lastCrcKeyValueDict = new Dictionary<Crc32, string>();

        var sorted = true;
        var lastCrc = default(Crc32);
        foreach (var entry in entries)
        {
            lastKeyValueDict[entry.Key] = entry.Value;
            lastCrcKeyValueDict[entry.Crc32] = entry.Value;
            copyList.Add(entry);

            if (entry.Crc32 <  lastCrc)
                sorted = false;

            lastCrc = entry.Crc32;
        }

        if (this is ISortedDatModel && !sorted)
            throw new InvalidOperationException("Unable to create sorted DAT model from unsorted entries.");

        Entries = new ReadOnlyCollection<DatStringEntry>(copyList);
        _lastKeyValueDictionary = lastKeyValueDict;
        _lastCrcKeyValueDictionary = lastCrcKeyValueDict;
    }

    public bool ContainsKey(string key)
    {
        return Keys.Contains(key);
    }

    public string GetValue(string key)
    {
        return _lastKeyValueDictionary[key];
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

    public bool ContainsKey(Crc32 keyCrc)
    {
        return CrcKeys.Contains(keyCrc);
    }

    public abstract ReadOnlyFrugalList<DatStringEntry> EntriesWithCrc(Crc32 crc);

    public ReadOnlyFrugalList<DatStringEntry> EntriesWithKey(string key)
    {
        if (!_lastKeyValueDictionary.ContainsKey(key))
            return ReadOnlyFrugalList<DatStringEntry>.Empty;

        var crc = Entries.First(e => e.Key.Equals(key)).Crc32;
        return EntriesWithCrc(crc);
    }

    public string GetValue(Crc32 key)
    {
        return _lastCrcKeyValueDictionary[key];
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