using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using PG.Commons.Binary;

namespace PG.Commons.Test.Binary;

[TestClass]
public class BinaryBaseTest
{
    [TestMethod]
    public void Test_Size()
    {
        var binary = new Mock<BinaryBase>();
        binary.Protected().Setup<int>("GetSizeCore").Returns(99);


        Assert.AreEqual(99, binary.Object.Size);
        Assert.AreEqual(99, binary.Object.Size);

        binary.Protected().Verify("GetSizeCore", Times.Once());
    }

    [TestMethod]
    public void Test_Bytes()
    {
        var expected = new byte[] { 1, 2, 3 };

        var binary = new Mock<BinaryBase>();
        binary.Protected().Setup<byte[]>("ToBytesCore").Returns(expected);

        CollectionAssert.AreEqual(expected, binary.Object.Bytes);

        var bytes = binary.Object.Bytes;
        bytes[0] = 3;
        bytes[1] = 1;
        bytes[2] = 2;

        CollectionAssert.AreEqual(expected, binary.Object.Bytes);

        binary.Protected().Verify("ToBytesCore", Times.Once());


    }

    [TestMethod]
    public void Test_Bytes_Null_Throws()
    {
        var binary = new Mock<BinaryBase>();
        binary.Protected().Setup<byte[]>("ToBytesCore").Returns((byte[])null!);

        Assert.ThrowsException<InvalidOperationException>(() => binary.Object.Bytes);
    }
}