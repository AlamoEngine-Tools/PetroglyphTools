using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Data.Entries;

public class VirtualMegDataEntryReferenceTest_ReferenceLocation : MegDataEntryBaseTest<MegDataEntryOriginInfo>
{
    private readonly IMegFile _megFile;

    public VirtualMegDataEntryReferenceTest_ReferenceLocation()
    {
        FileSystem.File.Create("file.meg");
        _megFile = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegFileVersion.V1),
            ServiceProvider);
    }


    private MegDataEntryBase<MegDataEntryOriginInfo> CreateVirtualMegDataEntryReference(string path, Crc32 crc, MegDataEntryOriginInfo location)
    {
        return new VirtualMegDataEntryReference(MegDataEntryTest.CreateEntry(path, crc), location);
    }

    protected override MegDataEntryBase<MegDataEntryOriginInfo> CreateEntry(string path, Crc32 crc, MegDataEntryOriginInfo location)
    {
        return CreateVirtualMegDataEntryReference(path, crc, location);
    }

    protected override MegDataEntryOriginInfo CreateLocation(int seed)
    {
        return new MegDataEntryOriginInfo(new MegDataEntryLocationReference(_megFile,
            MegDataEntryTest.CreateEntry(seed.ToString(), DefaultCrc, (uint)seed, (uint)seed)));
    }

    [Fact]
    public void Equals_HashCode()
    {
        var entry = CreateVirtualMegDataEntryReference("path", DefaultCrc, CreateLocation(1));
        var entryEquals = CreateVirtualMegDataEntryReference("path", DefaultCrc, CreateLocation(1));
        var entryNotEqualsOtherEntry = CreateVirtualMegDataEntryReference("other", DefaultCrc, CreateLocation(1));
        var entryNotEqualsOtherLocation = CreateVirtualMegDataEntryReference("path", DefaultCrc, CreateLocation(99));

        Assert.True(entry.Equals(entry));
        Assert.True(entry.Equals((object)entryEquals));
        Assert.True(entry.Equals(entryEquals));
        Assert.True(entry.Equals((object)entry));
        Assert.Equal(entry.GetHashCode(), entryEquals.GetHashCode());

        Assert.False(entry.Equals((object?)null));
        Assert.False(entry.Equals(null));
        Assert.False(entry.Equals(new object()));
        Assert.False(entry.Equals(entryNotEqualsOtherLocation));
        Assert.False(entry.Equals(entryNotEqualsOtherEntry));

        Assert.NotEqual(entry.GetHashCode(), entryNotEqualsOtherEntry.GetHashCode());
        Assert.NotEqual(entry.GetHashCode(), entryNotEqualsOtherLocation.GetHashCode());
    }
}