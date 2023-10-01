using System.Collections.Generic;

namespace PG.StarWarsGame.Files.MEG.Data;

/// <summary>
/// Data model to represent a PG MEG archive.
/// </summary>
public interface IMegArchive : IReadOnlyList<MegFileDataEntry>
{
    /// <summary>
    /// Determines whether the <see cref="IMegArchive"/> contains a specific file entry.
    /// </summary>
    /// <param name="entry">The entry to locate in the <see cref="IMegArchive"/>.</param>
    /// <returns><see langword="true"/> if the <see cref="MegFileDataEntry"/> is found in the archive; otherwise, <see langword="false"/>.</returns>
    bool Contains(MegFileDataEntry entry);

    /// <summary>
    /// Determines the index of a specific file entry in the <see cref="IMegArchive"/>.
    /// </summary>
    /// <param name="entry">The entry to locate in the <see cref="IMegArchive"/>.</param>
    /// <returns>The index of <paramref name="entry"/> if found in the list; otherwise, -1.</returns>
    int IndexOf(MegFileDataEntry entry);

    /// <summary>
    /// Tries to find any <see cref="MegFileDataEntry"/> by matching the provided filename.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="megFileDataEntries"></param>
    /// <returns></returns>
    bool TryGetAllEntriesWithMatchingPattern(string fileName, out IReadOnlyList<MegFileDataEntry> megFileDataEntries);
}