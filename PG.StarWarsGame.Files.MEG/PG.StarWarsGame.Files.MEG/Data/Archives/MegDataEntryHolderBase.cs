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
    /// 
    /// </summary>
    protected readonly ReadOnlyCollection<T> Files;

    private readonly ReadOnlyCollection<string> _fileNames;

    /// <inheritdoc />
    public T this[int index] => Files[index];

    /// <inheritdoc />
    public int Count => Files.Count;

    /// <summary>
    /// Initializes a new instance of the <see cref="MegArchive"/> class
    /// by coping all elements of the given <paramref name="files"/> list.
    /// </summary>
    /// <param name="files">The list of files in this archive.</param>
    protected MegDataEntryHolderBase(IList<T> files)
    {
        if (files == null)
            throw new ArgumentNullException(nameof(files));

        Files = new ReadOnlyCollection<T>(files.ToList());
        _fileNames = new ReadOnlyCollection<string>(Files.Select(x => x.FilePath).ToList());
    }

    /// <inheritdoc />
    public bool Contains(T entry)
    {
        return Files.Contains(entry);
    }

    /// <inheritdoc />
    public int IndexOf(T entry)
    {
        return Files.IndexOf(entry);
    }

    /// <inheritdoc />
    public IReadOnlyList<T> FindAllEntries(string searchPattern)
    {
        Debug.Assert(_fileNames.Count == Files.Count);

        if (Files.Count == 0)
            return Array.Empty<T>();

        var glob = Glob.Parse(searchPattern,
            new GlobOptions { Evaluation = new EvaluationOptions { CaseInsensitive = true } });

        var foundMatches = new List<T>();

        for (var i = 0; i < _fileNames.Count; i++)
        {
            if (glob.IsMatch(_fileNames[i]))
                foundMatches.Add(Files[i]);
        }

        return foundMatches;
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        return Files.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}