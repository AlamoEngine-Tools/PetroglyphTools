namespace PG.StarWarsGame.Files.MEG.Services.FileSystem;

/// <summary>
/// Determines which directory separator path normalization shall use.
/// </summary>
internal enum DirectorySeparatorKind
{
    /// <summary>
    /// Uses the directory separators of the current system.
    /// </summary>
    System,
    /// <summary>
    /// Uses the Windows directory separators, which is the backslash character '\' as primary and forward slash '/' as secondary.
    /// </summary>
    Windows,
    /// <summary>
    /// Uses the Linux directory separator, which is the forward slash character '/'.
    /// </summary>
    Linux
}