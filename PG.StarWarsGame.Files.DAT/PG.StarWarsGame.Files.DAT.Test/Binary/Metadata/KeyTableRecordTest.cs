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
        Assert.ThrowsException<ArgumentNullException>(() => new KeyTableRecord(null!, ""));
        Assert.ThrowsException<ArgumentNullException>(() => new KeyTableRecord("", null!));
    }

    [TestMethod]
    [DataRow("testöäü")]
    [DataRow("👌")]
    [DataRow("\u00A0")]
    public void Test_Ctor_NotAscii_Throws(string input)
    {
        Assert.ThrowsException<ArgumentException>(() => new KeyTableRecord(input, input));
    }

    [TestMethod]
    [DataRow("", "randomÖÄÜ")]
    [DataRow("   ", "randomÖÄÜ")]
    [DataRow("test", "randomÖÄÜ")]
    [DataRow("test\tTAB", "randomÖÄÜ")]
    [DataRow("test\\r\\n", "randomÖÄÜ")]
    public void Test_Ctor(string input, string original)
    {
        var record = new KeyTableRecord(input, original);
        Assert.AreEqual(input, record.Key);
        Assert.AreEqual(original, record.OriginalKey);
        Assert.AreEqual(input.Length, record.Size); // Value has unicode which is two times the char length.
        CollectionAssert.AreEqual(Encoding.ASCII.GetBytes(record.Key), record.Bytes);
    }
}