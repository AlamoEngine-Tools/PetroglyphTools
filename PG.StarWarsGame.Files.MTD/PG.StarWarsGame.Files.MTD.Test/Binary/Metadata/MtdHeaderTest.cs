using System;
using PG.StarWarsGame.Files.MTD.Binary.Metadata;
using Xunit;

namespace PG.StarWarsGame.Files.MTD.Test.Binary.Metadata;

public class MtdHeaderTest
{
    [Fact]
    public void Ctor_InvalidArgs_ThrowsArgumentOORException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MtdHeader((uint)int.MaxValue + 1));
    }
    
    [Fact]
    public void Ctor()
    {
        new MtdHeader(0);
        new MtdHeader(1);
        new MtdHeader(int.MaxValue);
        Assert.True(true);
    }

    [Fact]
    public void Ctor_FileNumber()
    {
        var header = new MtdHeader(123);
        Assert.Equal(123u, header.Count);
    }

    [Fact]
    public void Size()
    {
        Assert.Equal(4, default(MtdHeader).Size);
    }

    [Fact]
    public void Bytes()
    {
        var header = new MtdHeader(2);
        var expectedBytes = new byte[]
        {
            0x2, 0x0, 0x0, 0x0
        };
        Assert.Equal(expectedBytes, header.Bytes);
    }
}