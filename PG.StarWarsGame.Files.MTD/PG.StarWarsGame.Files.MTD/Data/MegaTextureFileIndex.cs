// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Drawing;
using AnakinRaW.CommonUtilities;
using PG.Commons.DataTypes;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MTD.Binary;

namespace PG.StarWarsGame.Files.MTD.Data;

/// <summary>
/// Represents a Petroglyph Mega Texture Directory (MTD) entry.
/// </summary>
public sealed class MegaTextureFileIndex : IEquatable<MegaTextureFileIndex>, IHasCrc32
{
    /// <summary>
    /// Gets the ASCII file name of the entry.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Gets the area of the entry within the associated Mega Texture.
    /// </summary>
    public Rectangle Area { get; }

    /// <summary>
    /// Gets a value indicating whether the entry has an alpha channel.
    /// </summary>
    public bool HasAlpha { get; }

    /// <summary>
    /// Gets the CRC32 checksum of the entry.
    /// </summary>
    public Crc32 Crc32 { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegaTextureFileIndex"/> class with the specified information.
    /// </summary>
    /// <param name="fileName">The file name of the entry.</param>
    /// <param name="nameChecksum">The <see cref="PG.Commons.Hashing.Crc32"/> checksum of the file name.</param>
    /// <param name="area">The area of the entry within the Mega Texture.</param>
    /// <param name="hasAlpha">Information whether this entry has an alpha channel.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="fileName"/> is longer than 63 characters.</exception>
    /// <exception cref="ArgumentException"><paramref name="fileName"/> is empty.</exception>
    /// <exception cref="ArgumentException"><paramref name="fileName"/> does contain non-ASCII characters.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <see langword="null"/>.</exception>
    internal MegaTextureFileIndex(string fileName, Crc32 nameChecksum, Rectangle area, bool hasAlpha)
    {
        ThrowHelper.ThrowIfNullOrEmpty(fileName);
        if (fileName.Length > MtdFileConstants.MaxFileNameSize) 
            throw new ArgumentOutOfRangeException(nameof(fileName), "A file name must have at most 63 characters");
        StringUtilities.ValidateIsAsciiOnly(fileName.AsSpan());

        FileName = fileName;
        Area = area;
        HasAlpha = hasAlpha;
        Crc32 = nameChecksum;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;
        if (obj is not MegaTextureFileIndex other)
            return false;
        return Equals(other);
    }

    /// <inheritdoc />
    public bool Equals(MegaTextureFileIndex? other)
    {
        if (ReferenceEquals(this, other)) 
            return true;
        if (other is null)
            return false;
        return FileName == other.FileName && Area.Equals(other.Area) && HasAlpha == other.HasAlpha && Crc32.Equals(other.Crc32);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(FileName, Area, HasAlpha, Crc32);
    }
}