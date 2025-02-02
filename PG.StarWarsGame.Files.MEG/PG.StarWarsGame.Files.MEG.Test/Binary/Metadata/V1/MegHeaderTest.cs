using System;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Metadata.V1;

public class MegHeaderTest
{
    [Fact]
    public void Ctor_InvalidArgs_ThrowsArgumentOORException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MegHeader((uint)int.MaxValue + 1, (uint)int.MaxValue + 1));
        Assert.Throws<ArgumentException>(() => new MegHeader(1, 2));
    }

    [Fact]
    public void Ctor()
    {
        new MegHeader(0, 0);
        new MegHeader(1, 1);
        new MegHeader(int.MaxValue, int.MaxValue);
        Assert.True(true);
    }

    [Fact]
    public void FileNumber()
    {
        IMegHeader header = new MegHeader(1, 1);
        Assert.Equal(1, header.FileNumber);
    }

    [Fact]
    public void Size()
    {
        Assert.Equal(8, default(MegHeader).Size);
    }

    [Fact]
    public void Bytes()
    {
        var header = new MegHeader(2, 2);
        var expectedBytes = new byte[]
        {
            0x2, 0x0, 0x0, 0x0,
            0x2, 0x0, 0x0, 0x0
        };
        Assert.Equal(expectedBytes, header.Bytes);
    }   
    
    [Fact]
    public void GetBytes()
    {
        var header = new MegHeader(2, 2);
        var expectedBytes = new byte[]
        {
            0x2, 0x0, 0x0, 0x0,
            0x2, 0x0, 0x0, 0x0
        };

        Span<byte> buffer = new byte[header.Size];
        buffer.Fill(1);

        header.GetBytes(buffer);

        Assert.Equal(expectedBytes, buffer.Slice(0, header.Size).ToArray());
    }
}