// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Services;

namespace PG.StarWarsGame.Files.MEG.Data;

/// <summary>
/// Represents an archived file inside a MEG archive.
/// </summary>
/// <remarks>
/// Note that <see cref="Offset"/> is based on the actual target MEG file, this instance was create from.
/// </remarks>
public sealed class MegDataEntry : IEquatable<MegDataEntry>, IComparable<MegDataEntry>
{
    /// <summary>
    /// Gets the relative file path as defined in the *.MEG file.<br />
    /// Usually this file path is relative to the game or mod's DATA directory, e.g. Data/My/file.xml
    /// </summary>
    /// <remarks>
    /// Path operators such as ./ or ../ are permitted. It's the application's responsibility to resolve paths and
    /// deal with potential dangerous file paths.
    /// </remarks>
    public string FilePath { get; }

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
    /// Indicates whether the file is encrypted
    /// </summary>
    public bool Encrypted { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegDataEntry"/> class.
    /// </summary>
    /// <param name="crc">The <see cref="Crc32"/> of the file name.</param>
    /// <param name="filePath">The file name.</param>
    /// <param name="offset">The file's data offset in the meg file.</param>
    /// <param name="size">The file's data size in bytes</param>
    internal MegDataEntry(Crc32 crc, string filePath, uint offset, uint size)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Archived file name must not be null or empty.", nameof(filePath));
        FileNameCrc32 = crc;
        FilePath = filePath;
        Offset = offset;
        Size = size;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <b>Note:</b>
    /// In contrast to comparison, equality is checked on all properties. Path equality is ordinal and case-sensitive
    /// </remarks>
    public bool Equals(MegDataEntry? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other))
            return true;

        // Equality for path needs to binary and case sensitive, cause CRC32 is a checksum of the path and thus changes if any bit is different.
        return FileNameCrc32 == other.FileNameCrc32 && FilePath.Equals(other.FilePath, StringComparison.Ordinal) && Offset == other.Offset && Size == other.Size;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is MegDataEntry other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(FileNameCrc32, FilePath, Offset, Size);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <b>Note:</b>
    /// In contrast to equality, comparison is only based on the <see cref="FileNameCrc32"/> checksum.
    /// </remarks>
    public int CompareTo(MegDataEntry? other)
    {
        // IMPORTANT: Changing the logic here also requires to update the binary models!
        if (ReferenceEquals(this, other)) 
            return 0;
        if (ReferenceEquals(null, other)) 
            return 1;
        return FileNameCrc32.CompareTo(other.FileNameCrc32);
    }
}
