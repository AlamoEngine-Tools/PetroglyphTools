using System;

namespace PG.StarWarsGame.Files.MEG.Data;

/// <summary>
/// 
/// </summary>
public sealed class MegDataEntryOriginInfo
{
    /// <summary>
    /// 
    /// </summary>
    public MegFileDataEntry? MegFileLocation { get; }

    /// <summary>
    /// 
    /// </summary>
    public string? FilePath { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filePath"></param>
    public MegDataEntryOriginInfo(string filePath)
    {
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="megFileLocation"></param>
    public MegDataEntryOriginInfo(MegFileDataEntry megFileLocation)
    {
        MegFileLocation = megFileLocation ?? throw new ArgumentNullException(nameof(megFileLocation));
    }
}