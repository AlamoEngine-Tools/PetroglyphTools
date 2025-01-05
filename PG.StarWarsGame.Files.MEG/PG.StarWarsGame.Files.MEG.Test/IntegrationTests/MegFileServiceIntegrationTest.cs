using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.IntegrationTests;

public class MegFileServiceIntegrationTest : CommonMegTestBase
{
    private readonly IMegFileService _megFileService;

    public MegFileServiceIntegrationTest()
    {
        _megFileService = ServiceProvider.GetRequiredService<IMegFileService>();
    }

    private void TestMegFiles(string megFilePath, ExpectedMegTestData expectedData)
    {
        var megVersion = _megFileService.GetMegFileVersion(megFilePath, out var encrypted);
        Assert.Equal(expectedData.IsMegFileVersion, megVersion);
        Assert.Equal(expectedData.IsMegEncrypted, encrypted);

        var meg = _megFileService.Load(megFilePath);
        TestMegModelContent(meg, expectedData, false);

        for (var i = 0; i < meg.Archive.Count; i++)
        {
            var entry = meg.Archive[i];
            var expected = expectedData.EntryNames[i];
            Assert.Equal(expected, entry.FilePath);
        }

        using var param = new MegFileInformation(
            expectedData.NewMegFilePath,
            expectedData.NewMegFileVersion,
            expectedData.EncryptionData);

        var builderInformation = meg.Archive.Select(e =>
            new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(new MegDataEntryLocationReference(meg, e))));

        using (var fs = FileSystem.File.OpenWrite(expectedData.NewMegFilePath))
        {
            _megFileService.CreateMegArchive(fs, expectedData.NewMegFileVersion, expectedData.EncryptionData,
                builderInformation);
        }

        Assert.True(FileSystem.File.Exists(expectedData.NewMegFilePath));

        var createdVersion = _megFileService.GetMegFileVersion(expectedData.NewMegFilePath, out var newEncrypted);
        Assert.Equal(expectedData.NewMegFileVersion, createdVersion);
        Assert.Equal(expectedData.EncryptionData is null, !newEncrypted);


        var actualBytes = FileSystem.File.ReadAllBytes(expectedData.NewMegFilePath);
        var expectedBytes = FileSystem.File.ReadAllBytes(megFilePath);
        if (expectedData.NewMegIsBinaryEqual)
            Assert.Equal(expectedBytes, actualBytes);
        else
            Assert.NotEqual(expectedBytes, actualBytes);

        var newMeg = _megFileService.Load(megFilePath);
        TestMegModelContent(newMeg, expectedData, true);
    }

    private static void TestMegModelContent(IMegFile meg, ExpectedMegTestData expectedData, bool isNewMeg)
    {
        Assert.NotNull(meg);
        Assert.Equal(expectedData.MegFileCount, meg.Content.Count);
        Assert.Equal(expectedData.IsMegFileVersion, meg.FileInformation.FileVersion);
        Assert.Equal(expectedData.EntryNames.Count, meg.Archive.Count);

        if (isNewMeg)
            Assert.Equal(expectedData.EncryptionData is null, !meg.FileInformation.HasEncryption);
        else
            Assert.Equal(expectedData.IsMegEncrypted, meg.FileInformation.HasEncryption);

        for (var i = 0; i < meg.Archive.Count; i++)
        {
            var entry = meg.Archive[i];
            var expected = expectedData.EntryNames[i];
            Assert.Equal(expected, entry.FilePath);
        }
    }

    private record ExpectedMegTestData
    {
        public MegFileVersion IsMegFileVersion { get; init; }
        public bool IsMegEncrypted { get; init; }

        public int MegFileCount { get; init; }

        public IList<string> EntryNames { get; init; } = null!;


        public string NewMegFilePath { get; init; } = null!;

        public bool NewMegIsBinaryEqual { get; init; }

        public MegFileVersion NewMegFileVersion { get; init; }

        public MegEncryptionData? EncryptionData { get; }
    }

    #region Create Meg Archive

    [Fact]
    public void Test_CreateMegArchive_EntryNotFoundInMeg_Throws()
    {
        const string megFileName = "test.meg";
        const string dummyMegFile = "dummy.meg";
        const string newFileName = "new.meg";

        FileSystem.Initialize()
            .WithFile(megFileName).Which(m => m.HasBytesContent(MegTestConstants.ContentMegFileV1))
            .WithFile(dummyMegFile).Which(m => m.HasBytesContent([0, 0, 0, 0, 0, 0, 0, 0]));

        var meg = _megFileService.Load(megFileName);
       
        var dummyMeg = new MegFile(new MegArchive([]), new MegFileInformation(dummyMegFile, MegFileVersion.V1),
            ServiceProvider);

        var builderInfo = new List<MegFileDataEntryBuilderInfo>
        {
            MegFileDataEntryBuilderInfo.FromEntry(dummyMeg, meg.Archive[0])
        };

        Assert.Throws<FileNotInMegException>(() =>
        {
            using var fs = FileSystem.File.OpenWrite(newFileName);
            _megFileService.CreateMegArchive(fs, meg.FileInformation.FileVersion, null, builderInfo);
        });
    }

    [Fact]
    public void Test_CreateMegArchive_EntryNotFoundOnFileSystem_Throws()
    {
        const string megFileName = "test.meg";
        const string newFileName = "new.meg";

        FileSystem.Initialize().WithFile(megFileName).Which(m => m.HasBytesContent(MegTestConstants.ContentMegFileV1));

        var meg = _megFileService.Load(megFileName);

        var builderInfo = new List<MegFileDataEntryBuilderInfo>
        {
            MegFileDataEntryBuilderInfo.FromFile("notFound.txt", null)
        };
        Assert.Throws<FileNotFoundException>(() =>
        {
            using var fs = FileSystem.File.OpenWrite(newFileName);
            _megFileService.CreateMegArchive(fs, meg.FileInformation.FileVersion, null, builderInfo);
        });
    }
    
    [Fact]
    public void Test_CreateMegArchive_MegWithEntriesOfSameNameButWithDifferentData()
    {
        const string megFileName = "test.meg";

        var expectedBytes = new byte[]
        {
            2, 0, 0, 0, 2, 0, 0, 0, // Header
            4, 0, 102, 105, 108, 101, // "file"
            4, 0, 102, 105, 108, 101, // "file"
            16, 54, 159, 140, 0, 0, 0, 0, 3, 0, 0, 0, 60, 0, 0, 0, 0, 0, 0, 0,
            16, 54, 159, 140, 1, 0, 0, 0, 3, 0, 0, 0, 63, 0, 0, 0, 1, 0, 0, 0,
            49, 50, 51, // 123
            52, 53, 54 // 456
        };


        FileSystem.Initialize().WithFile("1.txt").Which(m => m.HasStringContent("123"));
        FileSystem.Initialize().WithFile("2.txt").Which(m => m.HasStringContent("456"));

        var builderInfo = new List<MegFileDataEntryBuilderInfo>
        {
            MegFileDataEntryBuilderInfo.FromFile("1.txt", "file"),
            MegFileDataEntryBuilderInfo.FromFile("2.txt", "file")
        };

        using (var fs = FileSystem.File.OpenWrite(megFileName))
        {
            _megFileService.CreateMegArchive(fs, MegFileVersion.V1, null, builderInfo);
        }

        var bytes = FileSystem.File.ReadAllBytes(megFileName);

        Assert.Equal(expectedBytes, bytes);
    }

    #endregion

    #region Read / Write V1

    [Fact]
    public void Test_MegV1_WithEntries()
    {
        const string megFileName = "test.meg";

        FileSystem.Initialize().WithFile(megFileName).Which(m => m.HasBytesContent(MegTestConstants.ContentMegFileV1));

        var expectedData = new ExpectedMegTestData
        {
            IsMegFileVersion = MegFileVersion.V1,
            IsMegEncrypted = false,
            MegFileCount = 2,
            EntryNames = new List<string>
            {
                "DATA\\XML\\CAMPAIGNFILES.XML",
                "DATA\\XML\\GAMEOBJECTFILES.XML"
            },
            NewMegFilePath = "new.meg",
            NewMegFileVersion = MegFileVersion.V1,
            NewMegIsBinaryEqual = true
        };
        TestMegFiles(megFileName, expectedData);
    }

    [Fact]
    public void Test_MegV1_Empty()
    {
        const string megFileName = "test.meg";
        const string megResource = "Files.v1_empty.meg";

        FileSystem.Initialize().WithFile(megFileName)
            .Which(m => m.HasBytesContent(TestUtility.GetEmbeddedResourceAsByteArray(GetType(), megResource)));

        var expectedData = new ExpectedMegTestData
        {
            IsMegFileVersion = MegFileVersion.V1,
            IsMegEncrypted = false,
            MegFileCount = 0,
            EntryNames = new List<string>(),
            NewMegFilePath = "new.meg",
            NewMegFileVersion = MegFileVersion.V1,
            NewMegIsBinaryEqual = true
        };
        TestMegFiles(megFileName, expectedData);
    }

    [Fact]
    public void Test_MegV1_EntriesHaveNonAsciiNames()
    {
        const string megFileName = "test.meg";
        const string megResource = "Files.v1_2_files_with_extended_ascii_name.meg";

        FileSystem.Initialize().WithFile(megFileName)
            .Which(m => m.HasBytesContent(TestUtility.GetEmbeddedResourceAsByteArray(GetType(), megResource)));

        var expectedData = new ExpectedMegTestData
        {
            IsMegFileVersion = MegFileVersion.V1,
            IsMegEncrypted = false,
            MegFileCount = 2,
            EntryNames = new List<string>
            {
                "TEST?.TXT",
                "TEST?.TXT"
            },
            NewMegFilePath = "new.meg",
            NewMegFileVersion = MegFileVersion.V1,
            NewMegIsBinaryEqual = false
        };

        TestMegFiles(megFileName, expectedData);
    }

    #endregion


    // TODO: Need to test V3 Encrypted!
}