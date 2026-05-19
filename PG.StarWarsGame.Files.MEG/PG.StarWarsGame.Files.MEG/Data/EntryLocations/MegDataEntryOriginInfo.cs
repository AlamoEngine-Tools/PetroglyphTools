// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

namespace PG.StarWarsGame.Files.MEG.Data.EntryLocations;

/// <summary>
/// The origin of a MEG data entry which is either packed in a MEG archive, present on the file system, or backed by an in-memory byte buffer.
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
    /// Gets a value indicating whether the data entry originates from an in-memory byte buffer.
    /// </summary>
    /// <remarks>
    /// If this property returns <see langword="true"/>, the <see cref="Bytes"/> property is guaranteed to be non-<see langword="null"/>.
    /// </remarks>
    [MemberNotNullWhen(true, nameof(Bytes))]
    public bool IsBytes => Bytes != null;

    /// <summary>
    /// Gets the MEG file's data entry. <see langeword="null"/> if not present.
    /// </summary>
    public MegDataEntryLocationReference? MegFileLocation { get; }

    /// <summary>
    /// Gets the file's path on the file system. <see langeword="null"/> if not present.
    /// </summary>
    public IFileInfo? FileInfo { get; }

    /// <summary>
    /// Gets the in-memory byte buffer that holds this data entry's bytes. <see langword="null"/> if not present.
    /// </summary>
    /// <remarks>
    /// This is a defensive copy of the buffer passed to the constructor. Mutations to the constructor's
    /// input after construction do not affect this property.
    /// </remarks>
    public byte[]? Bytes { get; }

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

    /// <summary>
    /// Initializes a new instance of the <see cref="MegDataEntryOriginInfo"/> structure backed by the specified byte buffer.
    /// The buffer is copied; subsequent mutations to <paramref name="bytes"/> do not affect this origin.
    /// </summary>
    /// <param name="bytes">The buffer containing the entry bytes.</param>
    /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is <see langword="null"/>.</exception>
    public MegDataEntryOriginInfo(byte[] bytes)
    {
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));
        Bytes = (byte[])bytes.Clone();
    }

    /// <inheritdoc/>
    public bool Equals(MegDataEntryOriginInfo? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Equals(MegFileLocation, other.MegFileLocation) &&
               string.Equals(FileInfo?.FullName, other.FileInfo?.FullName, StringComparison.Ordinal) &&
               ReferenceEquals(Bytes, other.Bytes);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || (obj is MegDataEntryOriginInfo other && Equals(other));
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(MegFileLocation, FileInfo?.FullName, Bytes);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        if (IsLocalFile)
            return $"Local File: '{FileInfo.FullName}'";
        if (IsEntryReference)
            return $"MEG Entry: '{MegFileLocation}'";
        return $"Bytes: {Bytes!.Length} bytes";
    }
}
