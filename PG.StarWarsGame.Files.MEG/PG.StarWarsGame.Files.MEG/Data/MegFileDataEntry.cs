using System;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Data;

/// <summary>
/// 
/// </summary>
public sealed class MegFileDataEntry
{
    /// <summary>
    /// 
    /// </summary>
    public MegDataEntry DataEntry { get; }

    /// <summary>
    /// 
    /// </summary>
    public IMegFile MegFile { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="megFile"></param>
    /// <param name="dataEntry"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public MegFileDataEntry(IMegFile megFile, MegDataEntry dataEntry)
    {
        MegFile = megFile ?? throw new ArgumentNullException(nameof(megFile));
        DataEntry = dataEntry ?? throw new ArgumentNullException(nameof(dataEntry));
    }
}