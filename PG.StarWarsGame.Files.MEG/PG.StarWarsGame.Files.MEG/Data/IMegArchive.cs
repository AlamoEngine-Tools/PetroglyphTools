using System.Collections.Generic;

namespace PG.StarWarsGame.Files.MEG.Data;

/// <summary>
/// Data model to represent a PG MEG archive.
/// </summary>
public interface IMegArchive : IReadOnlyList<MegDataEntry>
{
    /// <summary>
    /// Determines whether the <see cref="IMegArchive"/> contains a specific file entry.
    /// </summary>
    /// <param name="entry">The entry to locate in the <see cref="IMegArchive"/>.</param>
    /// <returns><see langword="true"/> if the <see cref="MegDataEntry"/> is found in the archive; otherwise, <see langword="false"/>.</returns>
    bool Contains(MegDataEntry entry);

    /// <summary>
    /// Determines the index of a specific file entry in the <see cref="IMegArchive"/>.
    /// </summary>
    /// <param name="entry">The entry to locate in the <see cref="IMegArchive"/>.</param>
    /// <returns>The index of <paramref name="entry"/> if found in the list; otherwise, -1.</returns>
    int IndexOf(MegDataEntry entry);

    /// <summary>
    /// Tries to find any <see cref="MegDataEntry"/> by matching the provided search pattern.
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
    IReadOnlyList<MegDataEntry> FindAllEntries(string searchPattern);
}