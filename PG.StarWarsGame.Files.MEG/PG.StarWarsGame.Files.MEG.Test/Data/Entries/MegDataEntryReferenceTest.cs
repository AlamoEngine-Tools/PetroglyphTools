using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Test.Data.Entries;

[TestClass]
public class MegDataEntryReferenceTest : MegDataEntryBaseTest<MegDataEntryReferenceLocation>
{
    // We need a instance global variable, cause IMegFile equality is only done by reference.
    private readonly IMegFile _megFile = new Mock<IMegFile>().Object;

    protected override MegDataEntryBase<MegDataEntryReferenceLocation> CreateEntry(string path, Crc32 crc, MegDataEntryReferenceLocation location)
    {
        var newLocation = CreateLocation(path, crc, location);
        return new MegDataEntryReference(newLocation);
    }

    protected MegDataEntryReferenceLocation CreateLocation(string path, Crc32 crc, MegDataEntryReferenceLocation location)
    {
        return new MegDataEntryReferenceLocation(_megFile,
            new MegDataEntry(path, crc, location.DataEntry.Location, false));
    }

    protected override MegDataEntryReferenceLocation CreateLocation(int seed)
    {
        unchecked
        {
            return new MegDataEntryReferenceLocation(_megFile,
                new MegDataEntry("path", DefaultCrc, new MegDataEntryLocation((uint)seed, (uint)seed), false));
        }
    }
}