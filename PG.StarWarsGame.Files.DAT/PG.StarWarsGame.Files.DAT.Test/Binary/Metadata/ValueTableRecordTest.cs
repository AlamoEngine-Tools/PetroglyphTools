using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;

namespace PG.StarWarsGame.Files.DAT.Test.Binary.Metadata;

[TestClass]
public class ValueTableRecordTest
{
    [TestMethod]
    public void Test_Ctor_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new ValueTableRecord(null!));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("test")]
    [DataRow("testöäü")]
    [DataRow("test\\r\\n")]
    [DataRow("👌")]
    [DataRow("\u00A0")]
    public void Test_Ctor(string input)
    {
        var record = new ValueTableRecord(input);
        Assert.AreEqual(input, record.Value);
        Assert.AreEqual(input.Length * 2, record.Size); // Value has unicode which is two times the char length.
        CollectionAssert.AreEqual(Encoding.Unicode.GetBytes(record.Value), record.Bytes);
    }
}