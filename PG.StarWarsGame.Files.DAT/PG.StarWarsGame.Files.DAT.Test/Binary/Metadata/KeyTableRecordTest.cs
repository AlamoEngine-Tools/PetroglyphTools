using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;

namespace PG.StarWarsGame.Files.DAT.Test.Binary.Metadata;

[TestClass]
public class KeyTableRecordTest
{
    [TestMethod]
    public void Test_Ctor_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new KeyTableRecord(null!));
    }

    [TestMethod]
    [DataRow("testöäü")]
    [DataRow("👌")]
    [DataRow("\u00A0")]
    public void Test_Ctor_NotAscii_Throws(string input)
    {
        Assert.ThrowsException<ArgumentException>(() => new KeyTableRecord(input));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("test")]
    [DataRow("test\tTAB")]
    [DataRow("test\\r\\n")]
    public void Test_Ctor(string input)
    {
        var record = new KeyTableRecord(input);
        Assert.AreEqual(input, record.Key);
        Assert.AreEqual(input.Length, record.Size); // Value has unicode which is two times the char length.
        CollectionAssert.AreEqual(Encoding.ASCII.GetBytes(record.Key), record.Bytes);
    }
}