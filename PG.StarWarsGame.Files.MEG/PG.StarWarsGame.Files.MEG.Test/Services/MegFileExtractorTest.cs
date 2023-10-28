using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using PG.Testing;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

[TestClass]
public class MegFileExtractorTest
{
    private MegFileExtractor _extractor = null!;
    private readonly MockFileSystem _fileSystem = new();
    private readonly Mock<IServiceProvider> _serviceProvider = new();

    [TestInitialize]
    public void Init()
    {
        _serviceProvider.Setup(sp => sp.GetService(typeof(IFileSystem))).Returns(_fileSystem);
        _extractor = new MegFileExtractor(_serviceProvider.Object);
    }

    [TestMethod]
    public void Test_GetAbsoluteFilePath_Throws()
    {
        var entry = new MegDataEntry(new Crc32(123), "path", 456, 789);
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
        var path = _extractor.GetAbsoluteFilePath(Create(entryPath), rootDir, preserveHierarchy);
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
        var path = _extractor.GetAbsoluteFilePath(Create(entryPath), rootDir, preserveHierarchy);
        Assert.AreEqual(expectedPath, path);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Test_GetAbsoluteFilePath_ThrowsInvalidOperation()
    {
        var entry = Create("notAFile.txt/");
        _extractor.GetAbsoluteFilePath(entry, "someRoot", false);
    }

    [TestMethod]
    public void Test_GetFileData_ThrowsArgumentsIncorrect()
    {
        var entry = Create("file.txt");
        Assert.ThrowsException<ArgumentNullException>(() => _extractor.GetFileData(null!, entry));

        var meg = new Mock<IMegFile>();
        Assert.ThrowsException<ArgumentNullException>(() => _extractor.GetFileData(meg.Object, null!));
    }

    [TestMethod]
    public void Test_GetFileData_Throws_MegFileNotFound()
    { 
        var entry = Create("file.txt");

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.FilePath).Returns(_fileSystem.Path.GetFullPath("a.meg"));

        Assert.ThrowsException<FileNotFoundException>(() => _extractor.GetFileData(meg.Object, entry));
    }

    [TestMethod]
    public void Test_GetFileData_Throws_FileNotInMeg()
    {
        _fileSystem.AddEmptyFile("a.meg");
        var entry = Create("file.txt");

        var archive = new Mock<IMegArchive>();
        archive.Setup(a => a.Contains(entry)).Returns(false);

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.FilePath).Returns(_fileSystem.Path.GetFullPath("a.meg"));
        meg.SetupGet(m => m.Content).Returns(archive.Object);

        Assert.ThrowsException<FileNotInMegException>(() => _extractor.GetFileData(meg.Object, entry));
    }

    [TestMethod]
    public void Test_GetFileData()
    {
        var megFileDataStream = new MemoryStream(new byte[] { 1, 2, 3, 4 });
        _fileSystem.AddEmptyFile("a.meg");
        var entry = Create("file.txt");

        var archive = new Mock<IMegArchive>();
        archive.Setup(a => a.Contains(entry)).Returns(true);

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.FilePath).Returns(_fileSystem.Path.GetFullPath("a.meg"));
        meg.SetupGet(m => m.Content).Returns(archive.Object);


        var streamFactory = new Mock<IMegDataStreamFactory>();
        streamFactory.Setup(f => f.CreateDataStream(_fileSystem.Path.GetFullPath("a.meg"), entry.Offset, entry.Size)).Returns(megFileDataStream);
        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegDataStreamFactory))).Returns(streamFactory.Object);


        var stream = _extractor.GetFileData(meg.Object, entry);

        Assert.AreSame(megFileDataStream, stream);
    }

    [TestMethod]
    public void Test_ExtractFile_ThrowsArgumentsIncorrect()
    {
        var entry = Create("file.txt");
        Assert.ThrowsException<ArgumentNullException>(() => _extractor.ExtractFile(null!, entry, "file.txt", false));

        var meg = new Mock<IMegFile>();
        Assert.ThrowsException<ArgumentNullException>(() => _extractor.ExtractFile(meg.Object, null!, "file.txt", false));

        Assert.ThrowsException<ArgumentNullException>(() => _extractor.ExtractFile(meg.Object, entry, null!, false));

        Assert.ThrowsException<ArgumentException>(() => _extractor.ExtractFile(meg.Object, entry, "", false));
        Assert.ThrowsException<ArgumentException>(() => _extractor.ExtractFile(meg.Object, entry, "    ", false));
    }

    [TestMethod]
    public void Test_ExtractFile_Throws_MegFileNotFound()
    {
        var entry = Create("file.txt");

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.FilePath).Returns(_fileSystem.Path.GetFullPath("a.meg"));

        Assert.ThrowsException<FileNotFoundException>(() => _extractor.ExtractFile(meg.Object, entry, "file.txt", false));
    }

    [TestMethod]
    public void Test_ExtractFile_Throws_FileNotInMeg()
    {
        _fileSystem.AddEmptyFile("a.meg");
        var entry = Create("file.txt");

        var archive = new Mock<IMegArchive>();
        archive.Setup(a => a.Contains(entry)).Returns(false);

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.FilePath).Returns(_fileSystem.Path.GetFullPath("a.meg"));
        meg.SetupGet(m => m.Content).Returns(archive.Object);

        Assert.ThrowsException<FileNotInMegException>(() => _extractor.ExtractFile(meg.Object, entry, "file.txt", false));
    }

    [TestMethod]
    public void Test_ExtractData_NoOverwrite()
    {
        var megFileData = new byte[] { 1, 2, 3, 4 };
        _fileSystem.AddEmptyFile("a.meg");
        var entry = Create("file.txt");

        var archive = new Mock<IMegArchive>();
        archive.Setup(a => a.Contains(entry)).Returns(true);

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.FilePath).Returns(_fileSystem.Path.GetFullPath("a.meg"));
        meg.SetupGet(m => m.Content).Returns(archive.Object);


        var streamFactory = new Mock<IMegDataStreamFactory>();
        streamFactory.Setup(f => f.CreateDataStream(_fileSystem.Path.GetFullPath("a.meg"), entry.Offset, entry.Size)).Returns(new MemoryStream(megFileData));
        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegDataStreamFactory))).Returns(streamFactory.Object);


        Assert.IsFalse(_fileSystem.FileExists("file.txt"));

        var extracted = _extractor.ExtractFile(meg.Object, entry, "file.txt", false);
        Assert.IsTrue(extracted);

        Assert.IsTrue(_fileSystem.FileExists("file.txt"));

        var actualFileData = _fileSystem.File.ReadAllBytes("file.txt");
        CollectionAssert.AreEqual(megFileData, actualFileData);


        // Overwrite File with som other data;
        var otherFileData = new byte[] { 4, 3, 2, 1 };
        _fileSystem.File.WriteAllBytes("file.txt", otherFileData);

        //Extract again
        extracted = _extractor.ExtractFile(meg.Object, entry, "file.txt", false);
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

        var entry = Create("file.txt");

        var archive = new Mock<IMegArchive>();
        archive.Setup(a => a.Contains(entry)).Returns(true);

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.FilePath).Returns(_fileSystem.Path.GetFullPath("a.meg"));
        meg.SetupGet(m => m.Content).Returns(archive.Object);


        var streamFactory = new Mock<IMegDataStreamFactory>();
        streamFactory.Setup(f => f.CreateDataStream(_fileSystem.Path.GetFullPath("a.meg"), entry.Offset, entry.Size)).Returns(new MemoryStream(megFileData));
        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegDataStreamFactory))).Returns(streamFactory.Object);


        CollectionAssert.AreEqual(existingFileData, _fileSystem.File.ReadAllBytes("file.txt"));

        var extracted = _extractor.ExtractFile(meg.Object, entry, "file.txt", true);
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

        var entry = Create(filePathWhereToExtract);

        var archive = new Mock<IMegArchive>();
        archive.Setup(a => a.Contains(entry)).Returns(true);

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.FilePath).Returns(_fileSystem.Path.GetFullPath("a.meg"));
        meg.SetupGet(m => m.Content).Returns(archive.Object);


        var streamFactory = new Mock<IMegDataStreamFactory>();
        streamFactory.Setup(f => f.CreateDataStream(_fileSystem.Path.GetFullPath("a.meg"), entry.Offset, entry.Size)).Returns(new MemoryStream(megFileData));
        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegDataStreamFactory))).Returns(streamFactory.Object);

        var extracted = _extractor.ExtractFile(meg.Object, entry, filePathWhereToExtract, false);
        Assert.IsTrue(extracted);

        var actualFileData = _fileSystem.File.ReadAllBytes(filePathWhereToExtract);
        CollectionAssert.AreEqual(megFileData, actualFileData);
    }

    [PlatformSpecificTestMethod(TestConstants.PLATFORM_WINDOWS)]
    [ExpectedException(typeof(ArgumentException))]
    [DataRow("c:/")]
    [DataRow("c:")]
    [DataRow(null)]
    public void Test_ExtractData_Throws_IllegalPath_Windows(string path)
    {
        _fileSystem.AddEmptyFile("a.meg");

        var filePathWhereToExtract = path;

        var entry = Create(filePathWhereToExtract);

        var archive = new Mock<IMegArchive>();
        archive.Setup(a => a.Contains(entry)).Returns(true);

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.FilePath).Returns(_fileSystem.Path.GetFullPath("a.meg"));
        meg.SetupGet(m => m.Content).Returns(archive.Object);

        _extractor.ExtractFile(meg.Object, entry, filePathWhereToExtract, false);
    }
    
    [ExpectedException(typeof(ArgumentException))]
    [PlatformSpecificTestMethod(TestConstants.PLATFORM_LINUX)]
    [DataRow("/")]
    [DataRow(null)]
    public void Test_ExtractData_Throws_IllegalPath_Linux(string path)
    {
        _fileSystem.AddEmptyFile("a.meg");

        var filePathWhereToExtract = path;

        var entry = Create(filePathWhereToExtract);

        var archive = new Mock<IMegArchive>();
        archive.Setup(a => a.Contains(entry)).Returns(true);

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.FilePath).Returns(_fileSystem.Path.GetFullPath("a.meg"));
        meg.SetupGet(m => m.Content).Returns(archive.Object);

        _extractor.ExtractFile(meg.Object, entry, filePathWhereToExtract, false);
    }


    private static MegDataEntry Create(string path, uint size = 1)
    {
        return new(new Crc32(0), path, 0, size);
    }
}