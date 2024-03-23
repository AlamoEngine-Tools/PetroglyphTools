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

class DatModel : IDatModel
{
    private readonly ReadOnlyCollection<DatStringEntry> _entries;

    private readonly IReadOnlyDictionary<string, string> _lastKeyValueDictionary;
    private readonly IReadOnlyDictionary<Crc32, string> _lastCrcKeyValueDictionary;

    private bool Sorted => KeySortOder == DatFileType.OrderedByCrc32;

    [MemberNotNullWhen(true, nameof(Sorted))]
    private IReadOnlyDictionary<Crc32, IndexRange>? CrcToIndexMap { get; }

    public int Count => _entries.Count;

    public DatStringEntry this[int index] => _entries[index];

    public ISet<string> Keys => new HashSet<string>(_lastKeyValueDictionary.Keys);

    public ISet<Crc32> CrcKeys => new HashSet<Crc32>(_lastCrcKeyValueDictionary.Keys);

    public DatFileType KeySortOder { get; }

    public DatModel(IList<DatStringEntry> entries)
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
            if (entry.Crc32 < lastCrc)
                sorted = false;

            lastKeyValueDict[entry.Key] = entry.Value;
            lastCrcKeyValueDict[entry.Crc32] = entry.Value;
            copyList.Add(entry);

            lastCrc = entry.Crc32;
        }

        _entries = new ReadOnlyCollection<DatStringEntry>(copyList);
        _lastKeyValueDictionary = lastKeyValueDict;
        _lastCrcKeyValueDictionary = lastCrcKeyValueDict;

        KeySortOder = sorted ? DatFileType.OrderedByCrc32 : DatFileType.NotOrdered;

        if (sorted)
            CrcToIndexMap = Crc32Utilities.ListToCrcIndexRangeTable(_entries);
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

    public ReadOnlyFrugalList<DatStringEntry> EntriesWithCrc(Crc32 crc)
    {
        if (Sorted)
            return Crc32Utilities.ItemsWithCrc(crc, CrcToIndexMap!, _entries);

        if (!ContainsKey(crc))
            return ReadOnlyFrugalList<DatStringEntry>.Empty;

        var list = new FrugalList<DatStringEntry>();
        foreach (var entry in _entries)
        {
            if (entry.Crc32 == crc)
                list.Add(entry);
        }
        return list.AsReadOnly();
    }

    public ReadOnlyFrugalList<DatStringEntry> EntriesWithKey(string key)
    {
        if (!_lastKeyValueDictionary.ContainsKey(key))
            return ReadOnlyFrugalList<DatStringEntry>.Empty;

        var crc = _entries.First(e => e.Key.Equals(key)).Crc32;
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
        return _entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}