using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AnakinRaW.CommonUtilities.Collections;
using PG.Commons.DataTypes;
using PG.Commons.Hashing;
using PG.Commons.Utilities;

namespace PG.StarWarsGame.Files.DAT.Data;

class DatModel : IDatModel
{
    private readonly ReadOnlyCollection<DatFileEntry> _entries;
    private readonly ReadOnlyCollection<string> _keys;
    private readonly IReadOnlyDictionary<Crc32, IndexRange> _crcToIndexMap;

    public int Count => _entries.Count;

    public DatFileEntry this[int index] => _entries[index];

    public DatKind Kind { get; }

    public DatModel(IList<DatFileEntry> entries, DatKind kind)
    {
        if (entries == null)
            throw new ArgumentNullException(nameof(entries));

        var copyList = new List<DatFileEntry>(entries.Count);
        var keys = new List<string>(entries.Count);

        var sorted = true;
        var lastCrc = default(Crc32);
        foreach (var entry in entries)
        {
            if (entry.Crc32 < lastCrc)
                sorted = false;
            keys.Add(entry.Key);
            copyList.Add(entry);
            lastCrc = entry.Crc32;
        }

        if (kind is DatKind.LocalizedStrings && !sorted)
            throw new ArgumentException("Localized string table is required to be sorted.");

        Kind = kind;
        _entries = new ReadOnlyCollection<DatFileEntry>(copyList);
        _keys = new ReadOnlyCollection<string>(keys);
        _crcToIndexMap = Crc32Utilities.ListToCrcIndexRangeTable(_entries);
    }

    public bool ContainsKey(string key)
    {
        return _keys.Contains(key);
    }

    public bool ContainsKey(Crc32 keyCrc)
    {
        return _crcToIndexMap.ContainsKey(keyCrc);
    }

    public ReadOnlyFrugalList<DatFileEntry> EntriesWithCrc(Crc32 crc)
    {
        if (!_crcToIndexMap.TryGetValue(crc, out var indexRange))
            return ReadOnlyFrugalList<DatFileEntry>.Empty;

        var length = indexRange.Length;

        if (length == 1)
            return new ReadOnlyFrugalList<DatFileEntry>(_entries[indexRange.Start]);

        var array = new DatFileEntry[length];
        for (var i = indexRange.Start; i < length; i++)
            array[i] = _entries[i];

        return new ReadOnlyFrugalList<DatFileEntry>(array);
    }

    public ReadOnlyFrugalList<DatFileEntry> EntriesWithKey(string key)
    {
        var first = _entries.FirstOrDefault(e => e.Key.Equals(key));
        if (first.Crc32 == default)
            return ReadOnlyFrugalList<DatFileEntry>.Empty;

        return EntriesWithCrc(first.Crc32);
    }

    public IEnumerator<DatFileEntry> GetEnumerator()
    {
        return _entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}