using System;
using System.Collections;
using System.Collections.Generic;

namespace PG.StarWarsGame.Files.MEG.Data;

/// <inheritdoc cref="IMegArchive"/>
public sealed class MegArchive : IMegArchive
{
    private readonly IReadOnlyList<MegFileDataEntry> _files;

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
        _files = new List<MegFileDataEntry>(files);
    }

    /// <inheritdoc />
    public bool Contains(MegFileDataEntry entry)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool TryGetAllEntriesWithMatchingPattern(string fileName,
        out IReadOnlyList<MegFileDataEntry> megFileDataEntries)
    {
        throw new NotImplementedException();
        //if (string.IsNullOrWhiteSpace(fileName))
        //{
        //    megFileDataEntries = Array.Empty<MegFileDataEntry>();
        //    return false;
        //}

        //megFileDataEntries = Content.Where(dataEntry => ContainsPathIgnoreCase(dataEntry.RelativeFilePath, fileName))
        //    .ToList();
        //return megFileDataEntries.Any();
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


    private static bool ContainsPathIgnoreCase(string relativePath, string partFileName)
    {
#if NETSTANDARD2_1_OR_GREATER
        return relativePath.Contains(partFileName, StringComparison.CurrentCultureIgnoreCase);
#else
        return relativePath.IndexOf(partFileName, StringComparison.CurrentCultureIgnoreCase) >= 0;
#endif
    }
}