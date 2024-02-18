namespace PG.Commons.Utilities.FileSystem;

/// <summary>
/// Determines which character casing path normalization shall use.
/// </summary>
public enum UnifyCasingKind
{
    /// <summary>
    /// No casing will be applied.
    /// </summary>
    None,
    /// <summary>
    /// If the underlying file system is case-insensitive, all characters will be replaced by their upper case variant.
    /// </summary>
    UpperCase,
    /// <summary>
    /// All characters will be replaced by their upper case variant, regardless of the underlying file system.
    /// </summary>
    UpperCaseForce,
    /// <summary>
    /// If the underlying file system is case-insensitive, all characters will be replaced by their lower case variant.
    /// </summary>
    LowerCase,
    /// <summary>
    /// All characters will be replaced by their lower case variant, regardless of the underlying file system.
    /// </summary>
    LowerCaseForce,
}