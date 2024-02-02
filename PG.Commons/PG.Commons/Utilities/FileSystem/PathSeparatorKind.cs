namespace PG.Commons.Utilities.FileSystem;

/// <summary>
/// Determines which path separator path normalization shall use.
/// </summary>
public enum PathSeparatorKind
{
    /// <summary>
    /// Uses the default path separator of the current system.
    /// </summary>
    System,
    /// <summary>
    /// Uses the Windows path separator, which is the backslash character '\'.
    /// </summary>
    Windows,
    /// <summary>
    /// Uses the Linux path separator, which is the forward slash character '/'.
    /// </summary>
    Linux
}