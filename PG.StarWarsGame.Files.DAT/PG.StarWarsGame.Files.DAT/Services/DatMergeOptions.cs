namespace PG.StarWarsGame.Files.DAT.Services;

/// <summary>
/// Specifies how merging should treat existing keys.
/// </summary>
public enum DatMergeOptions
{
    /// <summary>
    /// An existing key will be kept.
    /// </summary>
    KeepExisting,
    /// <summary>
    /// An existing key will be overwritten by the new value.
    /// </summary>
    Overwrite
}