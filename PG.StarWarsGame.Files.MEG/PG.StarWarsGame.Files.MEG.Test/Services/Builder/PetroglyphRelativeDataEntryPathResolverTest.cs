using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.Testing;
using FileSystem = System.IO.Abstractions.FileSystem;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

[TestClass]
public class PetroglyphRelativeDataEntryPathResolverTest
{
    private PetroglyphRelativeDataEntryPathResolver _pathResolver;

    [TestInitialize]
    public void Setup()
    {
        var sc = new ServiceCollection();
        // Use the real file system here
        sc.AddSingleton<IFileSystem>(new FileSystem());
        _pathResolver = new PetroglyphRelativeDataEntryPathResolver(sc.BuildServiceProvider());
    }

    [TestMethod]
    [DataRow(null, null)]
    [DataRow("", null)]
    [DataRow("test", "test")]
    [DataRow("test/", null)]
    [DataRow("test.xml", "test.xml")]
    [DataRow("a/test", "a\\test")]
    [DataRow("a/.test", "a\\.test")]
    [DataRow("./a/test.xml", "a\\test.xml")]
    [DataRow("../test.xml", null)]
    [DataRow("./../test.xml", null)]
    [DataRow("./../corruption/test.xml", "test.xml")]
    [DataRow("./../corruption/../test.xml", null)]
    [DataRow("../corruption1/test.xml", null)]
    [DataRow("/Games/Petroglyph/corruption/test", "test")]
    [DataRow("/Games/Petroglyph/corruption/test/", null)]
    [DataRow("/Games/Petroglyph/corruption1/test", null)]
    public void Test_ResolveEntryPath_Relative(string path, string expectedEntryPath)
    {
        const string basePath = "/Games/Petroglyph/corruption/";

        var actualEntryPath = _pathResolver.ResolvePath(path, basePath);
        Assert.AreEqual(expectedEntryPath, actualEntryPath);
    }

    [PlatformSpecificTestMethod(TestPlatformIdentifier.Windows)]
    [DataRow("   ", null)]
    [DataRow("c:/game/corruption/test", "test")]
    [DataRow("c:\\game\\corruption\\test", "test")]
    [DataRow("c:/game/corruption/test/", null)]
    [DataRow("c:/game/corruption/xml/test", "xml\\test")]
    [DataRow("C:/GAME/CORRUPTION/test", "test")] // Test case insensitivity on Windows
    [DataRow("D:/game/corruption", null)]
    [DataRow("D:/game/test", null)]
    [DataRow("c:test", "test")]
    [DataRow("C:xml/test", "xml\\test")]
    [DataRow("c:", null)]
    [DataRow("d:test", null)]
    public void TestResolveEntryPath_AbsoluteOrRooted_Windows(string path, string expectedEntryPath)
    {
        const string basePath = "c:/game/corruption";
        var actualEntryPath = _pathResolver.ResolvePath(path, basePath);
        Assert.AreEqual(expectedEntryPath, actualEntryPath);
    }

    [PlatformSpecificTestMethod(TestPlatformIdentifier.Linux)]
    [DataRow("   ", "   ")]
    [DataRow("/game/corruption/test", "test")]
    [DataRow("/other/corruption/test/", null)]
    [DataRow("/game/corruption/xml/test", "xml/test")]
    [DataRow("/GAME/CORRUPTION/test", null)] // Test case sensitivity on Linux
    public void TestResolveEntryPath_AbsoluteOrRooted_Linux(string path, string expectedEntryPath)
    {
        const string basePath = "/game/corruption/";
        var actualEntryPath = _pathResolver.ResolvePath(path, basePath);
        Assert.AreEqual(expectedEntryPath, actualEntryPath);
    }
}