using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.Testing;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

[TestClass]
public class PetroglyphRelativeDataEntryPathResolverTest
{
    private readonly PetroglyphRelativeDataEntryPathResolver _pathResolver = new();

    [TestMethod]
    [DataRow(null, null)]
    [DataRow("", null)]
    [DataRow("   ", "   ")]
    [DataRow("test", "test")]
    [DataRow("test/", null)]
    [DataRow("test.xml", "test.xml")]
    [DataRow("a/test", "a/test")]
    [DataRow("a/.test", "a/.test")]
    [DataRow("./a/test.xml", "a/test")]
    [DataRow("../test.xml", null)]
    [DataRow("./../test.xml", null)]
    [DataRow("./../corruption/test.xml", "test.xml")]
    [DataRow("./../corruption/../test.xml", null)]
    public void Test_ResolveEntryPath_Relative(string path, string expectedEntryPath)
    {
        const string basePath = "/Games/Petroglyph/corruption/";

        var actualEntryPath = _pathResolver.ResolvePath(path, basePath);
        Assert.AreEqual(expectedEntryPath, actualEntryPath);
    }

    [PlatformSpecificTestMethod(TestPlatformIdentifier.Windows)]
    [DataRow("c:/game/corruption/", "c:/game/corruption/test", "test")]
    [DataRow("c:/game/corruption/", "c:/game/corruption/test/", null)]
    [DataRow("c:/game/corruption/", "c:/game/corruption/xml/test", "xml/test")]
    [DataRow("c:/game/corruption/", "C:/GAME/CORRUPTION/test", "test")] // Test case insensitivity on Windows
    [DataRow("c:/game/corruption/", "D:/game/corruption", null)]
    [DataRow("c:/game/corruption/", "D:/game/test", null)]
    [DataRow("c:/game/corruption/", "c:test", "test")]
    [DataRow("c:/game/corruption/", "c:xml/test", "xml/test")]
    [DataRow("c:/game/corruption/", "d:test", null)]
    public void TestResolveEntryPath_AbsoluteOrRooted_Windows(string basePath, string path, string expectedEntryPath)
    {
        var actualEntryPath = _pathResolver.ResolvePath(path, basePath);
        Assert.AreEqual(expectedEntryPath, actualEntryPath);
    }

    [PlatformSpecificTestMethod(TestPlatformIdentifier.Linux)]
    [DataRow("/game/corruption/", "/game/corruption/test", "test")]
    [DataRow("/game/corruption/", "/other/corruption/test/", null)]
    [DataRow("/game/corruption/", "/game/corruption/xml/test", "xml/test")]
    [DataRow("/game/corruption/", "/GAME/CORRUPTION/test", null)] // Test case sensitivity on Linux
    public void TestResolveEntryPath_AbsoluteOrRooted_Linux(string basePath, string path, string expectedEntryPath)
    {
        var actualEntryPath = _pathResolver.ResolvePath(path, basePath);
        Assert.AreEqual(expectedEntryPath, actualEntryPath);
    }
}