using System;
using System.Collections.Generic;
using System.Drawing;
using AnakinRaW.CommonUtilities;
using PG.Commons.DataTypes;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MTD.Binary;

namespace PG.StarWarsGame.Files.MTD.Data;

public sealed class MegaTextureFileIndex : IEqualityComparer<MegaTextureFileIndex>, IHasCrc32
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
    public bool Equals(MegaTextureFileIndex x, MegaTextureFileIndex y)
    {
        if (ReferenceEquals(x, y)) 
            return true;
        if (ReferenceEquals(x, null)) 
            return false;
        if (ReferenceEquals(y, null))
            return false;
        if (x.GetType() != y.GetType()) 
            return false;
        return x.FileName == y.FileName && x.Area.Equals(y.Area) && x.HasAlpha == y.HasAlpha && x.Crc32.Equals(y.Crc32);
    }

    /// <inheritdoc />
    public int GetHashCode(MegaTextureFileIndex obj)
    {
        return HashCode.Combine(obj.FileName, obj.Area, obj.HasAlpha, obj.Crc32);
    }
}