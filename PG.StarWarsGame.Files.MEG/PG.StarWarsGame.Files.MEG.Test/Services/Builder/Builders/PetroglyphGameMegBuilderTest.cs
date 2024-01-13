using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using PG.Testing;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

[TestClass]
public class PetroglyphGameMegBuilderTest : MegBuilderTestSuite
{
    public const string BasePath = "/Games/Petroglyph/corruption/";

    protected override Type ExpectedFileInfoValidatorType => typeof(DefaultFileInformationValidator);
    protected override Type ExpectedDataEntryValidatorType => typeof(NotNullDataEntryValidator);
    protected override Type? ExpectedDataEntryPathNormalizerType => null;
    protected override bool? ExpectedOverwritesDuplicates => true;
    protected override bool? ExpectedAutomaticallyAddFileSizes => true;

    private PetroglyphGameMegBuilder CreatePetroBuilder(string basePath, IServiceProvider serviceProvider)
    {
        return new TestPetroglyphGameMegBuilder(basePath, serviceProvider);
    }

    protected override MegBuilderBase CreateBuilder(IServiceProvider serviceProvider)
    {
        return CreatePetroBuilder(BasePath, serviceProvider);
    }

    [TestMethod]
    public new void Test_Ctor_Throws()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(FileSystem);
        Assert.ThrowsException<ArgumentNullException>(() => new EmpireAtWarMegBuilder(null!, sc.BuildServiceProvider()));
        Assert.ThrowsException<ArgumentException>(() => new EmpireAtWarMegBuilder("", sc.BuildServiceProvider()));
    }

    [TestMethod]
    public new void Test_Ctor()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(FileSystem);
        var builder = CreatePetroBuilder(BasePath, sc.BuildServiceProvider());
        Assert.AreEqual(FileSystem.Path.GetFullPath(BasePath), builder.BaseDirectory);
    }

    [TestMethod]
    public void Test_Ctor_BasePathIsTreatedAsDirectory()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(FileSystem);

        // Skipping trailing path separator on purpose
        var builder = CreatePetroBuilder("/game/corruption.dir", sc.BuildServiceProvider());

        // Assert trailing path separator in instance.
        Assert.AreEqual(FileSystem.Path.GetFullPath("/game/corruption.dir/"), builder.BaseDirectory);
    }

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
        var builder = CreatePetroBuilder(BasePath, CreateServiceProvider());
        var actualEntryPath = builder.ResolveEntryPath(path);
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
        var builder = CreatePetroBuilder(basePath, CreateServiceProvider());
        var actualEntryPath = builder.ResolveEntryPath(path);
        Assert.AreEqual(expectedEntryPath, actualEntryPath);
    }

    [PlatformSpecificTestMethod(TestPlatformIdentifier.Linux)]
    [DataRow("/game/corruption/", "/game/corruption/test", "test")]
    [DataRow("/game/corruption/", "/other/corruption/test/", null)]
    [DataRow("/game/corruption/", "/game/corruption/xml/test", "xml/test")]
    [DataRow("/game/corruption/", "/GAME/CORRUPTION/test", null)] // Test case sensitivity on Linux
    public void TestResolveEntryPath_AbsoluteOrRooted_Linux(string basePath, string path, string expectedEntryPath)
    {
        var builder = CreatePetroBuilder(basePath, CreateServiceProvider());
        var actualEntryPath = builder.ResolveEntryPath(path);
        Assert.AreEqual(expectedEntryPath, actualEntryPath);
    }


    private class TestPetroglyphGameMegBuilder(string baseDirectory, IServiceProvider services)
        : PetroglyphGameMegBuilder(baseDirectory, services);
}