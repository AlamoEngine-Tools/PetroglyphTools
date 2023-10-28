// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

public interface IMegDataEntryHolder<T> : IReadOnlyList<T> where T : IMegDataEntry
{
    /// <summary>
    /// Determines whether the <see cref="IMegArchive"/> contains a specific file entry.
    /// </summary>
    /// <param name="entry">The entry to locate in the <see cref="IMegArchive"/>.</param>
    /// <returns><see langword="true"/> if the <typeparamref name="T"/> is found in the archive; otherwise, <see langword="false"/>.</returns>
    bool Contains(T entry);

    /// <summary>
    /// Determines the index of a specific file entry in the <see cref="IMegArchive"/>.
    /// </summary>
    /// <param name="entry">The entry to locate in the <see cref="IMegArchive"/>.</param>
    /// <returns>The index of <paramref name="entry"/> if found in the list; otherwise, -1.</returns>
    int IndexOf(T entry);

    /// <summary>
    /// Tries to find any <typeparamref name="T"/> by matching the provided search pattern.
    /// <br/>
    /// The resulting list is ordered by CRC32 of the file name, just are a MEG archive is ordered. The list is empty, if no matches are found. 
    /// </summary>
    /// <remarks>
    /// The search pattern supports globbing. So **/*.xml is a valid query.
    /// <br/>
    /// <br/>
    /// <b>NOTE:</b>
    /// File names in a MEG archive may be absolute or relative. They also can be non-canonical (e.g., "/../data/./config.meg").
    /// The search pattern might produce false-positive and false negatives, since this method is <b>not</b> designed to resolve paths.
    /// </remarks>
    /// <param name="searchPattern">The globbing pattern.</param>
    /// <returns></returns>
    // TODO: FrugalList<T> cause i expect patterns mostly to contain 0 or 1 item.
    IReadOnlyList<T> FindAllEntries(string searchPattern);
}