using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Test.Files;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

[TestClass]
public class EmpireAtWarMegBuilder_IntegrationTest
{
    private readonly MockFileSystem _fileSystem = new();
    private EmpireAtWarMegBuilder _eawMegBuilder;
    private IServiceProvider _serviceProvider;

    [TestInitialize]
    public void Setup()
    {
        var gamePath = "/game/corruption/data";
        _fileSystem.AddDirectory(gamePath);

        _serviceProvider = CreateServiceProvider();

        _eawMegBuilder = new EmpireAtWarMegBuilder(gamePath, _serviceProvider);
    }

    private IServiceProvider CreateServiceProvider()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        MegDomain.RegisterServices(sc);
        return sc.BuildServiceProvider();
    }

    [TestMethod]
    public void Test_BuildMeg()
    {
        _fileSystem.AddFile("entry.txt", new MockFileData("test"));

        var entry1 = _eawMegBuilder.ResolveEntryPath("entry1.txt");
        var entry2 = _eawMegBuilder.ResolveEntryPath("/game/corruption/data/xml/entry2.txt");
        var entry3 = _eawMegBuilder.ResolveEntryPath("/other/corruption/data/xml/entry3.txt");

        Assert.AreEqual("entry1", entry1);
        Assert.AreEqual("xml/entry2.txt", entry2);
        Assert.IsNull(entry3);

        var result1 = _eawMegBuilder.AddFile("entry.txt", entry1);
        var result1a = _eawMegBuilder.AddFile("entry.txt", entry1, true);
        var result2 = _eawMegBuilder.AddFile("entry.txt", entry2);
        var result2a = _eawMegBuilder.AddFile("entry.txt", "/game/corruption/data/xml/entry2.txt");
        var result3 = _eawMegBuilder.AddFile("entry.txt", "/other/corruption/data/xml/entry3.txt");

        Assert.IsTrue(result1.Added);
        Assert.IsFalse(result1a.Added);
        Assert.IsTrue(result2.Added);
        Assert.IsFalse(result2a.Added);
        Assert.IsFalse(result3.Added);

        Assert.AreEqual("ENTRY.TXT", result1.AddedBuilderInfo!.FilePath);
        Assert.AreEqual("XML\\ENTRY2.TXT", result2.AddedBuilderInfo!.FilePath);

        Assert.AreEqual(2, _eawMegBuilder.DataEntries.Count);

        Assert.IsFalse(_eawMegBuilder.ValidateFileInformation(new MegFileInformation("new.meg", MegFileVersion.V2)));
        Assert.IsFalse(_eawMegBuilder.ValidateFileInformation(new MegFileInformation("?new.meg", MegFileVersion.V1)));
        Assert.IsFalse(_eawMegBuilder.ValidateFileInformation(new MegFileInformation("new.meg", MegFileVersion.V3, MegEncryptionDataTest.CreateRandomData())));

        _eawMegBuilder.Build(new MegFileInformation("new.meg", MegFileVersion.V1), false);

        Assert.IsTrue(_fileSystem.FileExists("new.meg"));

        var megFileService = _serviceProvider.GetRequiredService<IMegFileService>();
        var meg = megFileService.Load("new.meg");

        Assert.AreEqual(2, meg.Archive.Count);

        var packedEntry1 = meg.Archive.First(x => x.FilePath.Equals("ENTRY.TXT"));
        var packedEntry2 = meg.Archive.First(x => x.FilePath.Equals("XML\\ENTRY2.TXT"));

        Assert.IsNotNull(packedEntry1);
        Assert.IsNotNull(packedEntry2);

        var extractor = _serviceProvider.GetRequiredService<IMegFileExtractor>();
        var entry1Data = extractor.GetFileData(new MegDataEntryLocationReference(meg, packedEntry1));
        var entry2Data = extractor.GetFileData(new MegDataEntryLocationReference(meg, packedEntry1));

        using var ms = new MemoryStream();
        entry1Data.CopyTo(ms);
        entry2Data.CopyTo(ms);

        var dataString = new StreamReader(ms).ReadToEnd();
        Assert.AreEqual("testtest",dataString);
    }
}