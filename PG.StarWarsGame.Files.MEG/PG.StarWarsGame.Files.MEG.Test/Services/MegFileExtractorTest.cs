using System;
using System.IO;
using System.IO.Abstractions;
using Moq;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using PG.StarWarsGame.Files.MEG.Utilities;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;
using static PG.StarWarsGame.Files.MEG.Test.Data.Entries.MegDataEntryTest;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

public class MegFileExtractorTest
{
    private readonly MegFileExtractor _extractor;
    private readonly MockFileSystem _fileSystem = new();
    private readonly Mock<IServiceProvider> _serviceProvider = new();
    private readonly Mock<IMegDataStreamFactory> _streamFactory = new();

    public MegFileExtractorTest()
    {
        _serviceProvider.Setup(sp => sp.GetService(typeof(IFileSystem))).Returns(_fileSystem);
        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegDataStreamFactory))).Returns(_streamFactory.Object);
        _extractor = new MegFileExtractor(_serviceProvider.Object);
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
    public void Test_GetFileData_Throws()
    { 
        var entry = CreateEntry("file.txt");
        var meg = new Mock<IMegFile>();
        var location = new MegDataEntryLocationReference(meg.Object, entry);

        _streamFactory.Setup(sf => sf.GetDataStream(location)).Throws<Exception>();

        Assert.Throws<Exception>(() => _extractor.GetFileData(location));
    }

    [Fact]
    public void Test_GetFileData()
    {
        var ms = new MemoryStream([1, 2, 3, 4]);
        var megFileDataStream = FromMemoryStream(ms);

        var entry = CreateEntry("file.txt");
        var meg = new Mock<IMegFile>();
        var location = new MegDataEntryLocationReference(meg.Object, entry);

        _streamFactory.Setup(f => f.GetDataStream(location)).Returns(megFileDataStream);
        
        var stream = _extractor.GetFileData(location);
        
        Assert.Same(megFileDataStream, stream);
    }

    [Fact]
    public void Test_ExtractFile_ThrowsArgumentsIncorrect()
    {
        Assert.Throws<ArgumentNullException>(() => _extractor.ExtractFile(null!, "path", false));

        var entry = CreateEntry("file.txt");
        var meg = new Mock<IMegFile>();
        var location = new MegDataEntryLocationReference(meg.Object, entry);

        Assert.Throws<ArgumentNullException>(() => _extractor.ExtractFile(location, null!, false));

        Assert.Throws<ArgumentException>(() => _extractor.ExtractFile(location, "", false));
        Assert.Throws<ArgumentException>(() => _extractor.ExtractFile(location, "    ", false));
    }

    [Fact]
    public void Test_ExtractFile_Throws()
    {
        var entry = CreateEntry("file.txt");
        var meg = new Mock<IMegFile>();
        var location = new MegDataEntryLocationReference(meg.Object, entry);

        _streamFactory.Setup(sf => sf.GetDataStream(location)).Throws<Exception>();

        Assert.Throws<Exception>(() => _extractor.ExtractFile(location, "file.txt", false));
    }

    [Fact]
    public void Test_ExtractData_NoOverwrite()
    {
        var megFileData = new byte[] { 1, 2, 3, 4 };
        _fileSystem.Initialize().WithFile("a.meg");

        var entry = CreateEntry("file.txt");
        var meg = new Mock<IMegFile>();
        var location = new MegDataEntryLocationReference(meg.Object, entry);

        Assert.False(_fileSystem.File.Exists("file.txt"));

        _streamFactory.Setup(f => f.GetDataStream(location)).Returns(FromMemoryStream(new MemoryStream(megFileData)));

        var extracted = _extractor.ExtractFile(location, "file.txt", false);
        
        Assert.True(extracted);
        Assert.True(_fileSystem.File.Exists("file.txt"));

        var actualFileData = _fileSystem.File.ReadAllBytes("file.txt");
        Assert.Equal(megFileData, actualFileData);


        // Overwrite File with some other data;
        var otherFileData = new byte[] { 4, 3, 2, 1 };
        _fileSystem.File.WriteAllBytes("file.txt", otherFileData);

        //Extract again
        extracted = _extractor.ExtractFile(location, "file.txt", false);
        Assert.False(extracted);

        actualFileData = _fileSystem.File.ReadAllBytes("file.txt");
        Assert.Equal(otherFileData, actualFileData);
    }

    [Fact]
    public void Test_ExtractData_Overwrite()
    {
        var megFileData = new byte[] { 1, 2, 3, 4 };

        var existingFileData = new byte[] { 4, 3, 2, 1 };
        _fileSystem.Initialize().WithFile("file.txt").Which(m => m.HasBytesContent(existingFileData));

        _fileSystem.Initialize().WithFile("a.meg");

        Assert.Equal(existingFileData, _fileSystem.File.ReadAllBytes("file.txt"));

        var entry = CreateEntry("file.txt");
        var meg = new Mock<IMegFile>();
        var location = new MegDataEntryLocationReference(meg.Object, entry);

        _streamFactory.Setup(f => f.GetDataStream(location)).Returns(FromMemoryStream(new MemoryStream(megFileData)));

        var extracted = _extractor.ExtractFile(location, "file.txt", true);
        Assert.True(extracted);

        var actualFileData = _fileSystem.File.ReadAllBytes("file.txt");
        Assert.Equal(megFileData, actualFileData);
    }

    [Fact]
    public void Test_ExtractData_CreateDirectories()
    {
        var megFileData = new byte[] { 1, 2, 3, 4 };

        _fileSystem.Initialize().WithFile("a.meg");

        var filePathWhereToExtract = "new/file.txt";

        var entry = CreateEntry(filePathWhereToExtract);
        var meg = new Mock<IMegFile>();
        var location = new MegDataEntryLocationReference(meg.Object, entry);


       _streamFactory.Setup(f => f.GetDataStream(location)).Returns(FromMemoryStream(new MemoryStream(megFileData)));

        var extracted = _extractor.ExtractFile(location, filePathWhereToExtract, false);
        Assert.True(extracted);

        var actualFileData = _fileSystem.File.ReadAllBytes(filePathWhereToExtract);
        Assert.Equal(megFileData, actualFileData);
    }

    private MegFileDataStream FromMemoryStream(MemoryStream stream)
    {
        return new MegFileDataStream("path", stream, 0, (uint)stream.Length);
    }

    [PlatformSpecificTheory(TestPlatformIdentifier.Windows)]
    [InlineData("c:/")]
    [InlineData("c:")]
    public void Test_ExtractData_Throws_IllegalPath_Windows(string filePathWhereToExtract)
    {
        _fileSystem.Initialize().WithFile("a.meg");

        var entry = CreateEntry("path");
        var meg = new Mock<IMegFile>();
        var location = new MegDataEntryLocationReference(meg.Object, entry);

        Assert.Throws<ArgumentException>(() => _extractor.ExtractFile(location, filePathWhereToExtract, false));
    }

    [PlatformSpecificTheory(TestPlatformIdentifier.Linux)]
    [InlineData("/")]
    public void Test_ExtractData_Throws_IllegalPath_Linux(string filePathWhereToExtract)
    {
        _fileSystem.Initialize().WithFile("a.meg");

        var entry = CreateEntry("path");
        var meg = new Mock<IMegFile>();
        var location = new MegDataEntryLocationReference(meg.Object, entry);

        Assert.Throws<ArgumentException>(() => _extractor.ExtractFile(location, filePathWhereToExtract, false));
    }
}