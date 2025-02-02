using System.IO.Abstractions;
using AnakinRaW.CommonUtilities.FileSystem.Normalization;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.Testing;
using Testably.Abstractions;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

public class PetroglyphRelativeDataEntryPathResolverTest
{
    private readonly PetroglyphRelativeDataEntryPathResolver _pathResolver;
    private readonly IFileSystem _fileSystem = new RealFileSystem();

    public PetroglyphRelativeDataEntryPathResolverTest()
    {
        var sc = new ServiceCollection();
        // Use the real file system here
        sc.AddSingleton(_fileSystem);
        _pathResolver = new PetroglyphRelativeDataEntryPathResolver(sc.BuildServiceProvider());
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("test", "test")]
    [InlineData("test/", null)]
    [InlineData("test.xml", "test.xml")]
    [InlineData("a/test", "a\\test")]
    [InlineData("a/.test", "a\\.test")]
    [InlineData("./a/test.xml", "a\\test.xml")]
    [InlineData("../test.xml", null)]
    [InlineData("./../test.xml", null)]
    [InlineData("./../corruption/test.xml", "test.xml")]
    [InlineData("./../corruption/../test.xml", null)]
    [InlineData("../corruption1/test.xml", null)]
    [InlineData("Games/Petroglyph/corruption/test", "test", true)]
    [InlineData("Games/Petroglyph/corruption/test/", null, true)]
    [InlineData("Games/Petroglyph/corruption1/test", null, true)]
    public void ResolveEntryPath_Relative(string? path, string? expectedEntryPath, bool resolvePathFull = false)
    {
        const string basePath = "Games/Petroglyph/corruption/";


        var normalizedExpected = expectedEntryPath is not null
            ? PathNormalizer.Normalize(expectedEntryPath, new PathNormalizeOptions { UnifyDirectorySeparators = true })
            : expectedEntryPath;

        if (resolvePathFull)
            path = _fileSystem.Path.GetFullPath(path!);

        var actualEntryPath = _pathResolver.ResolvePath(path, basePath);
        Assert.Equal(normalizedExpected, actualEntryPath);
    }

    [PlatformSpecificTheory(TestPlatformIdentifier.Windows)]
    [InlineData("   ", null)]
    [InlineData("c:/game/corruption/test", "test")]
    [InlineData("c:\\game\\corruption\\test", "test")]
    [InlineData("c:/game/corruption/test/", null)]
    [InlineData("c:/game/corruption/xml/test", "xml\\test")]
    [InlineData("C:/GAME/CORRUPTION/test", "test")] // Test case insensitivity on Windows
    [InlineData("D:/game/corruption", null)]
    [InlineData("D:/game/test", null)]
    [InlineData("c:test", "test")]
    [InlineData("C:xml/test", "xml\\test")]
    [InlineData("c:", null)]
    [InlineData("d:test", null)]
    public void TestResolveEntryPath_AbsoluteOrRooted_Windows(string path, string? expectedEntryPath)
    {
        const string basePath = "c:/game/corruption";
        var actualEntryPath = _pathResolver.ResolvePath(path, basePath);
        Assert.Equal(expectedEntryPath, actualEntryPath);
    }

    [PlatformSpecificTheory(TestPlatformIdentifier.Linux)]
    [InlineData("   ", "   ")]
    [InlineData("/game/corruption/test", "test")]
    [InlineData("/other/corruption/test/", null)]
    [InlineData("/game/corruption/xml/test", "xml/test")]
    [InlineData("/GAME/CORRUPTION/test", null)] // Test case sensitivity on Linux
    public void TestResolveEntryPath_AbsoluteOrRooted_Linux(string path, string? expectedEntryPath)
    {
        const string basePath = "/game/corruption/";
        var actualEntryPath = _pathResolver.ResolvePath(path, basePath);
        Assert.Equal(expectedEntryPath, actualEntryPath);
    }
}