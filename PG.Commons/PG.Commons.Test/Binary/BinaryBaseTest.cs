using System;
using Moq;
using Moq.Protected;
using PG.Commons.Binary;
using Xunit;

namespace PG.Commons.Test.Binary;

public class BinaryBaseTest
{
    [Fact]
    public void Test_Size()
    {
        var binary = new Mock<BinaryBase>();
        binary.Protected().Setup<int>("GetSizeCore").Returns(99);


        Assert.Equal(99, binary.Object.Size);
        Assert.Equal(99, binary.Object.Size);

        binary.Protected().Verify("GetSizeCore", Times.Once());
    }

    [Fact]
    public void Test_Bytes()
    {
        var expected = new byte[] { 1, 2, 3 };

        var binary = new Mock<BinaryBase>();
        binary.Protected().Setup<byte[]>("ToBytesCore").Returns(expected);

        Assert.Equal(expected, binary.Object.Bytes);

        var bytes = binary.Object.Bytes;
        bytes[0] = 3;
        bytes[1] = 1;
        bytes[2] = 2;

        Assert.Equal(expected, binary.Object.Bytes);

        binary.Protected().Verify("ToBytesCore", Times.Once());
    }

    [Fact]
    public void Test_Bytes_Null_Throws()
    {
        var binary = new Mock<BinaryBase>();
        binary.Protected().Setup<byte[]>("ToBytesCore").Returns((byte[])null!);

        Assert.Throws<InvalidOperationException>(() => binary.Object.Bytes);
    }
}