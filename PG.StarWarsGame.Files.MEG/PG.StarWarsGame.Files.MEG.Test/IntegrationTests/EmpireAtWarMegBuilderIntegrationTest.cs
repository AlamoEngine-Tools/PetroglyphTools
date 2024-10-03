using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using AnakinRaW.CommonUtilities.FileSystem.Normalization;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Extensibility;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Test.Files;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.IntegrationTests;


public class EmpireAtWarMegBuilderIntegrationTest
{
    private readonly MockFileSystem _fileSystem = new();
    private readonly EmpireAtWarMegBuilder _eawMegBuilder;
    private readonly IServiceProvider _serviceProvider;

    public EmpireAtWarMegBuilderIntegrationTest()
    {
        var gamePath = "/game/corruption/data";
        _fileSystem.Initialize().WithSubdirectory(gamePath);

        _serviceProvider = CreateServiceProvider();

        _eawMegBuilder = new EmpireAtWarMegBuilder(gamePath, _serviceProvider);
    }

    private IServiceProvider CreateServiceProvider()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        sc.CollectPgServiceContributions();
        return sc.BuildServiceProvider();
    }

    [Fact]
    public void Test_BuildMeg()
    {
        _fileSystem.Initialize().WithFile("entry.txt").Which(m => m.HasStringContent("test"));

        var entry1 = _eawMegBuilder.ResolveEntryPath("entry1.txt");
        var entry2 = _eawMegBuilder.ResolveEntryPath("/game/corruption/data/xml/entry2.txt");
        var entry3 = _eawMegBuilder.ResolveEntryPath("/other/corruption/data/xml/entry3.txt");
        var entry4 = _eawMegBuilder.ResolveEntryPath("/game/corruption/data/xml/entry4ÖÄÜ.txt");

        Assert.Equal("entry1.txt", entry1);
        Assert.Equal(PathNormalizer.Normalize("xml\\entry2.txt", new PathNormalizeOptions { UnifyDirectorySeparators = true }), entry2);
        Assert.Null(entry3);
        Assert.Equal(PathNormalizer.Normalize("xml\\entry4ÖÄÜ.txt", new PathNormalizeOptions { UnifyDirectorySeparators = true }), entry4);

        var result1 = _eawMegBuilder.AddFile("entry.txt", entry1!);
        var result1a = _eawMegBuilder.AddFile("entry.txt", entry1!, true);
        var result2 = _eawMegBuilder.AddFile("entry.txt", entry2!);
        var result3 = _eawMegBuilder.AddFile("entry.txt", "/other/corruption/data/xml/entry3.txt");
        var result4 = _eawMegBuilder.AddFile("entry.txt", entry4!);

        Assert.True(result1.Added);
        Assert.False(result1a.Added);
        Assert.True(result2.Added);
        Assert.True(result3.Added);
        Assert.True(result4.Added);

        Assert.Equal("ENTRY1.TXT", result1.AddedBuilderInfo!.FilePath);
        Assert.Equal("XML\\ENTRY2.TXT", result2.AddedBuilderInfo!.FilePath);
        Assert.Equal("OTHER\\CORRUPTION\\DATA\\XML\\ENTRY3.TXT", result3.AddedBuilderInfo!.FilePath);
        Assert.Equal("XML\\ENTRY4???.TXT", result4.AddedBuilderInfo!.FilePath);

        Assert.Equal(4, _eawMegBuilder.DataEntries.Count);

        Assert.False(_eawMegBuilder.ValidateFileInformation(new MegFileInformation("new.meg", MegFileVersion.V2)));
        Assert.False(_eawMegBuilder.ValidateFileInformation(new MegFileInformation("?new.meg", MegFileVersion.V1)));
        Assert.False(_eawMegBuilder.ValidateFileInformation(new MegFileInformation("new.meg", MegFileVersion.V3, MegEncryptionDataTest.CreateRandomData())));

        _eawMegBuilder.Build(new MegFileInformation("new.meg", MegFileVersion.V1), false);

        Assert.True(_fileSystem.File.Exists("new.meg"));

        var megFileService = _serviceProvider.GetRequiredService<IMegFileService>();
        var meg = megFileService.Load("new.meg");

        Assert.Equal(4, meg.Archive.Count);

        var packedEntry1 = meg.Archive.First(x => x.FilePath.Equals("ENTRY1.TXT"));
        var packedEntry2 = meg.Archive.First(x => x.FilePath.Equals("XML\\ENTRY2.TXT"));
        var packedEntry3 = meg.Archive.First(x => x.FilePath.Equals("OTHER\\CORRUPTION\\DATA\\XML\\ENTRY3.TXT"));
        var packedEntry4 = meg.Archive.First(x => x.FilePath.Equals("XML\\ENTRY4???.TXT"));

        Assert.NotNull(packedEntry1);
        Assert.NotNull(packedEntry2);
        Assert.NotNull(packedEntry3);
        Assert.NotNull(packedEntry4);

        var extractor = _serviceProvider.GetRequiredService<IMegFileExtractor>();
        var entry1Data = extractor.GetFileData(new MegDataEntryLocationReference(meg, packedEntry1));
        var entry2Data = extractor.GetFileData(new MegDataEntryLocationReference(meg, packedEntry2));
        var entry3Data = extractor.GetFileData(new MegDataEntryLocationReference(meg, packedEntry3));
        var entry4Data = extractor.GetFileData(new MegDataEntryLocationReference(meg, packedEntry4));

        using var ms = new MemoryStream();
        entry1Data.CopyTo(ms);
        entry2Data.CopyTo(ms);
        entry3Data.CopyTo(ms);
        entry4Data.CopyTo(ms);

        ms.Position = 0;

        var dataString = new StreamReader(ms).ReadToEnd();
        Assert.Equal("test" + "test" + "test" + "test", dataString);
    }
}