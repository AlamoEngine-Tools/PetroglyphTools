using System;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
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
        var megFile = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegFileVersion.V1),
            ServiceProvider);
        Assert.Throws<ArgumentNullException>(() => new MegDataEntryLocationReference(megFile, null!));
    }

    [Fact]
    public void Ctor()
    {
        FileSystem.File.Create("file.meg");
        var megFile = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegFileVersion.V1),
            ServiceProvider);
        var entry = MegDataEntryTest.CreateEntry("path");

        var reference = new MegDataEntryLocationReference(megFile, entry);

        Assert.Same(megFile, reference.MegFile);
        Assert.Same(entry, reference.DataEntry);
    }

    [Fact]
    public void Equals_Hashcode()
    {
        FileSystem.File.Create("a.meg");
        FileSystem.File.Create("b.meg");

        var megFileA = new MegFile(new MegArchive([]), new MegFileInformation("a.meg", MegFileVersion.V1),
            ServiceProvider);
        var megFileB = new MegFile(new MegArchive([]), new MegFileInformation("b.meg", MegFileVersion.V1),
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
        var megFile = new MegFile(new MegArchive([entry]), new MegFileInformation("file.meg", MegFileVersion.V1),
            ServiceProvider);
        
        var locationExists = new MegDataEntryLocationReference(megFile, entry);
        var locationNotExists = new MegDataEntryLocationReference(megFile, MegDataEntryTest.CreateEntry("other"));

        Assert.True(locationExists.Exists);
        Assert.False(locationNotExists.Exists);
    }
}