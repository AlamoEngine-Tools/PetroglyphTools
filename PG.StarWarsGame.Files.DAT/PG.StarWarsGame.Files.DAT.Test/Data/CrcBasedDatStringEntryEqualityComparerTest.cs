using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Data;

namespace PG.StarWarsGame.Files.DAT.Test.Data;

[TestClass]
public class CrcBasedDatStringEntryEqualityComparerTest
{
    [TestMethod]
    public void Test_Equals()
    {
        var comparer = CrcBasedDatStringEntryEqualityComparer.Instance;

        Assert.IsTrue(comparer.Equals(new DatStringEntry(), new DatStringEntry()));
        Assert.IsTrue(comparer.Equals(
            new DatStringEntry("1", new Crc32(1), "1", "1"), 
            new DatStringEntry("1", new Crc32(1), "2", "2")));

        Assert.IsTrue(comparer.Equals(
            new DatStringEntry("1", new Crc32(1), "1", "1"),
            new DatStringEntry("2", new Crc32(1), "2", "2")));

        Assert.IsFalse(comparer.Equals(
            new DatStringEntry("1", new Crc32(1), "1", "1"),
            new DatStringEntry("1", new Crc32(2), "1", "1")));
    }

    [TestMethod]
    public void Test_GetHashCode()
    {
        var comparer = CrcBasedDatStringEntryEqualityComparer.Instance;

        Assert.AreEqual(
            comparer.GetHashCode(new DatStringEntry("1", new Crc32(1), "1", "1")),
            comparer.GetHashCode(new DatStringEntry("1", new Crc32(1), "2", "2")));

        Assert.AreEqual(
            comparer.GetHashCode(new DatStringEntry("1", new Crc32(1), "1", "1")),
            comparer.GetHashCode(new DatStringEntry("2", new Crc32(1), "2", "2")));

        Assert.AreNotEqual(
            comparer.GetHashCode(new DatStringEntry("1", new Crc32(1), "1", "1")),
            comparer.GetHashCode(new DatStringEntry("1", new Crc32(2), "2", "2")));
    }
}