using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using DotNet.Globbing;

namespace PG.StarWarsGame.Files.MEG.Data;

/// <inheritdoc cref="IMegArchive"/>
public sealed class MegArchive : IMegArchive
{
    private readonly ReadOnlyCollection<MegFileDataEntry> _files;

    private readonly ReadOnlyCollection<string> _fileNames;

    /// <inheritdoc />
    public MegFileDataEntry this[int index] => _files[index];

    /// <inheritdoc />
    public int Count => _files.Count;

    /// <summary>
    /// Initializes a new instance of the <see cref="MegArchive"/> class
    /// by coping all elements of the given <paramref name="files"/> list.
    /// </summary>
    /// <param name="files">The list of files in this archive.</param>
    public MegArchive(IList<MegFileDataEntry> files)
    {
        if (files == null) 
            throw new ArgumentNullException(nameof(files));

        _files = new ReadOnlyCollection<MegFileDataEntry>(files.ToList());
        _fileNames = new ReadOnlyCollection<string>(_files.Select(x => x.FilePath).ToList());
    }

    /// <inheritdoc />
    public bool Contains(MegFileDataEntry entry)
    {
        return _files.Contains(entry);
    }

    /// <inheritdoc />
    public int IndexOf(MegFileDataEntry entry)
    {
        return _files.IndexOf(entry);
    }

    /// <inheritdoc />
    public IReadOnlyList<MegFileDataEntry> FindAllEntries(string searchPattern)
    {
        Debug.Assert(_fileNames.Count == _files.Count);

        if (_files.Count == 0)
            return Array.Empty<MegFileDataEntry>();

        var glob = Glob.Parse(searchPattern,
            new GlobOptions { Evaluation = new EvaluationOptions { CaseInsensitive = true } });

        var foundMatches = new List<MegFileDataEntry>();
        
        for (var i = 0; i < _fileNames.Count; i++)
        {
            if (glob.IsMatch(_fileNames[i])) 
                foundMatches.Add(_files[i]);
        }

        return foundMatches;
    }

    /// <inheritdoc />
    public IEnumerator<MegFileDataEntry> GetEnumerator()
    {
        return _files.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}