// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

namespace PG.StarWarsGame.Files.MEG.Data.EntryLocations;

/// <summary>
/// The origin of a MEG data entry which is either packed in a MEG archive or present on the file system.
/// </summary>
public sealed class MegDataEntryOriginInfo : IDataEntryLocation, IEquatable<MegDataEntryOriginInfo>
{
    /// <summary>
    /// Gets a value indicating whether the data entry originates from a local file on the file system.
    /// </summary>
    /// <remarks>
    /// If this property returns <see langword="true"/>, the <see cref="FileInfo"/> property is guaranteed to be non-<see langword="null"/>.
    /// </remarks>
    /// <value>
    /// <see langword="true"/> if the data entry is from a local file; otherwise, <see langword="false"/>.
    /// </value>
    [MemberNotNullWhen(true, nameof(FileInfo))]
    public bool IsLocalFile => FileInfo != null;

    /// <summary>
    /// Gets a value indicating whether the data entry originates from a MEG archive reference.
    /// </summary>
    /// <remarks>
    /// If this property returns <see langword="true"/>, the <see cref="MegFileLocation"/> property is guaranteed to be non-<see langword="null"/>.
    /// </remarks>
    /// <value>
    /// <see langword="true"/> if the data entry is from a referend MEG entry; otherwise, <see langword="false"/>.
    /// </value>
    [MemberNotNullWhen(true, nameof(MegFileLocation))]
    public bool IsEntryReference => MegFileLocation != null;

    /// <summary>
    /// Gets the MEG file's data entry. <see langeword="null"/> if not present.
    /// </summary>
    public MegDataEntryLocationReference? MegFileLocation { get; }

    /// <summary>
    /// Gets the file's path on the file system. <see langeword="null"/> if not present.
    /// </summary>
    public IFileInfo? FileInfo { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegDataEntryOriginInfo"/> structure to the specified file.
    /// </summary>
    /// <param name="fileInfo">The origin file.</param>
    /// <exception cref="ArgumentNullException"><paramref name="fileInfo"/> is <see langword="null"/>.</exception>
    public MegDataEntryOriginInfo(IFileInfo fileInfo)
    {
        FileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
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
        return Equals(MegFileLocation, other.MegFileLocation) && 
               string.Equals(FileInfo?.FullName, other.FileInfo?.FullName, StringComparison.Ordinal);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is MegDataEntryOriginInfo other && Equals(other));
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(MegFileLocation, FileInfo?.FullName);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return IsLocalFile 
            ? $"Local File: '{FileInfo.FullName}'"
            : $"MEG Entry: '{MegFileLocation}'";
    }
}