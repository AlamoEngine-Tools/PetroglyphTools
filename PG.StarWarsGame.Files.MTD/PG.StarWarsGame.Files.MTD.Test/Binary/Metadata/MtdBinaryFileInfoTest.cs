using System;
using PG.StarWarsGame.Files.MTD.Binary.Metadata;
using Xunit;

namespace PG.StarWarsGame.Files.MTD.Test.Binary.Metadata;

public class MtdBinaryFileInfoTest
{
    [Fact]
    public void Ctor_Test__ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => new MtdBinaryFileInfo(null!, 1, 2, 3, 4, true));
        Assert.Throws<ArgumentException>(() => new MtdBinaryFileInfo("", 1, 2, 3, 4, true));
        Assert.Throws<ArgumentException>(() => new MtdBinaryFileInfo("öäü", 1, 2, 3, 4, true));
    }

    [Fact]
    public void Ctor_Test__ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MtdBinaryFileInfo(new string('a', 64), 1, 2, 3, 4, true));
        Assert.Throws<ArgumentOutOfRangeException>(() => new MtdBinaryFileInfo("name", (uint)int.MaxValue + 1, 2, 3, 4, true));
        Assert.Throws<ArgumentOutOfRangeException>(() => new MtdBinaryFileInfo("name", 1, (uint)int.MaxValue + 1, 3, 4, true));
        Assert.Throws<ArgumentOutOfRangeException>(() => new MtdBinaryFileInfo("name", 1, 2, (uint)int.MaxValue + 1, 4, true));
        Assert.Throws<ArgumentOutOfRangeException>(() => new MtdBinaryFileInfo("name", 1, 2, 3, (uint)int.MaxValue + 1, true));
    }

    [Fact]
    public void Ctor_Test__Correct()
    {
        var fileInfo = new MtdBinaryFileInfo("name", 1, 2, 3, 4, true);
        Assert.Equal("name", fileInfo.Name);
        Assert.Equal(1, (int)fileInfo.X);
        Assert.Equal(2, (int)fileInfo.Y);
        Assert.Equal(3, (int)fileInfo.Width);
        Assert.Equal(4, (int)fileInfo.Height);
        Assert.True(fileInfo.Alpha);
    }

    [Fact]
    public void Ctor_Test__Bytes_Size_FullNameUsed()
    {
        var fileInfo = new MtdBinaryFileInfo(new string('a', 63), 1, 2, 3, 4, true);

        Assert.Equal(81, fileInfo.Size);

        var bytes = new byte[]
        {
            // Name blob
            0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61,
            0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61,
            0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61,
            0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61,
            0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61,
            0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61, 0x61,
            0x61, 0x61, 0x61, 0x00,
            // X
            0x1, 0x0, 0x0, 0x0,
            // Y
            0x2, 0x0, 0x0, 0x0,
            // Width
            0x3, 0x0, 0x0, 0x0,
            // Height
            0x4, 0x0, 0x0, 0x0,
            // Alpha
            0x1,
        };
        Assert.Equal(bytes, fileInfo.Bytes);
    }

    [Fact]
    public void Ctor_Test__Bytes_Size()
    {
        var fileInfo = new MtdBinaryFileInfo("name", 1, 2, 3, 4, true);

        Assert.Equal(81, fileInfo.Size);

        var bytes = new byte[]
        {
            // Name blob
            0x6e, 0x61, 0x6d, 0x65,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            // X
            0x1, 0x0, 0x0, 0x0,
            // Y
            0x2, 0x0, 0x0, 0x0,
            // Width
            0x3, 0x0, 0x0, 0x0,
            // Height
            0x4, 0x0, 0x0, 0x0,
            // Alpha
            0x1,
        };
        Assert.Equal(bytes, fileInfo.Bytes);
    }
}