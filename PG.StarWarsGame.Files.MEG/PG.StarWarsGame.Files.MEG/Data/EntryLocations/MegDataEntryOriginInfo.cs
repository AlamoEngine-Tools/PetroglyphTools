// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using AnakinRaW.CommonUtilities;

namespace PG.StarWarsGame.Files.MEG.Data.EntryLocations;

/// <summary>
/// The origin of a MEG data entry which is either packed in a MEG archive or present on the file system.
/// </summary>
public sealed class MegDataEntryOriginInfo : IDataEntryLocation, IEquatable<MegDataEntryOriginInfo>
{
    /// <summary>
    /// 
    /// </summary>
    [MemberNotNullWhen(true, nameof(FilePath))]
    public bool IsLocalFile => FilePath != null;

    /// <summary>
    /// 
    /// </summary>
    [MemberNotNullWhen(true, nameof(MegFileLocation))]
    public bool IsEntryReference => MegFileLocation != null;

    /// <summary>
    /// Gets the MEG file's data entry. <see langeword="null"/> if not present.
    /// </summary>
    public MegDataEntryLocationReference? MegFileLocation { get; }

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
        ThrowHelper.ThrowIfNullOrWhiteSpace(filePath);
        FilePath = filePath;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegDataEntryOriginInfo"/> structure to the specified MEG file's data entry.
    /// </summary>
    /// <param name="locationReference">The MEG file's data entry.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="locationReference"/> is <see langword="null"/>.</exception>
    public MegDataEntryOriginInfo(MegDataEntryLocationReference locationReference)
    {
        MegFileLocation = locationReference ?? throw new ArgumentNullException(nameof(locationReference));
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