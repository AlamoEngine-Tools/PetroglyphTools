using System.Collections.Generic;

namespace PG.StarWarsGame.Files.MEG.Data;

/// <summary>
/// Data model to represent a PG MEG archive.
/// </summary>
public interface IMegArchive : IReadOnlyList<MegFileDataEntry>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public bool Contains(MegFileDataEntry entry);

    /// <summary>
    /// Tries to find any <see cref="MegFileDataEntry"/> by matching the provided filename.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="megFileDataEntries"></param>
    /// <returns></returns>
    bool TryGetAllEntriesWithMatchingPattern(string fileName, out IReadOnlyList<MegFileDataEntry> megFileDataEntries);
}