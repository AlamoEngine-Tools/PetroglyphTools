// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using AnakinRaW.CommonUtilities.Collections;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

/// <summary>
/// Holds a CRC32-sorted list of MEG data entries.
/// </summary>
/// <typeparam name="T">The type of the data entry.</typeparam>
public interface IMegDataEntryHolder<T> : IReadOnlyList<T> where T : IMegDataEntry
{
    /// <summary>
    /// Determines whether the <see cref="IMegDataEntryHolder{T}"/> contains a specific file entry.
    /// </summary>
    /// <param name="entry">The entry to locate in the <see cref="IMegArchive"/>.</param>
    /// <returns><see langword="true"/> if the <typeparamref name="T"/> is found in the archive; otherwise, <see langword="false"/>.</returns>
    bool Contains(T entry);

    /// <summary>
    /// Determines the index of a specific file entry in the <see cref="IMegDataEntryHolder{T}"/>.
    /// </summary>
    /// <param name="entry">The entry to locate in the <see cref="IMegDataEntryHolder{T}"/>.</param>
    /// <returns>The index of <paramref name="entry"/> if found in the list; otherwise, -1.</returns>
    int IndexOf(T entry);

    /// <summary>
    /// Gets a list of data entries with the matching CRC32 checksum. 
    /// </summary>
    /// <param name="crc">The CRC to match.</param>
    /// <returns>List of matching data entries. </returns>
    ReadOnlyFrugalList<T> EntriesWithCrc(Crc32 crc);

    /// <summary>
    /// Get the last data entry with the matching CRC32 checksum or <see langword="null"/> if the no entry is found.
    /// </summary>
    /// <param name="crc">The CRC to match.</param>
    /// <returns><see langword="null"/> if no entry is found; otherwise the last entry in the <see cref="IMegDataEntryHolder{T}"/>.</returns>
    T? FirstEntryWithCrc(Crc32 crc);

    /// <summary>
    /// Tries to find any <typeparamref name="T"/> by matching the specified search pattern.
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
    /// <param name="caseInsensitive">When <see langword="true"/> the pattern ignores the character casing.</param>
    /// <returns>A list with all entries matching the specified pattern.</returns>
    /// <exception cref="ArgumentException"><paramref name="searchPattern"/> is empty.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="searchPattern"/> is <see langword="null"/>.</exception>
    ReadOnlyFrugalList<T> FindAllEntries(string searchPattern, bool caseInsensitive);
}