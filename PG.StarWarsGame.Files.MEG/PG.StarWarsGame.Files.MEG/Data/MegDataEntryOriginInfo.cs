// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Data;

/// <summary>
/// Contains information about the actual file location of an archived MEG data entry.
/// </summary>
public sealed class MegDataEntryOriginInfo : IEquatable<MegDataEntryOriginInfo>
{
    /// <summary>
    /// Gets the MEG file's data entry. <see langeword="null"/> if not present.
    /// </summary>
    public MegFileDataEntryLocation? MegFileLocation { get; }

    /// <summary>
    /// Gets the file's path on the file system. <see langeword="null"/> if not present.
    /// </summary>
    public string? FilePath { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegDataEntryOriginInfo"/> structure to the specified file path.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="filePath"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">If <paramref name="filePath"/> is empty or only whitespace.</exception>
    public MegDataEntryOriginInfo(string filePath)
    {
        if (filePath == null) 
            throw new ArgumentNullException(nameof(filePath));
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException(nameof(filePath));
        FilePath = filePath;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegDataEntryOriginInfo"/> structure to the specified MEG file's data entry.
    /// </summary>
    /// <param name="megFileDataEntry">The MEG file's data entry.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="megFileDataEntry"/> is <see langword="null"/>.</exception>
    public MegDataEntryOriginInfo(MegFileDataEntryLocation megFileDataEntry)
    {
        MegFileLocation = megFileDataEntry ?? throw new ArgumentNullException(nameof(megFileDataEntry));
    }

    /// <inheritdoc/>
    public bool Equals(MegDataEntryOriginInfo? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        return Equals(MegFileLocation, other.MegFileLocation) && string.Equals(FilePath, other.FilePath, StringComparison.Ordinal);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is MegDataEntryOriginInfo other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(MegFileLocation, FilePath);
    }
}