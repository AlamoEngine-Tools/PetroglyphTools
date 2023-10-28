using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using DotNet.Globbing;
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

        Entries = new ReadOnlyCollection<T>(entries.ToList());
        _fileNames = new ReadOnlyCollection<string>(Entries.Select(x => x.FilePath).ToList());
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
    public IReadOnlyList<T> FindAllEntries(string searchPattern)
    {
        Debug.Assert(_fileNames.Count == Entries.Count);

        if (Entries.Count == 0)
            return Array.Empty<T>();

        var glob = Glob.Parse(searchPattern,
            new GlobOptions { Evaluation = new EvaluationOptions { CaseInsensitive = true } });

        var foundMatches = new List<T>();

        for (var i = 0; i < _fileNames.Count; i++)
        {
            if (glob.IsMatch(_fileNames[i]))
                foundMatches.Add(Entries[i]);
        }

        return foundMatches;
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