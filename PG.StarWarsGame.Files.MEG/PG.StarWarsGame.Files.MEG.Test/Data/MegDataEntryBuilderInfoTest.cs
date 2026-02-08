using System;
using System.IO;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Data;

public class MegDataEntryBuilderInfoTest : CommonMegTestBase
{
    [Fact]
    public void Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new MegDataEntryBuilderInfo(null!));
    }

    #region Ctor_LocalFile

    [Fact]
    public void Ctor_FileDoesNotExist_ThrowsFileNotFoundException()
    {
        var origin = new MegDataEntryOriginInfo(FileSystem.FileInfo.New("test.xml"));
        Assert.Throws<FileNotFoundException>(() => new MegDataEntryBuilderInfo(origin));
        Assert.Throws<FileNotFoundException>(() => new MegDataEntryBuilderInfo(origin, "other.xml"));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(null, false)]
    [InlineData("TEST.XML", true)]
    [InlineData("TEST.XML", false)]
    [InlineData("    ", false)]
    public void Ctor_OriginIsLocalFile(string? overridePath, bool encrypted)
    {
        using (var w = FileSystem.File.CreateText("test.xml"))
            w.Write("123");

        var origin = new MegDataEntryOriginInfo(FileSystem.FileInfo.New("test.xml"));
        var info = new MegDataEntryBuilderInfo(origin, overridePath, encrypted);

        Assert.Same(origin, info.OriginInfo);
        Assert.Equal(overridePath ?? origin.FileInfo!.FullName, info.EntryPath);
        Assert.Equal(encrypted, info.Encrypted);
        Assert.Equal(3u, info.Size);
    }

    [Fact]
    public void Ctor_OriginIsLocalFile_PathEmpty_Throws()
    {
        using (var w = FileSystem.File.CreateText("test.xml"))
            w.Write("123");

        var origin = new MegDataEntryOriginInfo(FileSystem.FileInfo.New("test.xml"));
        Assert.Throws<ArgumentException>(() => new MegDataEntryBuilderInfo(origin, string.Empty));
    }


    [Fact]
    public void Ctor_OriginIsLocalFile_FileTooLarge_ThrowsMegEntrySizeException()
    {
        var mockFileInfo = new MegTestConstants.FakeFileInfo("large_file.bin", (long)uint.MaxValue + 1);
        var origin = new MegDataEntryOriginInfo(mockFileInfo);
        Assert.Throws<MegEntrySizeException>(() => new MegDataEntryBuilderInfo(origin, "path"));
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

        var info = new MegDataEntryBuilderInfo(origin);

        Assert.Same(origin, info.OriginInfo);
        Assert.Equal("path", info.EntryPath);
        Assert.Equal(321u, info.Size);
        Assert.Equal(123u, info.OriginInfo.MegFileLocation!.DataEntry.Location.Offset);
        Assert.True(info.Encrypted);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(null, false)]
    [InlineData("TEST.XML", true)]
    [InlineData("TEST.XML", false)]
    [InlineData("    ", false)]
    public void Ctor_OriginIsEntryReference_OverridesProperties(string? overridePath, bool encrypted)
    {
        FileSystem.File.Create("file.meg");
        var meg = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);
        var origin = new MegDataEntryOriginInfo(new MegDataEntryLocationReference(
            meg, MegDataEntryTest.CreateEntry("path", default, 123, 321, true)));

        var info = new MegDataEntryBuilderInfo(origin, overridePath, encrypted);

        Assert.Same(origin, info.OriginInfo);
        Assert.Equal(overridePath ?? "path", info.EntryPath);
        Assert.Equal(321u, info.Size);
        Assert.Equal(encrypted, info.Encrypted);
    }

    [Theory]
    [InlineData("")]
    public void Ctor_OriginIsEntryReference_OverridesProperties_PathEmpty_Throws(string path)
    {
        FileSystem.File.Create("file.meg");
        var meg = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);
        var origin = new MegDataEntryOriginInfo(new MegDataEntryLocationReference(
            meg, MegDataEntryTest.CreateEntry("path", default, 123, 321, true)));
        Assert.Throws<ArgumentException>(() => new MegDataEntryBuilderInfo(origin, path, true));
    }

    #endregion

    #region Factory_FromFile

    [Theory]
    [InlineData("TEST.XML", true)]
    [InlineData("TEST.XML", false)]
    [InlineData("    ", false)]
    [InlineData("    ", true)]
    public void FromFile(string overridePath, bool encrypted)
    {
        using (var w = FileSystem.File.CreateText("test.xml"))
            w.Write("123");

        var info = MegDataEntryBuilderInfo.FromFile(FileSystem.FileInfo.New("test.xml"), overridePath, encrypted);
        Assert.True(info.OriginInfo.IsLocalFile);
        Assert.Equal(overridePath, info.EntryPath);
        Assert.Equal(3u, info.Size);
        Assert.Equal(encrypted, info.Encrypted);
    }

    [Fact]
    public void FromFile_FileDoesNotExist_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>((Action)(() => 
            MegDataEntryBuilderInfo.FromFile(FileSystem.FileInfo.New("test.xml"), "test.xml", true)));
    }

    [Fact]
    public void FromFile_FileInfoNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => MegDataEntryBuilderInfo.FromFile(null!, "random"));
    }

    [Fact]
    public void FromFile_InvalidEntryPath_ThrowsArgumentException()
    {
        using (var w = FileSystem.File.CreateText("test.xml"))
            w.Write("123");
        Assert.Throws<ArgumentNullException>(() => 
            MegDataEntryBuilderInfo.FromFile(FileSystem.FileInfo.New("test.xml"), null!));
        Assert.Throws<ArgumentException>(() =>
            MegDataEntryBuilderInfo.FromFile(FileSystem.FileInfo.New("test.xml"), string.Empty));
    }

    #endregion

    #region Factory FromEntry

    [Fact]
    public void FromEntry_NullArgs()
    {
        FileSystem.File.Create("file.meg");
        var entry = MegDataEntryTest.CreateEntry("test.xml", default, 123, 321, true);
        var meg = new MegFile(new MegArchive([entry]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);
       
        Assert.Throws<ArgumentNullException>(() => MegDataEntryBuilderInfo.FromEntry(null!, entry));
        Assert.Throws<ArgumentNullException>(() => MegDataEntryBuilderInfo.FromEntry(meg, null!));
    }

    [Fact]
    public void FromEntry_EntryNotInMeg_ThrowsArgumentException()
    {
        FileSystem.File.Create("file.meg");
        var meg = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);
        var entry = MegDataEntryTest.CreateEntry("test.xml", default, 123, 321, true);

        Assert.Throws<ArgumentException>(() => MegDataEntryBuilderInfo.FromEntry(meg, entry));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(null, false)]
    [InlineData("TEST.XML", true)]
    [InlineData("TEST.XML", false)]
    [InlineData("    ", false)]
    public void FromEntry_SetsProperties(string? overridePath, bool encrypted)
    {
        FileSystem.File.Create("file.meg");
        var entry = MegDataEntryTest.CreateEntry("test.xml", default, 123, 321, true);
        var meg = new MegFile(new MegArchive([entry]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);
        
        var info = MegDataEntryBuilderInfo.FromEntry(meg, entry, overridePath, encrypted);

        Assert.True(info.OriginInfo.IsEntryReference);
        Assert.Equal(overridePath ?? "test.xml", info.EntryPath);
        Assert.Equal(321u, info.Size);
        Assert.Equal(123u, info.OriginInfo.MegFileLocation!.DataEntry.Location.Offset);
        Assert.Equal(encrypted, info.Encrypted);
    }

    [Fact]
    public void FromEntry_OriginIsEntryReference_EntryPathEmpty_ThrowsArgumentException()
    {
        FileSystem.File.Create("file.meg");
        var entry = MegDataEntryTest.CreateEntry("path", default, 123, 321, true);
        var meg = new MegFile(new MegArchive([entry]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);
        
        Assert.Throws<ArgumentException>(() =>
            MegDataEntryBuilderInfo.FromEntry(meg, entry, string.Empty, false));
    }

    #endregion

    #region Factory FromEntryReference

    [Fact]
    public void FromEntryReference_NullArgs()
    {
        Assert.Throws<ArgumentNullException>(() => MegDataEntryBuilderInfo.FromEntryReference(null!));
    }

    [Fact]
    public void FromEntryReference_EntryNotInMeg_ThrowsArgumentException()
    {
        FileSystem.File.Create("file.meg");
        var meg = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);
        var entry = MegDataEntryTest.CreateEntry("test.xml", default, 123, 321, true);

        Assert.Throws<ArgumentException>(() => 
            MegDataEntryBuilderInfo.FromEntryReference(new MegDataEntryLocationReference(meg, entry)));
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(null, false)]
    [InlineData("TEST.XML", true)]
    [InlineData("TEST.XML", false)]
    [InlineData("    ", false)]
    public void FromEntryReference_OriginIsLocalFile(string? overridePath, bool encrypted)
    {
        FileSystem.File.Create("file.meg");
        var entry = MegDataEntryTest.CreateEntry("test.xml", default, 123, 321, true);
        var meg = new MegFile(new MegArchive([entry]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);
        
        var info = MegDataEntryBuilderInfo.FromEntryReference(new MegDataEntryLocationReference(meg, entry), overridePath, encrypted);

        Assert.True(info.OriginInfo.IsEntryReference);
        Assert.Equal(overridePath ?? "test.xml", info.EntryPath);
        Assert.Equal(321u, info.Size);
        Assert.Equal(123u, info.OriginInfo.MegFileLocation!.DataEntry.Location.Offset);
        Assert.Equal(encrypted, info.Encrypted);
    }

    [Fact]
    public void FromEntryReference_OriginIsEntryReference_EntryPathEmpty_ThrowsArgumentException()
    {
        FileSystem.File.Create("file.meg");
        var entry = MegDataEntryTest.CreateEntry("path", default, 123, 321, true);
        var meg = new MegFile(new MegArchive([entry]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);
        Assert.Throws<ArgumentException>(() => MegDataEntryBuilderInfo.FromEntryReference(
            new MegDataEntryLocationReference(meg, entry), string.Empty, false));
    }

    #endregion

    #region RefreshSize

    [Fact]
    public void RefreshSize_FromMegEntry()
    {
        FileSystem.File.Create("file.meg");
        var entry = MegDataEntryTest.CreateEntry("path", default, 123, 321, true);
        var meg = new MegFile(new MegArchive([entry]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);

        var info = MegDataEntryBuilderInfo.FromEntryReference(new MegDataEntryLocationReference(meg, entry));

        Assert.Equal(321u, info.Size);

        info.RefreshSize();

        Assert.Equal(321u, info.Size);
    }

    [Fact]
    public void RefreshSize_FromFile_SizeUpdates()
    {
        using (var w = FileSystem.File.CreateText("test.xml"))
            w.Write("123");

        var info = MegDataEntryBuilderInfo.FromFile(FileSystem.FileInfo.New("test.xml"), "test.xml");

        Assert.Equal(3u, info.Size);

        using (var w = FileSystem.File.CreateText("test.xml"))
            w.Write("");

        info.RefreshSize();

        Assert.Equal(0u, info.Size);
    }

    [Fact]
    public void RefreshSize_FromFile_FileDeleted_ThrowsFileNotFoundException()
    {
        using (var w = FileSystem.File.CreateText("test.xml"))
            w.Write("123");

        var info = MegDataEntryBuilderInfo.FromFile(FileSystem.FileInfo.New("test.xml"), "test.xml");

        Assert.Equal(3u, info.Size);

        FileSystem.File.Delete("test.xml");

        Assert.Throws<FileNotFoundException>(() => info.RefreshSize());
    }

    [Fact]
    public void RefreshSize_FileTooLarge_ThrowsMegEntrySizeException()
    {
        var mockFileInfo = new MegTestConstants.FakeFileInfo("large_file.bin", uint.MaxValue);
        var info = MegDataEntryBuilderInfo.FromFile(mockFileInfo, "path");
        
        Assert.Equal(uint.MaxValue, info.Size);

        info.RefreshSize();

        mockFileInfo.Length = (long)uint.MaxValue + 1;

        Assert.Throws<MegEntrySizeException>(() => info.RefreshSize());
    }

    #endregion
}