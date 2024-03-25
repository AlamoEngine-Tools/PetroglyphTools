using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Data;

namespace PG.StarWarsGame.Files.DAT.Test.Data;

[TestClass]
public class DatStringEntryTest
{
    [TestMethod]
    public void Test_Ctor()
    {
        var entry = new DatStringEntry("abc", new Crc32(1), "valueöäü😊", "def");
        Assert.AreEqual("abc", entry.Key);
        Assert.AreEqual("def", entry.OriginalKey);
        Assert.AreEqual("valueöäü😊", entry.Value);
        Assert.AreEqual(new Crc32(1), entry.Crc32);
    }

    [TestMethod]
    public void Test_Ctor_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new DatStringEntry(null!, new Crc32(1), "value", "def"));

        Assert.ThrowsException<ArgumentNullException>(() =>
            new DatStringEntry("abc", new Crc32(1), null!, "def"));

        Assert.ThrowsException<ArgumentNullException>(() =>
            new DatStringEntry("abc", new Crc32(1), "value", null!));

        Assert.ThrowsException<ArgumentException>(() =>
            new DatStringEntry("öäü", new Crc32(1), "value"));
    }

    [TestMethod]
    public void Test_EqualsHashCode()
    {
        var entry1 = new DatStringEntry("123", new Crc32(1), "abc", "456");
        var entry2 = new DatStringEntry("123", new Crc32(2), "abc", "456");
        var entry3 = new DatStringEntry("123", new Crc32(1), "def", "456");
        var entry4 = new DatStringEntry("123", new Crc32(1), "abc", "789");
        var entry5 = new DatStringEntry("456", new Crc32(1), "abc", "789");

        Assert.IsTrue(entry1.Equals(entry4));
        Assert.AreEqual(entry1.GetHashCode(), entry4.GetHashCode());

        Assert.IsFalse(entry1.Equals(entry2));
        Assert.IsFalse(entry1.Equals(entry3));
        Assert.IsFalse(entry1.Equals(entry5));

        Assert.AreNotEqual(entry1.GetHashCode(), entry2.GetHashCode());
        Assert.AreNotEqual(entry1.GetHashCode(), entry3.GetHashCode());
        Assert.AreNotEqual(entry1.GetHashCode(), entry5.GetHashCode());
    }
}