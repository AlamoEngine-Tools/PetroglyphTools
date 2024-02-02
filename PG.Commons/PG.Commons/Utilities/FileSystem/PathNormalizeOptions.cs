namespace PG.Commons.Utilities.FileSystem;

/// <summary>
/// Options how path normalization shall be performed.
/// </summary>
public record PathNormalizeOptions
{
    /// <summary>
    /// Gets or sets whether directory separator shall be unified across the path based on <see cref="SeparatorKind"/>.
    /// </summary>
    public bool UnifySlashes { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public PathSeparatorKind SeparatorKind { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public UnifyCasingKind UnifyCase { get; init; }

    /// <summary>
    /// Gets or sets whether any trailing directory separators shall be removed at the end of the path.
    /// </summary>
    public bool TrimTrailingSeparator { get; init; }


}