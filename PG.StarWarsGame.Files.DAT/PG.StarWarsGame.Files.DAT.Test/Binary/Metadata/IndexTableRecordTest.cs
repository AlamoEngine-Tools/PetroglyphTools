using System;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Binary.Metadata;

public class IndexTableRecordTest
{
    [Fact]
    public void Test_Ctor_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new IndexTableRecord(new Crc32(123), uint.MaxValue, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new IndexTableRecord(new Crc32(123), 0, uint.MaxValue));
    }

    [Fact]
    public void Test_Ctor()
    {
        var record = new IndexTableRecord(new Crc32(1), 2,3);
        Assert.Equal(new Crc32(1), record.Crc32);
        Assert.Equal(2u, record.KeyLength);
        Assert.Equal(3u, record.ValueLength);

        Assert.Equal(sizeof(uint) * 3, record.Size);

        var expectedBytes = new byte[]
        {
            0x1, 0x0, 0x0, 0x0,
            0x3, 0x0, 0x0, 0x0,
            0x2, 0x0, 0x0, 0x0
        };
        Assert.Equal(expectedBytes, record.Bytes);
    }

    [Fact]
    public void Ctor_Test_Compare()
    {
        var r0 = new IndexTableRecord(new Crc32(0), 1, 1);
        var r1 = new IndexTableRecord(new Crc32(1), 1, 1);
        var r2 = new IndexTableRecord(new Crc32(1), 2, 2);

        Assert.Equal(0, r0.CompareTo(r0));
        Assert.Equal(-1, r0.CompareTo(r1));
        Assert.Equal(1, r1.CompareTo(r0));

        Assert.Equal(0, r1.CompareTo(r1));
        Assert.Equal(1, r1.CompareTo(r0));
        Assert.Equal(-1, r0.CompareTo(r1));

        Assert.Equal(0, r1.CompareTo(r2));
    }
}