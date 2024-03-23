using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;

namespace PG.StarWarsGame.Files.DAT.Test.Binary.Metadata;

[TestClass]
public class DatHeaderTest
{
    [TestMethod]
    [DataRow(0u)]
    [DataRow(1u)]
    [DataRow(100u)]
    public void Test_Ctor(uint number)
    {
        var header = new DatHeader(number);
        Assert.AreEqual(number, header.RecordCount);
        Assert.AreEqual(sizeof(uint), header.Size);
        CollectionAssert.AreEqual(BitConverter.GetBytes(header.RecordCount), header.Bytes);
    }

    [TestMethod]
    public void Test_Ctor()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new DatHeader((uint)int.MaxValue + 1));
    }
}