namespace PG.StarWarsGame.Files.DAT.Services;

/// <summary>
/// Informs about the merge operation that was performed on a specific key.
/// </summary>
public enum MergeOperation
{
    /// <summary>
    /// The key and its value have been added.
    /// </summary>
    Added,
    /// <summary>
    /// The key was already present and its value was overwritten.
    /// </summary>
    Overwritten
}