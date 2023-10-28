using PG.Commons.Services;
using System;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

public sealed class MegFileDataEntry : MegDataEntryBase, IEquatable<MegFileDataEntry>
{
    public override string FilePath { get; }

    public override Crc32 FileNameCrc32 { get; }

    public MegFileDataEntryLocation Location { get; }

    /// <summary>
    /// Indicates whether the file is encrypted
    /// </summary>
    public bool Encrypted { get; }

    internal MegFileDataEntry(string filePath, Crc32 crc32, MegFileDataEntryLocation location, bool encrypted)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Archived file name must not be null or empty.", nameof(filePath));
        FilePath = filePath;
        FileNameCrc32 = crc32;
        Location = location;
        Encrypted = encrypted;
    }

    public bool Equals(MegFileDataEntry? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return FilePath == other.FilePath && FileNameCrc32.Equals(other.FileNameCrc32) && Location.Equals(other.Location) && Encrypted == other.Encrypted;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is MegFileDataEntry other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FilePath, FileNameCrc32, Location, Encrypted);
    }
}