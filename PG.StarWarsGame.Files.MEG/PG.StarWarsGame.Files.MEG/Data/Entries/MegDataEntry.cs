using PG.Commons.Services;
using System;
using PG.StarWarsGame.Files.MEG.Data.Archives;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

/// <remarks>
/// This data entry is unaware of its owning MEG file. This class is used for the <see cref="IMegArchive"/>.
/// </remarks>
/// <inheritdoc cref="IMegDataEntry"/>
public sealed class MegDataEntry : MegDataEntryBase<MegDataEntryLocation>, IEquatable<MegDataEntry>
{
    /// <inheritdoc />
    public override string FilePath { get; }

    /// <inheritdoc />
    public override Crc32 FileNameCrc32 { get; }

    /// <summary>
    /// Indicates whether the file is encrypted
    /// </summary>
    public bool Encrypted { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegDataEntry"/> class.
    /// </summary>
    /// <param name="filePath">The file path of the entry.</param>
    /// <param name="crc32">The CRC32 checksum of the filePath</param>
    /// <param name="location">The location information of the entry inside it's MEG file.</param>
    /// <param name="encrypted">Indicator </param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    internal MegDataEntry(string filePath, Crc32 crc32, MegDataEntryLocation location, bool encrypted) : base(location)
    {
        if (filePath is null)
            throw new ArgumentNullException(nameof(filePath));
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Archived file name must not be null or empty.", nameof(filePath));

        // Note: We cannot validate correct CRC32 here in order to stay compatible with MIKE.NL's tool.
        // See the ASCII vs. Latin1 encoding problem.
        FilePath = filePath;
        FileNameCrc32 = crc32;
        Encrypted = encrypted;
    }

    /// <inheritdoc />
    public bool Equals(MegDataEntry? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Encrypted == other.Encrypted && base.Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Encrypted);
    }
}