// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.MEG.Data.EntryLocations;

/// <summary>
/// Location of an archived MEG data entry inside a .MEG file. 
/// </summary>
public readonly struct MegDataEntryLocation : IDataEntryLocation, IEquatable<MegDataEntryLocation>
{
    /// <summary>
    /// Gets the offset from the start of the .MEG file archive.
    /// </summary>
    public uint Offset { get; }

    /// <summary>
    /// Gets the file size in bytes.
    /// </summary>
    public uint Size { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegDataEntryLocation"/> structure to a given file offset and file size.
    /// </summary>
    /// <param name="offset">The offset of the file in bytes from the start its .MEG file.</param>
    /// <param name="size">The size of the file in bytes.</param>
    public MegDataEntryLocation(uint offset, uint size)
    {
        Offset = offset;
        Size = size;
    }

    /// <inheritdoc />
    public bool Equals(MegDataEntryLocation other)
    {
        return Offset == other.Offset && Size == other.Size;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is MegDataEntryLocation other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Offset, Size);
    }
}