using System;
using System.Drawing;
using AnakinRaW.CommonUtilities;
using PG.Commons.DataTypes;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MTD.Binary;

namespace PG.StarWarsGame.Files.MTD.Data;

public sealed class MegaTextureFileIndex : IEquatable<MegaTextureFileIndex>, IHasCrc32
{
    public string FileName { get; }

    public Rectangle Area { get; }

    public bool HasAlpha { get; }

    public Crc32 Crc32 { get; }

    public MegaTextureFileIndex(string fileName, Crc32 nameChecksum, Rectangle area, bool hasAlpha)
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