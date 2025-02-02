using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Test.Data.Entries;

public class MegDataEntryReferenceTest : MegDataEntryBaseTest<MegDataEntryLocationReference>
{
    private readonly IMegFile _megFile;

    public MegDataEntryReferenceTest()
    {
        FileSystem.File.Create("file.meg");
        _megFile = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegFileVersion.V1),
            ServiceProvider);
    }
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
        return new MegDataEntryLocationReference(_megFile,
            MegDataEntryTest.CreateEntry(path, crc, location.DataEntry.Location, false, null));
    }

    protected override MegDataEntryLocationReference CreateLocation(int seed)
    {
        unchecked
        {
            return new MegDataEntryLocationReference(_megFile, 
                MegDataEntryTest.CreateEntry("path", DefaultCrc, (uint)seed, (uint)seed));
        }
    }
}