using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Data.EntryLocations;

public class MegDataEntryLocationReferenceTest : CommonMegTestBase
{
    [Fact]
    public void Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new MegDataEntryLocationReference(null!, MegDataEntryTest.CreateEntry("path")));

        FileSystem.File.Create("file.meg");
        var megFile = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegVersion.V1),
            ServiceProvider);
        Assert.Throws<ArgumentNullException>(() => new MegDataEntryLocationReference(megFile, null!));
    }

    [Fact]
    public void Ctor()
    {
        FileSystem.File.Create("file.meg");
        var megFile = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegVersion.V1),
            ServiceProvider);
        var entry = MegDataEntryTest.CreateEntry("path");

        var reference = new MegDataEntryLocationReference(megFile, entry);

        Assert.Same(megFile, reference.Source);
        Assert.Same(entry, reference.DataEntry);
    }

    [Fact]
    public void Equals_Hashcode()
    {
        FileSystem.File.Create("a.meg");
        FileSystem.File.Create("b.meg");

        var megFileA = new MegFile(new MegArchive([]), new MegFileInformation("a.meg", MegVersion.V1),
            ServiceProvider);
        var megFileB = new MegFile(new MegArchive([]), new MegFileInformation("b.meg", MegVersion.V1),
            ServiceProvider);

        var entry = MegDataEntryTest.CreateEntry("path");

        var reference = new MegDataEntryLocationReference(megFileA, entry);

        var otherEqual = new MegDataEntryLocationReference(megFileA, entry);

        var otherNotEqualMeg = new MegDataEntryLocationReference(megFileB, entry);
        var otherNotEqualEntry = new MegDataEntryLocationReference(megFileA, MegDataEntryTest.CreateEntry("other"));

        Assert.Equal(reference, reference);
        Assert.Equal(reference, otherEqual);
        Assert.Equal(reference.GetHashCode(), otherEqual.GetHashCode());

        Assert.False(reference.Equals(null));
        Assert.NotEqual((object?)null, reference);
        Assert.True(reference.Equals(reference));
        Assert.NotEqual(reference, new object());
        Assert.NotEqual(reference, otherNotEqualMeg);
        Assert.NotEqual(reference, otherNotEqualEntry);

        Assert.NotEqual(reference.GetHashCode(), otherNotEqualMeg.GetHashCode());
        Assert.NotEqual(reference.GetHashCode(), otherNotEqualEntry.GetHashCode());
    }

    [Fact]
    public void Exists()
    {
        var entry = MegDataEntryTest.CreateEntry("path");

        FileSystem.File.Create("file.meg");
        var megFile = new MegFile(new MegArchive([entry]), new MegFileInformation("file.meg", MegVersion.V1),
            ServiceProvider);
        
        var locationExists = new MegDataEntryLocationReference(megFile, entry);
        var locationNotExists = new MegDataEntryLocationReference(megFile, MegDataEntryTest.CreateEntry("other"));

        Assert.True(locationExists.Exists);
        Assert.False(locationNotExists.Exists);
    }

    [Fact]
    public void GetData_ReturnsReferencedEntryContent()
    {
        var source = ServiceProvider.GetRequiredService<IMegService>().LoadArchive(MegTestConstants.ContentMegFileV1);

        var reference = new MegDataEntryLocationReference(source, source.Archive[0]);

        using var stream = reference.GetData();
        Assert.Equal(MegTestConstants.CampaignFilesContent, ReadAllBytes(stream));
    }

    [Fact]
    public void GetData_EntryNotInSource_Throws()
    {
        var source = ServiceProvider.GetRequiredService<IMegService>().LoadArchive(MegTestConstants.ContentMegFileV1);

        var reference =
            new MegDataEntryLocationReference(source, MegDataEntryTest.CreateEntry("not/in/archive.xml"));

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