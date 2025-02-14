﻿using System;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Data;

public class MegFileDataEntryBuilderInfoTest : CommonMegTestBase
{
    [Fact]
    public void Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new MegFileDataEntryBuilderInfo(null!));
    }

    #region Ctor_LocalFile

    [Fact]
    public void Ctor_OriginIsLocalFile()
    {
        var origin = new MegDataEntryOriginInfo("path");
        var info = new MegFileDataEntryBuilderInfo(origin);

        Assert.Same(origin, info.OriginInfo);
        Assert.Equal("path", info.FilePath);
        Assert.Null(info.Size);
        Assert.False(info.Encrypted);
    }

    [Fact]
    public void Ctor_OriginIsLocalFile_OverridesProperties()
    {
        var origin = new MegDataEntryOriginInfo("path");
        var info = new MegFileDataEntryBuilderInfo(origin, "PATH", 123, true);

        Assert.Same(origin, info.OriginInfo);
        Assert.Equal("PATH", info.FilePath);
        Assert.Equal(123u, info.Size);
        Assert.True(info.Encrypted);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    public void Ctor_OriginIsLocalFile_OverridesProperties_PathEmpty_Throws(string path)
    {
        var origin = new MegDataEntryOriginInfo("path");
        Assert.Throws<ArgumentException>(() => new MegFileDataEntryBuilderInfo(origin, path, 123, true));
    }

    #endregion

    #region Ctor_Entry

    [Fact]
    public void Ctor_OriginIsEntryReference()
    {
        FileSystem.File.Create("file.meg");
        var meg = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);
        var origin = new MegDataEntryOriginInfo(new MegDataEntryLocationReference(
            meg, MegDataEntryTest.CreateEntry("path", default, 123, 321, true)));

        var info = new MegFileDataEntryBuilderInfo(origin);

        Assert.Same(origin, info.OriginInfo);
        Assert.Equal("path", info.FilePath);
        Assert.Equal(321u, info.Size);
        Assert.Equal(123u, info.OriginInfo.MegFileLocation!.DataEntry.Location.Offset);
        Assert.True(info.Encrypted);
    }

    [Fact]
    public void Ctor_OriginIsEntryReference_OverridesProperties()
    {
        FileSystem.File.Create("file.meg");
        var meg = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);
        var origin = new MegDataEntryOriginInfo(new MegDataEntryLocationReference(
            meg, MegDataEntryTest.CreateEntry("path", default, 123, 321, true)));

        var info = new MegFileDataEntryBuilderInfo(origin, "PATH", 999, false);

        Assert.Same(origin, info.OriginInfo);
        Assert.Equal("PATH", info.FilePath);
        Assert.Equal(321u, info.Size); // Value 999 must be ignored
        Assert.False(info.Encrypted);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    public void Ctor_OriginIsEntryReference_OverridesProperties_PathEmpty_Throws(string path)
    {
        FileSystem.File.Create("file.meg");
        var meg = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);
        var origin = new MegDataEntryOriginInfo(new MegDataEntryLocationReference(
            meg, MegDataEntryTest.CreateEntry("path", default, 123, 321, true)));
        Assert.Throws<ArgumentException>(() => new MegFileDataEntryBuilderInfo(origin, path, 123, true));
    }

    #endregion

    #region Factory_LocalFile

    [Fact]
    public void Factory_FromFile()
    {
        var info = MegFileDataEntryBuilderInfo.FromFile("path", null);
        Assert.True(info.OriginInfo.IsLocalFile);
        Assert.Equal("path", info.FilePath);
        Assert.Null(info.Size);
        Assert.False(info.Encrypted);
    }

    [Fact]
    public void Factory_FromFile_OverridesProperties()
    {
        var info = MegFileDataEntryBuilderInfo.FromFile("path", "123", 123, true);
        Assert.True(info.OriginInfo.IsLocalFile);
        Assert.Equal("123", info.FilePath);
        Assert.Equal(123u, info.Size);
        Assert.True(info.Encrypted);
    }

    [Theory]
    [InlineData("", null)]
    [InlineData("   ", null)]
    [InlineData("test", "")]
    [InlineData("test", "   ")]
    public void Factory_FromFile_Throws(string path, string? overridePath)
    {
        Assert.Throws<ArgumentException>(() => MegFileDataEntryBuilderInfo.FromFile(path, overridePath));
    }

    [Fact]
    public void Factory_FromFile_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => MegFileDataEntryBuilderInfo.FromFile(null!, "random"));
    }

    #endregion

    #region Factory_Entry

    [Fact]
    public void Factory_OriginIsLocalFile()
    {
        FileSystem.File.Create("file.meg");
        var meg = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);
        var entry = MegDataEntryTest.CreateEntry("path", default, 123, 321, true);

        var info = MegFileDataEntryBuilderInfo.FromEntry(meg, entry);

        Assert.True(info.OriginInfo.IsEntryReference);
        Assert.Equal("path", info.FilePath);
        Assert.Equal(321u, info.Size);
        Assert.Equal(123u, info.OriginInfo.MegFileLocation!.DataEntry.Location.Offset);
        Assert.True(info.Encrypted);
    }

    [Fact]
    public void Factory_OriginIsEntryReference_OverridesProperties()
    {
        FileSystem.File.Create("file.meg");
        var meg = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);
        var entry = MegDataEntryTest.CreateEntry("path", default, 123, 321, true);

        var info = MegFileDataEntryBuilderInfo.FromEntry(meg, entry, "PATH", false);

        Assert.True(info.OriginInfo.IsEntryReference);
        Assert.Equal("PATH", info.FilePath);
        Assert.Equal(321u, info.Size); // Value 999 must be ignored
        Assert.Equal(123u, info.OriginInfo.MegFileLocation!.DataEntry.Location.Offset);
        Assert.False(info.Encrypted);
    }

    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    public void Factory_OriginIsEntryReference_OverridesProperties_PathEmpty_Throws(string path)
    {
        FileSystem.File.Create("file.meg");
        var meg = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);
        var entry = MegDataEntryTest.CreateEntry("path", default, 123, 321, true);
        Assert.Throws<ArgumentException>(() => MegFileDataEntryBuilderInfo.FromEntry(meg, entry, path, false));
    }

    #endregion
}