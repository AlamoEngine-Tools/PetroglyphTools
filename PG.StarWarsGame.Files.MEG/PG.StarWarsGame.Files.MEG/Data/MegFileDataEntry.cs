// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Hashing;
using PG.Commons.Utilities;

namespace PG.StarWarsGame.Files.MEG.Data;

/// <summary>
/// Represents a file packaged in a *.MEG file.
/// </summary>
public sealed class MegFileDataEntry : IEquatable<MegFileDataEntry>
{
    /// <summary>
    /// Gets the relative file path as defined in the *.MEG file.<br />
    /// Usually this file path is relative to the game or mod's DATA directory, e.g. Data/My/file.xml
    /// </summary>
    public string RelativeFilePath { get; }

    /// <summary>
    /// Gets the <see cref="Crc32"/> of the file name.
    /// </summary>
    public Crc32 FileNameCrc32 { get; }

    /// <summary>
    /// Gets the offset from the start of the *.MEG file archive.
    /// </summary>
    public uint Offset { get; }

    /// <summary>
    /// Gets the file size in bytes.
    /// </summary>
    public uint Size { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegFileDataEntry"/> class.
    /// </summary>
    /// <param name="crc">The <see cref="Crc32"/> of the file name.</param>
    /// <param name="relativeFilePath">The file name.</param>
    /// <param name="offset">The file's data offset in the meg file.</param>
    /// <param name="size">The file's data size in bytes</param>
    internal MegFileDataEntry(Crc32 crc, string relativeFilePath, uint offset, uint size)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(relativeFilePath);
        FileNameCrc32 = crc;
        RelativeFilePath = relativeFilePath;
        Offset = offset;
        Size = size;
    }

    /// <inheritdoc/>
    public bool Equals(MegFileDataEntry? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return FileNameCrc32.Equals(other.FileNameCrc32) && Offset == other.Offset && Size == other.Size;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is MegFileDataEntry other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(FileNameCrc32, Offset, Size);
    }
}
