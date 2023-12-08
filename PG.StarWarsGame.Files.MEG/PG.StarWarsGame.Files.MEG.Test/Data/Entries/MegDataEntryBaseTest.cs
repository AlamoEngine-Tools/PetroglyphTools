using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;

namespace PG.StarWarsGame.Files.MEG.Test.Data.Entries;

[TestClass]
public class MegDataEntryBaseTest : MegDataEntryBaseTest<MegDataEntryBaseTest.TestLocation>
{
    [TestMethod]
    public void Test_CtorNull()
    {
        Assert.ThrowsException<ArgumentNullException>(() => CreateEntry("path", new Crc32(0), null!));
    }

    protected override MegDataEntryBase<TestLocation> CreateEntry(string path, Crc32 crc, TestLocation location)
    {
        return new TestDataEntry(path, crc, location);
    }

    protected override TestLocation CreateLocation(int seed)
    {
        return new TestLocation();
    }

    private class TestDataEntry : MegDataEntryBase<TestLocation>
    {
        public override string FilePath { get; }
        public override Crc32 Crc32 { get; }

        public TestDataEntry(string path, Crc32 crc32, TestLocation location) : base(location)
        {
            FilePath = path;
            Crc32 = crc32;
        }
    }

    public class TestLocation : IDataEntryLocation
    {
        
    }

}

public abstract class MegDataEntryBaseTest<T> where T : IDataEntryLocation
{
    protected static readonly Crc32 DefaultCrc = new(123);
    protected static readonly Crc32 SecondaryCrc = new(456);

    [TestMethod]
    public void Test_CompareTo()
    {
        var seed = 251;

        var entry1 = CreateEntry("abc", DefaultCrc, CreateLocation(seed++));
        var entry2 = CreateEntry("xyz", DefaultCrc, CreateLocation(seed++));


        Assert.AreEqual(0, entry1.CompareTo(entry1));
        Assert.AreEqual(0, entry1.CompareTo(entry2));

        Assert.AreEqual(1, entry1.CompareTo(null!));

        var entry3 = CreateEntry("path", new Crc32(0), CreateLocation(seed++));
        var entry4 = CreateEntry("path", new Crc32(789), CreateLocation(seed++));
        var entry5 = CreateEntry("path", new Crc32(-1), CreateLocation(seed));

        Assert.AreEqual(-1, entry1.CompareTo(entry4));
        Assert.AreEqual(1, entry1.CompareTo(entry3));
        Assert.AreEqual(1, entry5.CompareTo(entry4));
    }

    [TestMethod]
    public void Test_Equals_HashCode_Base()
    {
        var seed = 251;

        var t1 = CreateLocation(seed++);
        var t2 = CreateLocation(seed++);
        var t3 = CreateLocation(seed);

        var entry1 = CreateEntry("path", DefaultCrc, t1);
        var entry2 = CreateEntry("path", DefaultCrc, t1);

        var entry3 = CreateEntry("path", SecondaryCrc, t1);
        var entry4 = CreateEntry("test", DefaultCrc, t1);
        var entry5 = CreateEntry("path", DefaultCrc, t2);
        var entry6 = CreateEntry("path", DefaultCrc, t3);
        var entry7 = CreateEntry("PATH", DefaultCrc, t1);

        Assert.IsTrue(entry1.Equals(entry2));
        Assert.IsTrue(entry1.Equals((object)entry2));
        Assert.IsTrue(entry1.Equals(entry1));
        Assert.IsTrue(entry1.Equals((object)entry1));
        Assert.AreEqual(entry1.GetHashCode(), entry2.GetHashCode());

        Assert.IsFalse(entry1.Equals((object?)null));
        Assert.IsFalse(entry1.Equals(null));
        Assert.IsFalse(entry1.Equals(new object()));
        Assert.IsFalse(entry1.Equals(entry3));
        Assert.IsFalse(entry1.Equals(entry4));
        Assert.IsFalse(entry1.Equals(entry5));
        Assert.IsFalse(entry1.Equals(entry6));
        Assert.IsFalse(entry1.Equals(entry7));

        Assert.AreNotEqual(entry1.GetHashCode(), entry3.GetHashCode());
        Assert.AreNotEqual(entry1.GetHashCode(), entry4.GetHashCode());
        Assert.AreNotEqual(entry1.GetHashCode(), entry5.GetHashCode());
        Assert.AreNotEqual(entry1.GetHashCode(), entry6.GetHashCode());
        Assert.AreNotEqual(entry1.GetHashCode(), entry7.GetHashCode());
    }

    protected abstract MegDataEntryBase<T> CreateEntry(string path, Crc32 crc, T location);

    protected abstract T CreateLocation(int seed);
}