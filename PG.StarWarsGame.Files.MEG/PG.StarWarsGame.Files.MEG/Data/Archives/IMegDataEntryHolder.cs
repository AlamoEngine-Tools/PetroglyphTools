// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using AnakinRaW.CommonUtilities.Collections;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

/// <summary>
/// Represents a CRC32-sorted list of MEG data entries.
/// </summary>
/// <typeparam name="T">The type of the data entry.</typeparam>
public interface IMegDataEntryHolder<T> : IReadOnlyList<T> where T : IMegDataEntry
{
    /// <summary>
    /// Determines whether the <see cref="IMegDataEntryHolder{T}"/> contains a specific file entry.
    /// </summary>
    /// <param name="entry">The entry to locate in the <see cref="IMegDataEntryHolder{T}"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="entry"/> is found in the <see cref="IMegDataEntryHolder{T}"/>; otherwise, <see langword="false"/>.</returns>
    bool Contains(T entry);

    /// <summary>
    /// Determines the index of a specific file entry in the <see cref="IMegDataEntryHolder{T}"/>.
    /// </summary>
    /// <param name="entry">The entry to locate in the <see cref="IMegDataEntryHolder{T}"/>.</param>
    /// <returns>The index of <paramref name="entry"/> if found in the list; otherwise, -1.</returns>
    int IndexOf(T entry);

    /// <summary>
    /// Gets the list of data entries with the matching CRC32 checksum, or an empty list if the CRC32 checksum is not found.
    /// </summary>
    /// <param name="crc">The CRC32 checksum to match.</param>
    /// <returns>The list of matching data entries.</returns>
    ImmutableFrugalList<T> EntriesWithCrc(Crc32 crc);

    /// <summary>
    /// Gets the first data entry with the matching CRC32 checksum.
    /// </summary>
    /// <param name="crc">The CRC32 checksum to match.</param>
    /// <returns>The first entry in the <see cref="IMegDataEntryHolder{T}"/> with the specified checksum.</returns>
    /// <exception cref="KeyNotFoundException"><paramref name="crc"/> is not found in the <see cref="IMegDataEntryHolder{T}"/>.</exception>
    T FirstEntryWithCrc(Crc32 crc);

    /// <summary>
    /// Finds all entries matching the specified search pattern.
    /// <br/>
    /// The resulting list is ordered by CRC32 of the file name, just as a MEG archive is ordered. The list is empty if no matches are found.
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
    /// <param name="caseInsensitive"><see langword="true"/> to ignore the character casing of the pattern; otherwise, <see langword="false"/>.</param>
    /// <returns>A list of all entries matching the specified pattern.</returns>
    /// <exception cref="ArgumentException"><paramref name="searchPattern"/> is empty.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="searchPattern"/> is <see langword="null"/>.</exception>
    ImmutableFrugalList<T> FindAllEntries(string searchPattern, bool caseInsensitive);
}