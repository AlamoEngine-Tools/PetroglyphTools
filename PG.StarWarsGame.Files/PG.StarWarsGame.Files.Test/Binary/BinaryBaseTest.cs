using System;
using Xunit;

namespace PG.StarWarsGame.Files.Test.Binary;

public class BinaryBaseTest
{
    [Fact]
    public void Size()
    {
        var binary = new TestBinary([1, 2, 3, 4, 5, 6]);
        Assert.Equal(6, binary.Size);
    }

    [Fact]
    public void Bytes()
    {
        var expected = new byte[] { 1, 2, 3 };

        var binary = new TestBinary(expected);
        Assert.Equal(expected, binary.Bytes);

        var bytes = binary.Bytes;
        bytes[0] = 3;
        bytes[1] = 1;
        bytes[2] = 2;

        Assert.Equal(expected, binary.Bytes);
    }

    [Fact]
    public void GetBytes()
    {
        var expected = new byte[] { 1, 2, 3 };

        var binary = new TestBinary(expected);

        Span<byte> buffer = new byte[binary.Size + 10];
        buffer.Fill(1);

        binary.GetBytes(buffer);

        Assert.Equal(expected, buffer.Slice(0, binary.Size).ToArray());

        Span<byte> ones = new byte[10];
        ones.Fill(1);
        Assert.Equal(ones.ToArray(), buffer.Slice(binary.Size).ToArray());
    }
}