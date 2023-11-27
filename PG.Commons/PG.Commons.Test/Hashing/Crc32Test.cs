using System;
using System.Buffers.Binary;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Hashing;

namespace PG.Commons.Test.Hashing;

[TestClass]
public class Crc32Test
{
    [TestMethod]
    public unsafe void Test_Default_Behavior()
    {
        var crcEmpty = new Crc32();
        var crcDefault = default(Crc32);

        Assert.AreEqual(sizeof(uint), sizeof(Crc32));

        Assert.IsTrue(crcEmpty == crcDefault);
        Assert.IsTrue(crcEmpty.Equals(crcDefault));

        Assert.IsFalse(crcEmpty.Equals(0));

        Assert.IsTrue((int)crcEmpty == 0);
        Assert.IsTrue((uint)crcEmpty == 0U);

        Assert.AreEqual(crcEmpty.GetHashCode(), crcDefault.GetHashCode());

        CollectionAssert.AreEqual(new byte[4], crcDefault.GetBytes());
        CollectionAssert.AreEqual(new byte[4], crcEmpty.GetBytes());
    }

    [TestMethod]
    public void Test_Construction()
    {
        var crc1u = new Crc32(1U);
        Assert.AreEqual(1, (int)crc1u);

        var crc1i = new Crc32(1);
        Assert.AreEqual(1, (int)crc1i);

        Assert.AreEqual(crc1i, crc1u);

        Span<byte> data = stackalloc byte[sizeof(int)];
        BinaryPrimitives.WriteInt32LittleEndian(data, 1);
        var crcFromSpan = new Crc32(data);

        Assert.AreEqual(1u, (uint)crcFromSpan);
    }

    [TestMethod]
    public void Test_Construction_Negative()
    {
        var crcM1 = new Crc32(-1);
        Assert.AreEqual(-1, (int)crcM1);
        Assert.AreEqual(uint.MaxValue, (uint)crcM1);
    }

    [TestMethod]
    public void Test_Construction_BigEndian()
    {
        Span<byte> data = stackalloc byte[sizeof(int)];
        BinaryPrimitives.WriteInt32BigEndian(data, 1);
        var crcFromSpan = new Crc32(data);

        Assert.AreNotEqual(1, (int)crcFromSpan);
    }

    [TestMethod]
    public void Test_Compare()
    {
        var crc1 = new Crc32(1);
        var crc2 = new Crc32(2);
        var crcM1 = new Crc32(-1);

        Assert.IsTrue(crc1 < crc2);
        Assert.IsFalse(crc1 > crc2);
        Assert.IsTrue(crc2 > crc1);
        Assert.IsFalse(crc2 < crc1);
        Assert.IsTrue(crc2 != crc1);

        Assert.IsTrue(crcM1 > crc2);
        Assert.IsFalse((int)crcM1 > (int)crc2);
    }

    [TestMethod]
    public void Test_ToString()
    {
        var crc1 = new Crc32(1);
        var crcM1 = new Crc32(-1);

        Assert.AreEqual("CRC: 1", crc1.ToString());
        Assert.AreEqual("CRC: 1", crc1.ToString(true));
        Assert.AreEqual($"CRC: {uint.MaxValue}", crcM1.ToString());
        Assert.AreEqual("CRC: -1", crcM1.ToString(true));
    }

    [TestMethod]
    public void Test_GetBytes()
    {
        var crcM1 = new Crc32(-1);
        var crcMax = new Crc32(uint.MaxValue);
        Assert.AreEqual(-1, (int)crcM1);

        var expected = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
        CollectionAssert.AreEqual(expected, crcM1.GetBytes());
        CollectionAssert.AreEqual(expected, crcMax.GetBytes());

        var data = new byte[sizeof(uint)];
        crcM1.GetBytes(data);
        CollectionAssert.AreEqual(expected, data.ToArray());
    }
}