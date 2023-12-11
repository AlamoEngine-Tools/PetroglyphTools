using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using DotNet.Globbing;
using PG.Commons.Collections;
using PG.Commons.DataTypes;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

/// <inheritdoc cref="IMegDataEntryHolder{T}"/>
public abstract class MegDataEntryHolderBase<T> : IMegDataEntryHolder<T> where T : IMegDataEntry
{
    /// <summary>
    /// All data entries of this instance.
    /// </summary>
    protected readonly ReadOnlyCollection<T> Entries;

    private readonly ReadOnlyCollection<string> _fileNames;

    private readonly IReadOnlyDictionary<Crc32, IndexRange> _crcToIndexMap;

    /// <inheritdoc />
    public T this[int index] => Entries[index];

    /// <inheritdoc />
    public int Count => Entries.Count;

    /// <summary>
    /// Initializes a new instance of the <see cref="MegArchive"/> class
    /// by coping all elements of the given <paramref name="entries"/> list.
    /// </summary>
    /// <param name="entries">The list of entries in this archive.</param>
    protected MegDataEntryHolderBase(IList<T> entries)
    {
        if (entries == null)
            throw new ArgumentNullException(nameof(entries));

        var copyList = new List<T>(entries.Count);
        var fileNames = new List<string>(entries.Count);

        var lastCrc = default(Crc32);
        foreach (var entry in entries)
        {
            if (entry.Crc32 < lastCrc)
                throw new ArgumentException("not sorted", nameof(entries));
            fileNames.Add(entry.FilePath);
            copyList.Add(entry);
            lastCrc = entry.Crc32;
        }

        Entries = new ReadOnlyCollection<T>(copyList);
        _fileNames = new ReadOnlyCollection<string>(fileNames);
        _crcToIndexMap = Crc32Utilities.ListToCrcIndexRangeTable(Entries);
    }

    /// <inheritdoc />
    public bool Contains(T entry)
    {
        return Entries.Contains(entry);
    }

    /// <inheritdoc />
    public int IndexOf(T entry)
    {
        return Entries.IndexOf(entry);
    }

    /// <inheritdoc />
    public ReadOnlyFrugalList<T> EntriesWithCrc(Crc32 crc)
    {
        if (!_crcToIndexMap.TryGetValue(crc, out var indexRange))
            return ReadOnlyFrugalList<T>.Empty;

        var length = indexRange.Length;
        
        if (length == 1)
            return new ReadOnlyFrugalList<T>(Entries[indexRange.Start]);

        var array = new T[length];
        for (var i = indexRange.Start; i < length; i++)
            array[i] = Entries[i];
        
        return new ReadOnlyFrugalList<T>(array);
    }

    /// <inheritdoc />
    public T? LastEntryWithCrc(Crc32 crc)
    {
        return EntriesWithCrc(crc).LastOrDefault();
    }

    /// <inheritdoc />
    public ReadOnlyFrugalList<T> FindAllEntries(string searchPattern)
    {
        Debug.Assert(_fileNames.Count == Entries.Count);

        if (Entries.Count == 0)
            return ReadOnlyFrugalList<T>.Empty;

        var glob = Glob.Parse(searchPattern,
            new GlobOptions { Evaluation = new EvaluationOptions { CaseInsensitive = true } });

        var foundMatches = new FrugalList<T>();

        for (var i = 0; i < _fileNames.Count; i++)
        {
            if (glob.IsMatch(_fileNames[i]))
                foundMatches.Add(Entries[i]);
        }

        return foundMatches.AsReadOnly();
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        return Entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}