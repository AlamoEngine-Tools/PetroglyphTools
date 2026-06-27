using System.IO;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Data.Entries;

public class MegDataEntryReferenceTest : MegDataEntryBaseTest<MegDataEntryLocationReference>
{
    private readonly IMegFile _megFile;

    public MegDataEntryReferenceTest()
    {
        FileSystem.File.Create("file.meg");
        _megFile = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegVersion.V1),
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

    [Fact]
    public void GetData_ReturnsReferencedEntryContent()
    {
        var source = ServiceProvider.GetRequiredService<IMegService>().LoadArchive(MegTestConstants.ContentMegFileV1);

        var reference = new MegDataEntryReference(new MegDataEntryLocationReference(source, source.Archive[0]));

        using var stream = reference.GetData();
        Assert.Equal(MegTestConstants.CampaignFilesContent, ReadAllBytes(stream));
    }

    [Fact]
    public void GetData_EntryNotInSource_Throws()
    {
        var source = ServiceProvider.GetRequiredService<IMegService>().LoadArchive(MegTestConstants.ContentMegFileV1);

        var reference = new MegDataEntryReference(
            new MegDataEntryLocationReference(source, MegDataEntryTest.CreateEntry("not/in/archive.xml")));

        Assert.Throws<EntryNotInMegException>(reference.GetData);
    }

    private static byte[] ReadAllBytes(Stream stream)
    {
        using (stream)
        {
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }
}