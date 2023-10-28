using PG.Commons.Services;
using System;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

public sealed class MegFileDataEntry : MegDataEntryBase<MegFileDataEntryLocation>, IEquatable<MegFileDataEntry>
{
    public override string FilePath { get; }

    public override Crc32 FileNameCrc32 { get; }

    /// <summary>
    /// Indicates whether the file is encrypted
    /// </summary>
    public bool Encrypted { get; }

    internal MegFileDataEntry(string filePath, Crc32 crc32, MegFileDataEntryLocation location, bool encrypted) : base(location)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Archived file name must not be null or empty.", nameof(filePath));
        FilePath = filePath;
        FileNameCrc32 = crc32;
        Encrypted = encrypted;
    }

    public bool Equals(MegFileDataEntry? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Encrypted == other.Encrypted && base.Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Encrypted);
    }
}