using System;
using Moq;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Data.EntryLocations;

public class MegDataEntryLocationReferenceTest
{
    [Fact]
    public void Test_Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new MegDataEntryLocationReference(null!, MegDataEntryTest.CreateEntry("path")));

        Assert.Throws<ArgumentNullException>(() =>
            new MegDataEntryLocationReference(new Mock<IMegFile>().Object, null!));
    }

    [Fact]
    public void Test_Ctor()
    {
        var meg = new Mock<IMegFile>().Object;
        var entry = MegDataEntryTest.CreateEntry("path");

        var reference = new MegDataEntryLocationReference(meg, entry);

        Assert.Same(meg, reference.MegFile);
        Assert.Same(entry, reference.DataEntry);
    }

    [Fact]
    public void Test_Equals_Hashcode()
    {
        var meg = new Mock<IMegFile>().Object;
        var entry = MegDataEntryTest.CreateEntry("path");

        var reference = new MegDataEntryLocationReference(meg, entry);

        var otherEqual = new MegDataEntryLocationReference(meg, entry);

        var otherNotEqualMeg = new MegDataEntryLocationReference(new Mock<IMegFile>().Object, entry);
        var otherNotEqualEntry = new MegDataEntryLocationReference(meg, MegDataEntryTest.CreateEntry("other"));

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
    public void Test_Exists()
    {
        var entry = MegDataEntryTest.CreateEntry("path");
        var archive = new Mock<IMegArchive>();
        archive.Setup(a => a.Contains(entry)).Returns(true);
        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(archive.Object);

        var locationExists = new MegDataEntryLocationReference(meg.Object, entry);
        var locationNotExists = new MegDataEntryLocationReference(meg.Object, MegDataEntryTest.CreateEntry("other"));

        Assert.True(locationExists.Exists);
        Assert.False(locationNotExists.Exists);
    }
}