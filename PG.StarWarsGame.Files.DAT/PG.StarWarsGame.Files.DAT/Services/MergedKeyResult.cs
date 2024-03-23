using PG.StarWarsGame.Files.DAT.Data;

namespace PG.StarWarsGame.Files.DAT.Services;

/// <summary>
/// Represents the result of an altering merge operation of a specific key.
/// </summary>
public readonly struct MergedKeyResult
{
    /// <summary>
    /// Gets the entry that was added or overwritten.
    /// </summary>
    public DatStringEntry NewEntry { get; }

    /// <summary>
    /// Gets the last entry that was overwritten.
    /// </summary>
    public DatStringEntry? OldEntry { get; }

    /// <summary>
    /// Gets a value indicating whether the new entry was added or overwritten.
    /// </summary>
    public MergeOperation Status => !OldEntry.HasValue ? MergeOperation.Added : MergeOperation.Overwritten;

    internal MergedKeyResult(DatStringEntry newEntry, DatStringEntry? oldEntry = null)
    {
        NewEntry = newEntry;
        OldEntry = oldEntry;
    }
}