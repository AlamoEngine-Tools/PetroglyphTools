// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.DAT.Data;

namespace PG.StarWarsGame.Files.DAT.Services;

/// <summary>
/// Represents the result of a merge operation for a specific key.
/// </summary>
public readonly struct MergedKeyResult
{
    /// <summary>
    /// Gets the entry that was added or replaced an existing entry.
    /// </summary>
    public DatStringEntry NewEntry { get; }

    /// <summary>
    /// Gets the entry that was overwritten or <see langword="null"/> if no entry was overwritten.
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