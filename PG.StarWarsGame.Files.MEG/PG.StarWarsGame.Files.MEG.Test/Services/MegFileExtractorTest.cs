using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using PG.Testing;

using static PG.StarWarsGame.Files.MEG.Test.Data.Entries.MegDataEntryTest;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

[TestClass]
public class MegFileExtractorTest
{
    private MegFileExtractor _extractor = null!;
    private readonly MockFileSystem _fileSystem = new();
    private readonly Mock<IServiceProvider> _serviceProvider = new();
    private readonly Mock<IMegDataStreamFactory> _streamFactory = new();

    [TestInitialize]
    public void Init()
    {
        _serviceProvider.Setup(sp => sp.GetService(typeof(IFileSystem))).Returns(_fileSystem);
        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegDataStreamFactory))).Returns(_streamFactory.Object);
        _extractor = new MegFileExtractor(_serviceProvider.Object);
    }

    [TestMethod]
    public void Test_GetAbsoluteFilePath_Throws()
    {
        var entry = CreateEntry("path");
        Assert.ThrowsException<ArgumentNullException>(() => _extractor.GetAbsoluteFilePath(null!, "path", false));
        Assert.ThrowsException<ArgumentNullException>(() => _extractor.GetAbsoluteFilePath(entry, null!, false));
        Assert.ThrowsException<ArgumentException>(() => _extractor.GetAbsoluteFilePath(entry, "", false));
        Assert.ThrowsException<ArgumentException>(() => _extractor.GetAbsoluteFilePath(entry, "   ", false));
    }



    [PlatformSpecificTestMethod(TestConstants.PLATFORM_WINDOWS)]
    [TestMethod]
    [DataRow("a.txt", "new", true, "C:\\new\\a.txt")]
    [DataRow("path/a.txt", "new", true, "C:\\new\\path\\a.txt")]
    [DataRow("../a.txt", "new", true, "C:\\a.txt")]
    [DataRow("a.txt", "D:\\", true, "D:\\a.txt")]
    [DataRow("../a.txt", "D:\\new\\", true, "D:\\a.txt")]
    [DataRow("D:\\new\\..\\a.txt", "new", true, "D:\\a.txt")]
    [DataRow("/a.txt", "D:/new", true, "C:\\a.txt")] // Note that /a.txt is rooted but not absolute (on Windows).
    //[DataRow("D:a.txt", "D:/new", true, "D:/a.txt")] // There is a bug in System.IO.Abstraction that prevents testing case. https://github.com/TestableIO/System.IO.Abstractions/issues/1044
    [DataRow("a.txt", "new", false, "C:\\new\\a.txt")]
    [DataRow("path/a.txt", "new", false, "C:\\new\\a.txt")]
    [DataRow("../a.txt", "new", false, "C:\\new\\a.txt")]
    [DataRow("a.txt", "D:\\", false, "D:\\a.txt")]
    [DataRow("..a.txt", "D:\\", false, "D:\\..a.txt")]
    [DataRow("C:/a.txt", "D:\\", false, "D:\\a.txt")]

    public void Test_GetAbsoluteFilePath_Windows(string entryPath, string rootDir, bool preserveHierarchy, string expectedPath)
    {
        var path = _extractor.GetAbsoluteFilePath(CreateEntry(entryPath), rootDir, preserveHierarchy);
        Assert.AreEqual(expectedPath, path);
    }

    [PlatformSpecificTestMethod(TestConstants.PLATFORM_LINUX)]
    [TestMethod]
    [DataRow("a.txt", "new", true, "/new/a.txt")]
    [DataRow("path/a.txt", "new", true, "/new/path/a.txt")]
    [DataRow("../a.txt", "new", true, "/a.txt")]
    [DataRow("a.txt", "/new", true, "/new/a.txt")]
    [DataRow("../a.txt", "/new/", true, "/a.txt")]
    [DataRow("/new/../a.txt", "path", true, "/a.txt")]
    [DataRow("a.txt", "new", false, "/new/a.txt")]
    [DataRow("path/a.txt", "new", false, "/new/a.txt")]
    [DataRow("../a.txt", "new", false, "/new/a.txt")]
    [DataRow("a.txt", "/", false, "/a.txt")]
    [DataRow("..a.txt", "/", false, "/..a.txt")]
    [DataRow("/new/a.txt", "/path/", false, "/path/a.txt")]

    public void Test_GetAbsoluteFilePath_Linux(string entryPath, string rootDir, bool preserveHierarchy, string expectedPath)
    {
        var path = _extractor.GetAbsoluteFilePath(CreateEntry(entryPath), rootDir, preserveHierarchy);
        Assert.AreEqual(expectedPath, path);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Test_GetAbsoluteFilePath_ThrowsInvalidOperation()
    {
        var entry = CreateEntry("notAFile.txt/");
        _extractor.GetAbsoluteFilePath(entry, "someRoot", false);
    }

    [TestMethod]
    public void Test_GetFileData_ThrowsArgumentNull()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _extractor.GetFileData(null!));
    }

    [TestMethod]
    public void Test_GetFileData_Throws()
    { 
        var entry = CreateEntry("file.txt");
        var meg = new Mock<IMegFile>();
        var location = new MegDataEntryLocationReference(meg.Object, entry);

        _streamFactory.Setup(sf => sf.GetDataStream(location)).Throws<Exception>();

        Assert.ThrowsException<Exception>(() => _extractor.GetFileData(location));
    }

    // TODO: This is a good test for the StreamFactory
    //[TestMethod]
    //public void Test_GetFileData_Throws_FileNotInMeg()
    //{
    //    _fileSystem.AddEmptyFile("a.meg");
    //    var entry = CreateEntry("file.txt");

    //    var archive = new Mock<IMegArchive>();
    //    archive.Setup(a => a.Contains(entry)).Returns(false);

    //    var meg = new Mock<IMegFile>();
    //    meg.SetupGet(m => m.FilePath).Returns(_fileSystem.Path.GetFullPath("a.meg"));
    //    meg.SetupGet(m => m.Content).Returns(archive.Object);

    //    var location = new MegDataEntryLocationReference(meg.Object, entry);

    //    Assert.ThrowsException<FileNotInMegException>(() => _extractor.GetFileData(location));
    //}

    [TestMethod]
    public void Test_GetFileData()
    {
        var megFileDataStream = new MemoryStream(new byte[] { 1, 2, 3, 4 });

        var entry = CreateEntry("file.txt");
        var meg = new Mock<IMegFile>();
        var location = new MegDataEntryLocationReference(meg.Object, entry);

        _streamFactory.Setup(f => f.GetDataStream(location)).Returns(megFileDataStream);
        
        var stream = _extractor.GetFileData(location);
        
        Assert.AreSame(megFileDataStream, stream);
    }

    [TestMethod]
    public void Test_ExtractFile_ThrowsArgumentsIncorrect()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _extractor.ExtractFile(null!, "path", false));

        var entry = CreateEntry("file.txt");
        var meg = new Mock<IMegFile>();
        var location = new MegDataEntryLocationReference(meg.Object, entry);

        Assert.ThrowsException<ArgumentNullException>(() => _extractor.ExtractFile(location, null!, false));

        Assert.ThrowsException<ArgumentException>(() => _extractor.ExtractFile(location, "", false));
        Assert.ThrowsException<ArgumentException>(() => _extractor.ExtractFile(location, "    ", false));
    }

    [TestMethod]
    public void Test_ExtractFile_Throws()
    {
        var entry = CreateEntry("file.txt");
        var meg = new Mock<IMegFile>();
        var location = new MegDataEntryLocationReference(meg.Object, entry);

        _streamFactory.Setup(sf => sf.GetDataStream(location)).Throws<Exception>();

        Assert.ThrowsException<Exception>(() => _extractor.ExtractFile(location, "file.txt", false));
    }

    [TestMethod]
    public void Test_ExtractData_NoOverwrite()
    {
        var megFileData = new byte[] { 1, 2, 3, 4 };
        _fileSystem.AddEmptyFile("a.meg");
        
        var entry = CreateEntry("file.txt");
        var meg = new Mock<IMegFile>();
        var location = new MegDataEntryLocationReference(meg.Object, entry);

        Assert.IsFalse(_fileSystem.FileExists("file.txt"));

        _streamFactory.Setup(f => f.GetDataStream(location)).Returns(new MemoryStream(megFileData));

        var extracted = _extractor.ExtractFile(location, "file.txt", false);
        
        Assert.IsTrue(extracted);
        Assert.IsTrue(_fileSystem.FileExists("file.txt"));

        var actualFileData = _fileSystem.File.ReadAllBytes("file.txt");
        CollectionAssert.AreEqual(megFileData, actualFileData);


        // Overwrite File with some other data;
        var otherFileData = new byte[] { 4, 3, 2, 1 };
        _fileSystem.File.WriteAllBytes("file.txt", otherFileData);

        //Extract again
        extracted = _extractor.ExtractFile(location, "file.txt", false);
        Assert.IsFalse(extracted);

        actualFileData = _fileSystem.File.ReadAllBytes("file.txt");
        CollectionAssert.AreEqual(otherFileData, actualFileData);
    }

    [TestMethod]
    public void Test_ExtractData_Overwrite()
    {
        var megFileData = new byte[] { 1, 2, 3, 4 };

        var existingFileData = new byte[] { 4, 3, 2, 1 };
        _fileSystem.AddFile("file.txt", new MockFileData(existingFileData));

        _fileSystem.AddEmptyFile("a.meg");

        CollectionAssert.AreEqual(existingFileData, _fileSystem.File.ReadAllBytes("file.txt"));

        var entry = CreateEntry("file.txt");
        var meg = new Mock<IMegFile>();
        var location = new MegDataEntryLocationReference(meg.Object, entry);

        _streamFactory.Setup(f => f.GetDataStream(location)).Returns(new MemoryStream(megFileData));

        var extracted = _extractor.ExtractFile(location, "file.txt", true);
        Assert.IsTrue(extracted);

        var actualFileData = _fileSystem.File.ReadAllBytes("file.txt");
        CollectionAssert.AreEqual(megFileData, actualFileData);
    }

    [TestMethod]
    public void Test_ExtractData_CreateDirectories()
    {
        var megFileData = new byte[] { 1, 2, 3, 4 };

        _fileSystem.AddEmptyFile("a.meg");

        var filePathWhereToExtract = "new/file.txt";

        var entry = CreateEntry(filePathWhereToExtract);
        var meg = new Mock<IMegFile>();
        var location = new MegDataEntryLocationReference(meg.Object, entry);


       _streamFactory.Setup(f => f.GetDataStream(location)).Returns(new MemoryStream(megFileData));

        var extracted = _extractor.ExtractFile(location, filePathWhereToExtract, false);
        Assert.IsTrue(extracted);

        var actualFileData = _fileSystem.File.ReadAllBytes(filePathWhereToExtract);
        CollectionAssert.AreEqual(megFileData, actualFileData);
    }

    [PlatformSpecificTestMethod(TestConstants.PLATFORM_WINDOWS)]
    [ExpectedException(typeof(ArgumentException))]
    [DataRow("c:/")]
    [DataRow("c:")]
    public void Test_ExtractData_Throws_IllegalPath_Windows(string filePathWhereToExtract)
    {
        _fileSystem.AddEmptyFile("a.meg");

        var entry = CreateEntry("path");
        var meg = new Mock<IMegFile>();
        var location = new MegDataEntryLocationReference(meg.Object, entry);

        _extractor.ExtractFile(location, filePathWhereToExtract, false);
    }

    [ExpectedException(typeof(ArgumentException))]
    [PlatformSpecificTestMethod(TestConstants.PLATFORM_LINUX)]
    [DataRow("/")]
    public void Test_ExtractData_Throws_IllegalPath_Linux(string filePathWhereToExtract)
    {
        _fileSystem.AddEmptyFile("a.meg");

        var entry = CreateEntry("path");
        var meg = new Mock<IMegFile>();
        var location = new MegDataEntryLocationReference(meg.Object, entry);

        _extractor.ExtractFile(location, filePathWhereToExtract, false);
    }
}