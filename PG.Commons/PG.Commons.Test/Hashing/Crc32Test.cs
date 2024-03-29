using System;
using System.Buffers.Binary;
using System.Linq;
using PG.Commons.Hashing;
using Xunit;

namespace PG.Commons.Test.Hashing;

public class Crc32Test
{
    [Fact]
    public unsafe void Test_Default_Behavior()
    {
        var crcEmpty = new Crc32();
        var crcDefault = default(Crc32);

        Assert.Equal(sizeof(uint), sizeof(Crc32));

        Assert.True(crcEmpty == crcDefault);
        Assert.True(crcEmpty.Equals(crcDefault));

        Assert.False(crcEmpty.Equals(0));

        Assert.True((int)crcEmpty == 0);
        Assert.True((uint)crcEmpty == 0U);

        Assert.Equal(crcEmpty.GetHashCode(), crcDefault.GetHashCode());

        Assert.Equal(new byte[4], crcDefault.GetBytes());
        Assert.Equal(new byte[4], crcEmpty.GetBytes());
    }

    [Fact]
    public void Test_Construction()
    {
        var crc1u = new Crc32(1U);
        Assert.Equal(1, (int)crc1u);

        var crc1i = new Crc32(1);
        Assert.Equal(1, (int)crc1i);

        Assert.Equal(crc1i, crc1u);

        Span<byte> data = stackalloc byte[sizeof(int)];
        BinaryPrimitives.WriteInt32LittleEndian(data, 1);
        var crcFromSpan = new Crc32(data);

        Assert.Equal(1u, (uint)crcFromSpan);
    }

    [Fact]
    public void Test_Construction_Negative()
    {
        var crcM1 = new Crc32(-1);
        Assert.Equal(-1, (int)crcM1);
        Assert.Equal(uint.MaxValue, (uint)crcM1);
    }

    [Fact]
    public void Test_Construction_BigEndian()
    {
        Span<byte> data = stackalloc byte[sizeof(int)];
        BinaryPrimitives.WriteInt32BigEndian(data, 1);
        var crcFromSpan = new Crc32(data);

        Assert.NotEqual(1, (int)crcFromSpan);
    }

    [Fact]
    public void Test_Compare()
    {
        var crc1 = new Crc32(1);
        var crc2 = new Crc32(2);
        var crcM1 = new Crc32(-1);

        Assert.True(crc1 < crc2);
        Assert.True(crc1 <= crc2);
        Assert.False(crc1 > crc2);
        Assert.False(crc1 >= crc2);
        Assert.True(crc2 > crc1);
        Assert.False(crc2 < crc1);
        Assert.True(crc2 != crc1);

        Assert.True(crcM1 > crc2);
        Assert.False((int)crcM1 > (int)crc2);
    }

    [Fact]
    public void Test_ToString()
    {
        var crc1 = new Crc32(1);
        var crcM1 = new Crc32(-1);

        Assert.Equal("CRC: 1", crc1.ToString());
        Assert.Equal("CRC: 1", crc1.ToString(true));
        Assert.Equal($"CRC: {uint.MaxValue}", crcM1.ToString());
        Assert.Equal("CRC: -1", crcM1.ToString(true));
    }

    [Fact]
    public void Test_GetBytes()
    {
        var crcM1 = new Crc32(-1);
        var crcMax = new Crc32(uint.MaxValue);
        Assert.Equal(-1, (int)crcM1);

        var expected = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
        Assert.Equal(expected, crcM1.GetBytes());
        Assert.Equal(expected, crcMax.GetBytes());

        var data = new byte[sizeof(uint)];
        crcM1.GetBytes(data);
        Assert.Equal(expected, data.ToArray());
    }

    [Fact]
    public void Test_Boxing()
    {
        object crc = new Crc32(2);
        Assert.Equal((object)new Crc32(2), (Crc32)crc);
    }
}