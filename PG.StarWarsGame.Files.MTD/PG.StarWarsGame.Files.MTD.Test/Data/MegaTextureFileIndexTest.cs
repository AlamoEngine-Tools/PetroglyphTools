using System;
using System.Drawing;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MTD.Data;
using Xunit;

namespace PG.StarWarsGame.Files.MTD.Test.Data;

public class MegaTextureFileIndexTest
{
    [Fact]
    public void Ctor_InvalidDataThrows()
    {
        Assert.Throws<ArgumentNullException>(() => new MegaTextureFileIndex(null!, default, Rectangle.Empty, false));
        Assert.Throws<ArgumentException>(() => new MegaTextureFileIndex("", default, Rectangle.Empty, false));
        Assert.Throws<ArgumentException>(() => new MegaTextureFileIndex("notAsciiöäü", default, Rectangle.Empty, false));
        Assert.Throws<ArgumentOutOfRangeException>(() => new MegaTextureFileIndex(new string('a', 64),default, Rectangle.Empty, false));
    }

    [Fact]
    public void Ctor_SetupData()
    {
        var entry = new MegaTextureFileIndex("fileName", new Crc32(123), new Rectangle(1, 2, 3, 4), true);
        Assert.Equal("fileName", entry.FileName);
        Assert.Equal(new Crc32(123), entry.Crc32);
        Assert.Equal(new Rectangle(1,2,3,4), entry.Area);
        Assert.True(entry.HasAlpha);
    }

    [Fact]
    public void Equals_HashCode()
    {
        var entry1 = new MegaTextureFileIndex("fileName", new Crc32(123), new Rectangle(1, 2, 3, 4), true);

        Assert.True(entry1.Equals(entry1));
        Assert.True(entry1.Equals((object)entry1));

        Assert.False(entry1.Equals((object)null!));
        Assert.False(entry1.Equals(null!));

        var entry2 = new MegaTextureFileIndex("fileName", new Crc32(123), new Rectangle(1, 2, 3, 4), true);
        Assert.True(entry1.Equals(entry2));
        Assert.True(entry1.Equals((object)entry2));
        Assert.Equal(entry1.GetHashCode(), entry2.GetHashCode());

        var entryOtherName = new MegaTextureFileIndex("other", new Crc32(123), new Rectangle(1, 2, 3, 4), true);
        Assert.False(entry1.Equals((object)entryOtherName));
        Assert.False(entry1.Equals(entryOtherName));

        var entryOtherCrc = new MegaTextureFileIndex("fileName", new Crc32(321), new Rectangle(1, 2, 3, 4), true);
        Assert.False(entry1.Equals((object)entryOtherCrc));
        Assert.False(entry1.Equals(entryOtherCrc));

        var entryOtherArea = new MegaTextureFileIndex("fileName", new Crc32(123), new Rectangle(4, 3, 2, 1), true);
        Assert.False(entry1.Equals((object)entryOtherArea));
        Assert.False(entry1.Equals(entryOtherArea));

        var entryOtherAlpha = new MegaTextureFileIndex("fileName", new Crc32(123), new Rectangle(1, 2, 3, 4), false);
        Assert.False(entry1.Equals((object)entryOtherAlpha));
        Assert.False(entry1.Equals(entryOtherAlpha));
    }
}