using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Services;
using PG.Testing;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

[TestClass]
public class MegFileExtractorTest
{
    private MegFileExtractor _extractor = null!;
    private readonly MockFileSystem _fileSystem = new();

    [TestInitialize]
    public void Init()
    {
        var sp = new ServiceCollection();
        sp.AddSingleton<IFileSystem>(_fileSystem);
        _extractor = new MegFileExtractor(sp.BuildServiceProvider());
    }

    [TestMethod]
    public void Test_GetAbsoluteFilePath_Throws()
    {
        var entry = new MegFileDataEntry(new Crc32(123), "path", 456, 789);
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
        var path = _extractor.GetAbsoluteFilePath(CreateFromFile(entryPath), rootDir, preserveHierarchy);
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
        var path = _extractor.GetAbsoluteFilePath(CreateFromFile(entryPath), rootDir, preserveHierarchy);
        Assert.AreEqual(expectedPath, path);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Test_GetAbsoluteFilePath_ThrowsInvalidOperation()
    {
        var entry = CreateFromFile("notAFile.txt/");
        _extractor.GetAbsoluteFilePath(entry, "someRoot", false);
    }


    private static MegFileDataEntry CreateFromFile(string path)
    {
        return new(new Crc32(0), path, 0, 0);
    }
}