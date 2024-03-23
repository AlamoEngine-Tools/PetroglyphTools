using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;

namespace PG.StarWarsGame.Files.DAT.Test.Binary.Metadata;

[TestClass]
public class IndexTableRecordTest
{
    [TestMethod]
    public void Test_Ctor_ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new IndexTableRecord(new Crc32(123), uint.MaxValue, 0));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new IndexTableRecord(new Crc32(123), 0, uint.MaxValue));
    }

    [TestMethod]
    public void Test_Ctor()
    {
        var record = new IndexTableRecord(new Crc32(1), 2,3);
        Assert.AreEqual(new Crc32(1), record.Crc32);
        Assert.AreEqual(2u, record.KeyLength);
        Assert.AreEqual(3u, record.ValueLength);

        Assert.AreEqual(sizeof(uint) * 3, record.Size);

        var expectedBytes = new byte[]
        {
            0x1, 0x0, 0x0, 0x0,
            0x3, 0x0, 0x0, 0x0,
            0x2, 0x0, 0x0, 0x0
        };
        CollectionAssert.AreEqual(expectedBytes, record.Bytes);
    }

    [TestMethod]
    public void Ctor_Test_Compare()
    {
        var r0 = new IndexTableRecord(new Crc32(0), 1, 1);
        var r1 = new IndexTableRecord(new Crc32(1), 1, 1);
        var r2 = new IndexTableRecord(new Crc32(1), 2, 2);

        Assert.AreEqual(0, r0.CompareTo(r0));
        Assert.AreEqual(-1, r0.CompareTo(r1));
        Assert.AreEqual(1, r1.CompareTo(r0));

        Assert.AreEqual(0, r1.CompareTo(r1));
        Assert.AreEqual(1, r1.CompareTo(r0));
        Assert.AreEqual(-1, r0.CompareTo(r1));

        Assert.AreEqual(0, r1.CompareTo(r2));
    }


}