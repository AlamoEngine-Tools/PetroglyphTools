using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AnakinRaW.CommonUtilities.FileSystem.Normalization;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using PG.StarWarsGame.Files.MEG.Test.Files;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

public class EmpireAtWarMegBuilderTest : PetroglyphGameMegBuilderTest
{
    protected override bool CanProduceInvalidEntryPaths => true;

    protected override Type ExpectedDataEntryValidatorType => typeof(EmpireAtWarMegBuilderDataEntryValidator);

    protected override Type ExpectedFileInfoValidatorType => typeof(EmpireAtWarMegFileInformationValidator);

    protected override Type ExpectedDataEntryPathNormalizerType => typeof(EmpireAtWarMegDataEntryPathNormalizer);

    protected override void SetupServices(ServiceCollection serviceCollection)
    {
        base.SetupServices(serviceCollection);
        serviceCollection.SupportMEG();
    }

    protected override PetroglyphGameMegBuilder CreatePetroBuilder(string basePath)
    {
        return new EmpireAtWarMegBuilder(basePath, ServiceProvider);
    }

    protected override PetroglyphGameMegBuilder CreateBuilder()
    {
        return new EmpireAtWarMegBuilder(BasePath, ServiceProvider);
    }

    protected override void AddDataToBuilder(IReadOnlyCollection<MegFileDataEntryBuilderInfo> data, PetroglyphGameMegBuilder builder)
    {
        foreach (var info in data)
        {
            var entryPath = builder.ResolveEntryPath(FileSystem.Path.GetFullPath(info.FilePath))!;
            builder.AddFile(info.FilePath, entryPath);
        }
    }

    protected override MegFileInformation CreateInvalidFileInfo(string path)
    {
        return new MegFileInformation(path, MegFileVersion.V2);
    }

    protected override string GetFailingEntryPath()
    {
        return TestUtility.GetRandom(["test\0test", new string('a', 300)]);
    }

    protected override (IReadOnlyCollection<MegFileDataEntryBuilderInfo> Data, byte[] Bytes) CreateValidData()
    {
        var xmlDir = FileSystem.Directory.CreateDirectory(FileSystem.Path.Combine(BasePath, "DATA", "XML"));

        var goFile = FileSystem.Path.Combine(xmlDir.FullName, "GAMEOBJECTFILES.XML");
        var cfFile = FileSystem.Path.Combine(xmlDir.FullName, "CAMPAIGNFILES.XML");

        FileSystem.File.WriteAllText(goFile, MegTestConstants.GameObjectFilesContent);
        FileSystem.File.WriteAllText(cfFile, MegTestConstants.CampaignFilesContent);

        var testMeg = new List<MegFileDataEntryBuilderInfo>
        {
            new(new MegDataEntryOriginInfo(goFile)),
            new(new MegDataEntryOriginInfo(cfFile))
        };

        return (testMeg, MegTestConstants.ContentMegFileV1);
    }

    [Fact]
    public void PetroglyphGameMegBuilderTest_Test_Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new EmpireAtWarMegBuilder(null!, ServiceProvider));
        Assert.Throws<ArgumentException>(() => new EmpireAtWarMegBuilder("", ServiceProvider));
    }

    [Fact]
    public void Test_BuildMeg()
    {
        var builder = CreateBuilder();

        FileSystem.Initialize().WithFile("entry.txt").Which(m => m.HasStringContent("test"));

        var entry1 = builder.ResolveEntryPath("entry1.txt");
        var entry2 = builder.ResolveEntryPath(FileSystem.Path.GetFullPath(FileSystem.Path.Combine(BasePath, "DATA", "XML", "entry2.txt")));
        var entry3 = builder.ResolveEntryPath(FileSystem.Path.GetFullPath(FileSystem.Path.Combine("other", BasePath, "DATA", "XML", "entry3.txt")));
        var entry4 = builder.ResolveEntryPath(FileSystem.Path.GetFullPath(FileSystem.Path.Combine(BasePath, "DATA", "XML", "entry4ÖÄÜ.txt")));

        Assert.Equal("entry1.txt", entry1);
        Assert.Equal(PathNormalizer.Normalize("DATA/XML/entry2.txt", new PathNormalizeOptions { UnifyDirectorySeparators = true }), entry2);
        Assert.Null(entry3);
        Assert.Equal(PathNormalizer.Normalize("DATA/XML/entry4ÖÄÜ.txt", new PathNormalizeOptions { UnifyDirectorySeparators = true }), entry4);

        var result1 = builder.AddFile("entry.txt", entry1!);
        var result1a = builder.AddFile("entry.txt", entry1!, true);
        var result2 = builder.AddFile("entry.txt", entry2!);
        var result3 = builder.AddFile("entry.txt", "/other/corruption/data/xml/entry3.txt");
        var result4 = builder.AddFile("entry.txt", entry4!);

        Assert.True(result1.Added);
        Assert.False(result1a.Added);
        Assert.True(result2.Added);
        Assert.True(result3.Added);
        Assert.True(result4.Added);

        Assert.Equal("ENTRY1.TXT", result1.AddedBuilderInfo!.FilePath);
        Assert.Equal("DATA\\XML\\ENTRY2.TXT", result2.AddedBuilderInfo!.FilePath);
        Assert.Equal("OTHER\\CORRUPTION\\DATA\\XML\\ENTRY3.TXT", result3.AddedBuilderInfo!.FilePath);
        Assert.Equal("DATA\\XML\\ENTRY4???.TXT", result4.AddedBuilderInfo!.FilePath);

        Assert.Equal(4, builder.DataEntries.Count);

        Assert.False(builder.ValidateFileInformation(new MegFileInformation("new.meg", MegFileVersion.V2)));
        Assert.False(builder.ValidateFileInformation(new MegFileInformation("?new.meg", MegFileVersion.V1)));
        Assert.False(builder.ValidateFileInformation(new MegFileInformation("new.meg", MegFileVersion.V3, MegEncryptionDataTest.CreateRandomData())));

        builder.Build(new MegFileInformation("new.meg", MegFileVersion.V1), false);

        Assert.True(FileSystem.File.Exists("new.meg"));

        var megFileService = ServiceProvider.GetRequiredService<IMegFileService>();
        var meg = megFileService.Load("new.meg");

        Assert.Equal(4, meg.Archive.Count);

        var packedEntry1 = meg.Archive.First(x => x.FilePath.Equals("ENTRY1.TXT"));
        var packedEntry2 = meg.Archive.First(x => x.FilePath.Equals("DATA\\XML\\ENTRY2.TXT"));
        var packedEntry3 = meg.Archive.First(x => x.FilePath.Equals("OTHER\\CORRUPTION\\DATA\\XML\\ENTRY3.TXT"));
        var packedEntry4 = meg.Archive.First(x => x.FilePath.Equals("DATA\\XML\\ENTRY4???.TXT"));

        Assert.NotNull(packedEntry1);
        Assert.NotNull(packedEntry2);
        Assert.NotNull(packedEntry3);
        Assert.NotNull(packedEntry4);

        var extractor = ServiceProvider.GetRequiredService<IMegFileExtractor>();
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