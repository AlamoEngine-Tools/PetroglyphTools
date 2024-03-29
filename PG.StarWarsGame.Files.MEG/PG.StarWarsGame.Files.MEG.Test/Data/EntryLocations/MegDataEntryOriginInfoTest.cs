using System;
using Moq;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Data.EntryLocations;

public class MegDataEntryOriginInfoTest
{
    [Fact]
    public void Test_Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new MegDataEntryOriginInfo((string)null!));
        Assert.Throws<ArgumentNullException>(() => new MegDataEntryOriginInfo((MegDataEntryLocationReference)null!));

        Assert.Throws<ArgumentException>(() => new MegDataEntryOriginInfo(string.Empty));
        Assert.Throws<ArgumentException>(() => new MegDataEntryOriginInfo("    "));
    }

    [Fact]
    public void Test_Ctor_Path()
    {
        var originInfo = new MegDataEntryOriginInfo("path");

        Assert.Equal("path", originInfo.FilePath);
        Assert.Null(originInfo.MegFileLocation);

        Assert.True(originInfo.IsLocalFile);
        Assert.False(originInfo.IsEntryReference);
    }

    [Fact]
    public void Test_Ctor_ReferenceLocation()
    {
        var meg = new Mock<IMegFile>().Object;
        var location = new MegDataEntryLocationReference(meg, MegDataEntryTest.CreateEntry("path"));

        var originInfo = new MegDataEntryOriginInfo(location);

        Assert.Equal(location, originInfo.MegFileLocation);
        Assert.Null(originInfo.FilePath);

        Assert.True(originInfo.IsEntryReference);
        Assert.False(originInfo.IsLocalFile);
    }

    [Fact]
    public void Test_HashCode()
    {
        var meg = new Mock<IMegFile>().Object;
        var location = new MegDataEntryLocationReference(meg, MegDataEntryTest.CreateEntry("path"));
        var otherLocation = new MegDataEntryLocationReference(meg, MegDataEntryTest.CreateEntry("path"));

        var originLoc = new MegDataEntryOriginInfo(location);
        var otherOriginLoc = new MegDataEntryOriginInfo(otherLocation);
        var originPath = new MegDataEntryOriginInfo("path");
        var otherOriginPath = new MegDataEntryOriginInfo("path");

        Assert.NotEqual(originLoc.GetHashCode(), originPath.GetHashCode());

        Assert.Equal(originLoc.GetHashCode(), otherOriginLoc.GetHashCode());
        Assert.Equal(originPath.GetHashCode(), otherOriginPath.GetHashCode());
    }

    [Fact]
    public void Test_Equals()
    {
        var meg = new Mock<IMegFile>().Object;
        var location = new MegDataEntryLocationReference(meg, MegDataEntryTest.CreateEntry("path"));
        var otherLocation = new MegDataEntryLocationReference(meg, MegDataEntryTest.CreateEntry("path"));

        var originLoc = new MegDataEntryOriginInfo(location);
        var otherOriginLoc = new MegDataEntryOriginInfo(otherLocation);
        var originPath = new MegDataEntryOriginInfo("path");
        var otherOriginPath = new MegDataEntryOriginInfo("path");


        Assert.Equal(originLoc, originLoc);
        Assert.Equal(originLoc, (object)originLoc);
        Assert.Equal(originLoc, otherOriginLoc);

        Assert.Equal(originPath, originPath);
        Assert.Equal(originPath, (object)originPath);
        Assert.Equal(originPath, otherOriginPath);

        Assert.False(originLoc.Equals(null));
        Assert.NotEqual((object?)null, originLoc);

        Assert.False(originPath.Equals(null));
        Assert.NotEqual((object?)null, originPath);

        Assert.NotEqual(originPath, originLoc);
        Assert.NotEqual(originPath, (object)originLoc);

        Assert.NotEqual(originPath, new MegDataEntryOriginInfo("PATH"));

        Assert.NotEqual(originLoc,
            new MegDataEntryOriginInfo(new MegDataEntryLocationReference(meg, MegDataEntryTest.CreateEntry("PATH"))));
    }
}