using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;
using static PG.StarWarsGame.Files.MEG.Test.Data.Entries.MegDataEntryTest;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

public class MegFileExtractorTest : CommonMegTestBase
{
    private readonly MegFileExtractor _extractor;

    public MegFileExtractorTest()
    {
        _extractor = new MegFileExtractor(ServiceProvider);
    }

    [Fact]
    public void Test_GetAbsoluteFilePath_Throws()
    {
        var entry = CreateEntry("path");
        Assert.Throws<ArgumentNullException>(() => _extractor.GetAbsoluteFilePath(null!, "path", false));
        Assert.Throws<ArgumentNullException>(() => _extractor.GetAbsoluteFilePath(entry, null!, false));
        Assert.Throws<ArgumentException>(() => _extractor.GetAbsoluteFilePath(entry, "", false));
        Assert.Throws<ArgumentException>(() => _extractor.GetAbsoluteFilePath(entry, "   ", false));
    }

    [PlatformSpecificTheory(TestPlatformIdentifier.Windows)]
    [InlineData("a.txt", "new", true, "C:\\new\\a.txt")]
    [InlineData("path/a.txt", "new", true, "C:\\new\\path\\a.txt")]
    [InlineData("../a.txt", "new", true, "C:\\a.txt")]
    [InlineData("a.txt", "D:\\", true, "D:\\a.txt")]
    [InlineData("../a.txt", "D:\\new\\", true, "D:\\a.txt")]
    [InlineData("D:\\new\\..\\a.txt", "new", true, "D:\\a.txt")]
    //[InlineData("/a.txt", "D:/new", true, "C:\\a.txt")] // Note that /a.txt is rooted but not absolute (on Windows).
    //[InlineData("D:a.txt", "D:/new", true, "D:\\a.txt")]
    [InlineData("a.txt", "new", false, "C:\\new\\a.txt")]
    [InlineData("path/a.txt", "new", false, "C:\\new\\a.txt")]
    [InlineData("../a.txt", "new", false, "C:\\new\\a.txt")]
    [InlineData("a.txt", "D:\\", false, "D:\\a.txt")]
    [InlineData("..a.txt", "D:\\", false, "D:\\..a.txt")]
    [InlineData("C:/a.txt", "D:\\", false, "D:\\a.txt")]

    public void Test_GetAbsoluteFilePath_Windows(string entryPath, string rootDir, bool preserveHierarchy, string expectedPath)
    {
        var path = _extractor.GetAbsoluteFilePath(CreateEntry(entryPath), rootDir, preserveHierarchy);
        Assert.Equal(expectedPath, path);
    }

    [PlatformSpecificTheory(TestPlatformIdentifier.Linux)]
    [InlineData("a.txt", "new", true, "/new/a.txt")]
    [InlineData("path/a.txt", "new", true, "/new/path/a.txt")]
    [InlineData("../a.txt", "new", true, "/a.txt")]
    [InlineData("a.txt", "/new", true, "/new/a.txt")]
    [InlineData("../a.txt", "/new/", true, "/a.txt")]
    [InlineData("/new/../a.txt", "path", true, "/a.txt")]
    [InlineData("a.txt", "new", false, "/new/a.txt")]
    [InlineData("path/a.txt", "new", false, "/new/a.txt")]
    [InlineData("../a.txt", "new", false, "/new/a.txt")]
    [InlineData("a.txt", "/", false, "/a.txt")]
    [InlineData("..a.txt", "/", false, "/..a.txt")]
    [InlineData("/new/a.txt", "/path/", false, "/path/a.txt")]

    public void Test_GetAbsoluteFilePath_Linux(string entryPath, string rootDir, bool preserveHierarchy, string expectedPath)
    {
        var path = _extractor.GetAbsoluteFilePath(CreateEntry(entryPath), rootDir, preserveHierarchy);
        Assert.Equal(expectedPath, path);
    }

    [Fact]
    public void Test_GetAbsoluteFilePath_ThrowsInvalidOperation()
    {
        var entry = CreateEntry("notAFile.txt/");
        Assert.Throws<InvalidOperationException>(() => _extractor.GetAbsoluteFilePath(entry, "someRoot", false));
    }

    [Fact]
    public void Test_GetFileData_ThrowsArgumentNull()
    {
        Assert.Throws<ArgumentNullException>(() => _extractor.GetFileData(null!));
    }

    [Fact]
    public void GetFileData_CannotReadFile_Throws()
    { 
        // Size 12 is not valid, as the entry does not really exist.
        var entry = CreateEntry("file.txt", default, 0, 12);

        FileSystem.File.Create("test.meg");
        var meg = new MegFile(new MegArchive([entry]), new MegFileInformation("test.meg", MegFileVersion.V1),
            ServiceProvider);

        var location = new MegDataEntryLocationReference(meg, entry);

        Assert.Throws<IOException>(() => _extractor.GetFileData(location));
    }

    [Fact]
    public void Test_ExtractFile_ThrowsArgumentsIncorrect()
    {
        Assert.Throws<ArgumentNullException>(() => _extractor.ExtractFile(null!, "path", false));

        var entry = CreateEntry("file.txt");

        using (FileSystem.File.Create("test.meg")) ;
        var meg = new MegFile(new MegArchive([entry]), new MegFileInformation("test.meg", MegFileVersion.V1),
            ServiceProvider);

        var location = new MegDataEntryLocationReference(meg, entry);

        Assert.Throws<ArgumentNullException>(() => _extractor.ExtractFile(location, null!, false));

        Assert.Throws<ArgumentException>(() => _extractor.ExtractFile(location, "", false));
        Assert.Throws<ArgumentException>(() => _extractor.ExtractFile(location, "    ", false));
    }

    [PlatformSpecificTheory(TestPlatformIdentifier.Windows)]
    [InlineData("c:/")]
    [InlineData("c:")]
    public void Test_ExtractData_Throws_IllegalPath_Windows(string filePathWhereToExtract)
    {
        FileSystem.Initialize().WithFile("a.meg");

        var entry = CreateEntry("path");
        var meg = new MegFile(new MegArchive([entry]), new MegFileInformation("a.meg", MegFileVersion.V1),
            ServiceProvider);
        var location = new MegDataEntryLocationReference(meg, entry);

        Assert.Throws<ArgumentException>(() => _extractor.ExtractFile(location, filePathWhereToExtract, false));
    }

    [PlatformSpecificTheory(TestPlatformIdentifier.Linux)]
    [InlineData("/")]
    public void Test_ExtractData_Throws_IllegalPath_Linux(string filePathWhereToExtract)
    {
        FileSystem.Initialize().WithFile("a.meg");

        var entry = CreateEntry("path");
        var meg = new MegFile(new MegArchive([entry]), new MegFileInformation("a.meg", MegFileVersion.V1),
            ServiceProvider);

        var location = new MegDataEntryLocationReference(meg, entry);

        Assert.Throws<ArgumentException>(() => _extractor.ExtractFile(location, filePathWhereToExtract, false));
    }

    [Fact]
    public void Test_GetFileData()
    {
        FileSystem.Initialize()
            .WithFile("test.meg").Which(m => m.HasBytesContent(MegTestConstants.ContentMegFileV1));

        var meg = ServiceProvider.GetRequiredService<IMegFileService>().Load("test.meg");

        // CampaignFiles.xml
        var entry = meg.Content[0];
        var location = new MegDataEntryLocationReference(meg, entry);

        using var stream = _extractor.GetFileData(location);

        var ms = new MemoryStream();
        stream.CopyTo(ms);

        Assert.Equal(MegTestConstants.CampaignFilesContent, ms.ToArray());
    }

    [Fact]
    public void Test_ExtractData_NoOverwrite()
    {
        FileSystem.Initialize()
            .WithFile("test.meg").Which(m => m.HasBytesContent(MegTestConstants.ContentMegFileV1));

        var meg = ServiceProvider.GetRequiredService<IMegFileService>().Load("test.meg");

        // CampaignFiles.xml
        var entry = meg.Content[0];
        var location = new MegDataEntryLocationReference(meg, entry);

        var extracted = _extractor.ExtractFile(location, "file.txt", false);

        Assert.True(extracted);
        Assert.True(FileSystem.File.Exists("file.txt"));

        var actualFileData = FileSystem.File.ReadAllBytes("file.txt");
        Assert.Equal(MegTestConstants.CampaignFilesContent, actualFileData);


        // Overwrite File with some other data;
        var otherFileData = new byte[] { 4, 3, 2, 1 };
        FileSystem.File.WriteAllBytes("file.txt", otherFileData);

        //Extract again
        extracted = _extractor.ExtractFile(location, "file.txt", false);
        Assert.False(extracted);

        actualFileData = FileSystem.File.ReadAllBytes("file.txt");
        Assert.Equal(otherFileData, actualFileData);
    }

    [Fact]
    public void Test_ExtractData_Overwrite()
    {
        var existingFileData = new byte[] { 4, 3, 2, 1 };
        FileSystem.Initialize()
            .WithFile("file.txt").Which(m => m.HasBytesContent(existingFileData))
            .WithFile("test.meg").Which(m => m.HasBytesContent(MegTestConstants.ContentMegFileV1));

        Assert.Equal(existingFileData, FileSystem.File.ReadAllBytes("file.txt"));

        var meg = ServiceProvider.GetRequiredService<IMegFileService>().Load("test.meg");

        // CampaignFiles.xml
        var entry = meg.Content[0];
        var location = new MegDataEntryLocationReference(meg, entry);

        var extracted = _extractor.ExtractFile(location, "file.txt", true);
        Assert.True(extracted);

        var actualFileData = FileSystem.File.ReadAllBytes("file.txt");
        Assert.Equal(MegTestConstants.CampaignFilesContent, actualFileData);
    }

    [Fact]
    public void Test_ExtractData_CreateDirectories()
    { 
        const string filePathWhereToExtract = "new/file.txt";

        FileSystem.Initialize()
            .WithFile("test.meg").Which(m => m.HasBytesContent(MegTestConstants.ContentMegFileV1));

        var meg = ServiceProvider.GetRequiredService<IMegFileService>().Load("test.meg");

        // CampaignFiles.xml
        var entry = meg.Content[0];
        var location = new MegDataEntryLocationReference(meg, entry);


        var extracted = _extractor.ExtractFile(location, filePathWhereToExtract, false);
        Assert.True(extracted);

        var actualFileData = FileSystem.File.ReadAllBytes(filePathWhereToExtract);
        Assert.Equal(MegTestConstants.CampaignFilesContent, actualFileData);
    }
}