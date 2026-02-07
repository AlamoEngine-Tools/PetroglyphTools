using System;
using System.IO.Abstractions;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using PG.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Data.EntryLocations;

public class MegDataEntryOriginInfoTest : PGTestBase
{
    [Fact]
    public void Ctor_InvalidArgs_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new MegDataEntryOriginInfo((IFileInfo)null!));
        Assert.Throws<ArgumentNullException>(() => new MegDataEntryOriginInfo((MegDataEntryLocationReference)null!));
    }

    [Fact]
    public void Ctor_FileInfo()
    {
        var fi = FileSystem.FileInfo.New("test.xml");
        var originInfo = new MegDataEntryOriginInfo(fi);

        Assert.Same(fi, originInfo.FileInfo);
        Assert.Null(originInfo.MegFileLocation);

        Assert.True(originInfo.IsLocalFile);
        Assert.False(originInfo.IsEntryReference);
    }

    [Fact]
    public void Ctor_ReferenceLocation()
    {
        using var _ = FileSystem.File.Create("test.meg");
        var meg = new MegFile(new MegArchive([]), new MegFileInformation("test.meg", MegFileVersion.V1),
            ServiceProvider);

        var location = new MegDataEntryLocationReference(meg, MegDataEntryTest.CreateEntry("path"));

        var originInfo = new MegDataEntryOriginInfo(location);

        Assert.Equal(location, originInfo.MegFileLocation);
        Assert.Null(originInfo.FileInfo);

        Assert.True(originInfo.IsEntryReference);
        Assert.False(originInfo.IsLocalFile);
    }

    [Fact]
    public void EqualsHashCode()
    {
        using var _ = FileSystem.File.Create("test.meg");
        var meg = new MegFile(new MegArchive([]), new MegFileInformation("test.meg", MegFileVersion.V1),
            ServiceProvider);

        var location = new MegDataEntryLocationReference(meg, MegDataEntryTest.CreateEntry("path"));
        var otherLocation = new MegDataEntryLocationReference(meg, MegDataEntryTest.CreateEntry("path"));

        var originLoc = new MegDataEntryOriginInfo(location);
        var otherOriginLoc = new MegDataEntryOriginInfo(otherLocation);
        var originPath = new MegDataEntryOriginInfo(FileSystem.FileInfo.New("test.xml"));
        var otherOriginPath = new MegDataEntryOriginInfo(FileSystem.FileInfo.New("test.xml"));


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

        Assert.NotEqual(originPath, new MegDataEntryOriginInfo(FileSystem.FileInfo.New("TEST.XML")));

        Assert.NotEqual(originLoc,
            new MegDataEntryOriginInfo(new MegDataEntryLocationReference(meg, MegDataEntryTest.CreateEntry("PATH"))));

        Assert.NotEqual(originLoc.GetHashCode(), originPath.GetHashCode());

        Assert.Equal(originLoc.GetHashCode(), otherOriginLoc.GetHashCode());
        Assert.Equal(originPath.GetHashCode(), otherOriginPath.GetHashCode());
    }
}