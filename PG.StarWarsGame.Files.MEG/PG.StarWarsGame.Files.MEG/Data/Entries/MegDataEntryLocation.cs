using System;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

public readonly struct MegDataEntryLocation : IEquatable<MegDataEntryLocation>
{
    /// <summary>
    /// Gets the offset from the start of the *.MEG file archive.
    /// </summary>
    public uint Offset { get; }

    /// <summary>
    /// Gets the file size in bytes.
    /// </summary>
    public uint Size { get; }

    public MegDataEntryLocation(uint offset, uint size)
    {
        Offset = offset;
        Size = size;
    }

    public bool Equals(MegDataEntryLocation other)
    {
        return Offset == other.Offset && Size == other.Size;
    }

    public override bool Equals(object? obj)
    {
        return obj is MegDataEntryLocation other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Offset, Size);
    }
}