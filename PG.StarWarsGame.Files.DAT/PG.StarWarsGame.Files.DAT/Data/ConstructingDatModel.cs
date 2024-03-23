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

internal class ConstructingDatModel : IDatModel
{
    private readonly IList<DatStringEntry> _entries;

    public int Count => _entries.Count;

    public DatFileType KeySortOder { get; }

    public DatStringEntry this[int index] => throw new NotImplementedException();

    public ISet<string> Keys => throw new NotImplementedException();

    public ISet<Crc32> CrcKeys => throw new NotImplementedException();


    public ConstructingDatModel(IEnumerable<DatStringEntry> entries, DatFileType fileType)
    {
        _entries = fileType == DatFileType.NotOrdered ? entries.ToList() : Crc32Utilities.SortByCrc32(entries);
        if (_entries.Count == 0)
            throw new ArgumentException("Dat file cannot be empty.", nameof(entries));
        KeySortOder = fileType;
    }

    public bool ContainsKey(string key) => throw new NotSupportedException();

    public string GetValue(string key) => throw new NotSupportedException();

    public bool TryGetValue(string key, [NotNullWhen(true)] out string? value) => throw new NotSupportedException();

    public bool ContainsKey(Crc32 keyCrc) => throw new NotSupportedException();

    public ReadOnlyFrugalList<DatStringEntry> EntriesWithCrc(Crc32 crc) => throw new NotSupportedException();

    public ReadOnlyFrugalList<DatStringEntry> EntriesWithKey(string key) => throw new NotSupportedException();

    public string GetValue(Crc32 key) => throw new NotSupportedException();

    public bool TryGetValue(Crc32 key, [NotNullWhen(true)] out string? value) => throw new NotSupportedException();

    public IEnumerator<DatStringEntry> GetEnumerator()
    {
        return _entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}