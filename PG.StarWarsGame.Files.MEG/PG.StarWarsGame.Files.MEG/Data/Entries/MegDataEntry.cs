using System;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using AnakinRaW.CommonUtilities;

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
    public override Crc32 Crc32 { get; }

    /// <summary>
    /// The original file path value without the default MEG encoding applied.
    /// </summary>
    public string OriginalFilePath { get; }

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
    /// <param name="encrypted">Indicates whether this entry is encrypted or not.</param>
    /// <param name="originalFilePath">The original file path value.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    internal MegDataEntry(string filePath, Crc32 crc32, MegDataEntryLocation location, bool encrypted, string originalFilePath) : base(location)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(filePath);
        ThrowHelper.ThrowIfNullOrEmpty(originalFilePath);

        StringUtilities.ValidateIsAsciiOnly(filePath.AsSpan());
        
        Crc32 = crc32;
        Encrypted = encrypted;
        OriginalFilePath = originalFilePath;

        // Note: We cannot validate correct CRC32 here in order to stay compatible with MIKE.NL's tool.
        // See the ASCII vs. Latin1 encoding problem.
        FilePath = filePath;
    }

    /// <inheritdoc />
    public bool Equals(MegDataEntry? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Encrypted == other.Encrypted && base.Equals(other);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj is not MegDataEntry other)
            return false;
        return Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Encrypted);
    }
}