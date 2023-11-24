using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Test.Data.Entries;

[TestClass]
public class MegDataEntryReferenceTest : MegDataEntryBaseTest<MegDataEntryLocationReference>
{
    // We need a instance global variable, cause IMegFile equality is only done by reference.
    private readonly Mock<IMegFile> _megFile = new();

    private MegDataEntryReference CreateEntryReference(string path, Crc32 crc, MegDataEntryLocationReference location)
    {
        var newLocation = CreateLocation(path, crc, location);
        return new MegDataEntryReference(newLocation);
    }

    protected override MegDataEntryBase<MegDataEntryLocationReference> CreateEntry(string path, Crc32 crc, MegDataEntryLocationReference location)
    {
        return CreateEntryReference(path, crc, location);
    }

    private MegDataEntryLocationReference CreateLocation(string path, Crc32 crc, MegDataEntryLocationReference location)
    {
        return new MegDataEntryLocationReference(_megFile.Object,
            new MegDataEntry(path, crc, location.DataEntry.Location, false));
    }

    protected override MegDataEntryLocationReference CreateLocation(int seed)
    {
        unchecked
        {
            return new MegDataEntryLocationReference(_megFile.Object,
                new MegDataEntry("path", DefaultCrc, new MegDataEntryLocation((uint)seed, (uint)seed), false));
        }
    }
}