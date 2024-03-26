namespace PG.StarWarsGame.Files.DAT.Services.Builder;

/// <summary>
/// Contains constants for controlling how to treat duplicate keys when building DAT files.
/// </summary>
public enum BuilderOverrideKind
{
    /// <summary>
    /// The DAT file does not support duplicates. The existing key-value pair does not get replaced.
    /// </summary>
    NoOverwrite,
    /// <summary>
    /// The DAT file does not support duplicates. The existing key-value pair gets replaced by the new entry.
    /// </summary>
    Overwrite,
    /// <summary>
    /// The DAT file supports duplicates.
    /// </summary>
    AllowDuplicate
}