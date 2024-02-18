using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;

namespace PG.StarWarsGame.Files.MEG.Test.Data.Entries;

[TestClass]
public class VirtualMegDataEntryReferenceTest_Path : MegDataEntryBaseTest<MegDataEntryOriginInfo>
{
    private VirtualMegDataEntryReference CreateVirtualMegDataEntryReference(string path, Crc32 crc, MegDataEntryOriginInfo location)
    {
        return new VirtualMegDataEntryReference(MegDataEntryTest.CreateEntry(path, crc), location);
    }

    protected override MegDataEntryBase<MegDataEntryOriginInfo> CreateEntry(string path, Crc32 crc, MegDataEntryOriginInfo location)
    {
        return CreateVirtualMegDataEntryReference(path, crc, location);
    }

    protected override MegDataEntryOriginInfo CreateLocation(int seed)
    {
        return new MegDataEntryOriginInfo(seed.ToString());
    }

    [TestMethod]
    public void Test_Equals_HashCode()
    {
        var entry = CreateVirtualMegDataEntryReference("path", DefaultCrc, CreateLocation(1));
        var entryEquals = CreateVirtualMegDataEntryReference("path", DefaultCrc, CreateLocation(1));
        var entryNotEqualsOtherEntry = CreateVirtualMegDataEntryReference("other", DefaultCrc, CreateLocation(1));
        var entryNotEqualsOtherLocation = CreateVirtualMegDataEntryReference("path", DefaultCrc, CreateLocation(99));

        Assert.IsTrue(entry.Equals(entry));
        Assert.IsTrue(entry.Equals((object)entryEquals));
        Assert.IsTrue(entry.Equals(entryEquals));
        Assert.IsTrue(entry.Equals((object)entry));
        Assert.AreEqual(entry.GetHashCode(), entryEquals.GetHashCode());

        Assert.IsFalse(entry.Equals((object?)null));
        Assert.IsFalse(entry.Equals(null));
        Assert.IsFalse(entry.Equals(new object()));
        Assert.IsFalse(entry.Equals(entryNotEqualsOtherLocation));
        Assert.IsFalse(entry.Equals(entryNotEqualsOtherEntry));

        Assert.AreNotEqual(entry.GetHashCode(), entryNotEqualsOtherEntry.GetHashCode());
        Assert.AreNotEqual(entry.GetHashCode(), entryNotEqualsOtherLocation.GetHashCode());
    }
}