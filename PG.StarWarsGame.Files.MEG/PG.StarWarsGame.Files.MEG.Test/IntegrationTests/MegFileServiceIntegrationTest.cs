using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using PG.Testing;
using Testably.Abstractions.Testing;

namespace PG.StarWarsGame.Files.MEG.Test.IntegrationTests;

[TestClass]
public class MegFileServiceIntegrationTest
{
    private readonly MockFileSystem _fileSystem = new();
    private IMegFileService _megFileService = null!;

    [TestInitialize]
    public void Init()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        PGDomain.RegisterServices(sc);
        MegDomain.RegisterServices(sc);
        var sp = sc.BuildServiceProvider();
        _megFileService = sp.GetRequiredService<IMegFileService>();
    }

    #region Create Meg Archive

    [TestMethod]
    public void Test_CreateMegArchive_EntryNotFoundInMeg_Throws()
    {
        const string megFileName = "/test.meg";
        const string newFileName = "/new.meg";

        _fileSystem.Initialize().WithFile(megFileName).Which(m => m.HasBytesContent(MegTestConstants.ContentMegFileV1));

        var meg = _megFileService.Load(megFileName);
        var dummyMeg = new Mock<IMegFile>();
        dummyMeg.SetupGet(m => m.Archive).Returns(new MegArchive(new List<MegDataEntry>()));

        var builderInfo = new List<MegFileDataEntryBuilderInfo>
        {
            MegFileDataEntryBuilderInfo.FromEntry(dummyMeg.Object, meg.Archive[0])
        };

        Assert.ThrowsException<FileNotInMegException>(() =>
        {
            using var fs = _fileSystem.File.OpenWrite(newFileName);
            _megFileService.CreateMegArchive(fs, meg.FileInformation.FileVersion, null, builderInfo);
        });
    }

    [TestMethod]
    public void Test_CreateMegArchive_EntryNotFoundOnFileSystem_Throws()
    {
        const string megFileName = "/test.meg";
        const string newFileName = "/new.meg";

        _fileSystem.Initialize().WithFile(megFileName).Which(m => m.HasBytesContent(MegTestConstants.ContentMegFileV1));

        var meg = _megFileService.Load(megFileName);

        var builderInfo = new List<MegFileDataEntryBuilderInfo>
        {
            MegFileDataEntryBuilderInfo.FromFile("notFound.txt", null)
        };
        Assert.ThrowsException<FileNotFoundException>(() =>
        {
            using var fs = _fileSystem.File.OpenWrite(newFileName);
            _megFileService.CreateMegArchive(fs, meg.FileInformation.FileVersion, null, builderInfo);
        });
    }
    
    [TestMethod]
    public void Test_CreateMegArchive_MegWithEntriesOfSameNameButWithDifferentData()
    {
        const string megFileName = "/test.meg";

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


        _fileSystem.Initialize().WithFile("1.txt").Which(m => m.HasStringContent("123"));
        _fileSystem.Initialize().WithFile("2.txt").Which(m => m.HasStringContent("456"));

        var builderInfo = new List<MegFileDataEntryBuilderInfo>
        {
            MegFileDataEntryBuilderInfo.FromFile("1.txt", "file"),
            MegFileDataEntryBuilderInfo.FromFile("2.txt", "file")
        };

        using (var fs = _fileSystem.File.OpenWrite(megFileName))
        {
            _megFileService.CreateMegArchive(fs, MegFileVersion.V1, null, builderInfo);
        }

        var bytes = _fileSystem.File.ReadAllBytes(megFileName);

        CollectionAssert.AreEqual(expectedBytes, bytes);
    }

    #endregion

    #region Read / Write V1

    [TestMethod]
    public void Test_MegV1_WithEntries()
    {
        const string megFileName = "/test.meg";

        _fileSystem.Initialize().WithFile(megFileName).Which(m => m.HasBytesContent(MegTestConstants.ContentMegFileV1));

        var expectedData = new ExpectedMegTestData
        {
            IsMegFileVersion = MegFileVersion.V1,
            IsMegEncrypted = false,
            MegFileCount = 2,
            EntryNames = new List<string>
            {
                "DATA/XML/GAMEOBJECTFILES.XML",
                "DATA/XML/CAMPAIGNFILES.XML"
            },
            NewMegFilePath = "/new.meg",
            NewMegFileVersion = MegFileVersion.V1,
            NewMegIsBinaryEqual = true
        };
        TestMegFiles(megFileName, expectedData);
    }

    [TestMethod]
    public void Test_MegV1_Empty()
    {
        const string megFileName = "/test.meg";
        const string megResource = "Files.v1_empty.meg";

        _fileSystem.Initialize().WithFile(megFileName)
            .Which(m => m.HasBytesContent(TestUtility.GetEmbeddedResourceAsByteArray(GetType(), megResource)));

        var expectedData = new ExpectedMegTestData
        {
            IsMegFileVersion = MegFileVersion.V1,
            IsMegEncrypted = false,
            MegFileCount = 0,
            EntryNames = new List<string>(),
            NewMegFilePath = "/new.meg",
            NewMegFileVersion = MegFileVersion.V1,
            NewMegIsBinaryEqual = true
        };
        TestMegFiles(megFileName, expectedData);
    }

    [TestMethod]
    public void Test_MegV1_EntriesHaveNonAsciiNames()
    {
        const string megFileName = "/test.meg";
        const string megResource = "Files.v1_2_files_with_extended_ascii_name.meg";

        _fileSystem.Initialize().WithFile(megFileName)
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
            NewMegFilePath = "/new.meg",
            NewMegFileVersion = MegFileVersion.V1,
            NewMegIsBinaryEqual = false
        };

        TestMegFiles(megFileName, expectedData);
    }

    #endregion

    private void TestMegFiles(string megFilePath, ExpectedMegTestData expectedData)
    {
        var megVersion = _megFileService.GetMegFileVersion(megFilePath, out var encrypted);
        Assert.AreEqual(expectedData.IsMegFileVersion, megVersion);
        Assert.AreEqual(expectedData.IsMegEncrypted, encrypted);

        var meg = _megFileService.Load(megFilePath);
        TestMegModelContent(meg, expectedData, false);

        for (var i = 0; i < meg.Archive.Count; i++)
        {
            var entry = meg.Archive[i];
            var expected = expectedData.EntryNames[i];
            Assert.AreEqual(expected, entry.FilePath);
        }

        using var param = new MegFileInformation(
            expectedData.NewMegFilePath,
            expectedData.NewMegFileVersion,
            expectedData.EncryptionData);

        var builderInformation = meg.Archive.Select(e =>
            new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(new MegDataEntryLocationReference(meg, e))));

        using (var fs = _fileSystem.File.OpenWrite(expectedData.NewMegFilePath))
        {
            _megFileService.CreateMegArchive(fs, expectedData.NewMegFileVersion, expectedData.EncryptionData, builderInformation);
        }

        Assert.IsTrue(_fileSystem.File.Exists(expectedData.NewMegFilePath));

        var createdVersion = _megFileService.GetMegFileVersion(expectedData.NewMegFilePath, out var newEncrypted);
        Assert.AreEqual(expectedData.NewMegFileVersion, createdVersion);
        Assert.AreEqual(expectedData.EncryptionData is null, !newEncrypted);


        var actualBytes = _fileSystem.File.ReadAllBytes(expectedData.NewMegFilePath);
        var expectedBytes = _fileSystem.File.ReadAllBytes(megFilePath);
        if (expectedData.NewMegIsBinaryEqual)
            CollectionAssert.AreEqual(expectedBytes, actualBytes);
        else
            CollectionAssert.AreNotEqual(expectedBytes, actualBytes);

        var newMeg = _megFileService.Load(megFilePath);
        TestMegModelContent(newMeg, expectedData, true);
    }

    private static void TestMegModelContent(IMegFile meg, ExpectedMegTestData expectedData, bool isNewMeg)
    {
        Assert.IsNotNull(meg);
        Assert.AreEqual(expectedData.MegFileCount, meg.Content.Count);
        Assert.AreEqual(expectedData.IsMegFileVersion, meg.FileInformation.FileVersion);
        Assert.AreEqual(expectedData.EntryNames.Count, meg.Archive.Count);

        if (isNewMeg)
            Assert.AreEqual(expectedData.EncryptionData is null, !meg.FileInformation.HasEncryption);
        else
            Assert.AreEqual(expectedData.IsMegEncrypted, meg.FileInformation.HasEncryption);

        for (var i = 0; i < meg.Archive.Count; i++)
        {
            var entry = meg.Archive[i];
            var expected = expectedData.EntryNames[i];
            Assert.AreEqual(expected, entry.FilePath);
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

        public MegEncryptionData? EncryptionData { get; init; }
    }


    // TODO: Need to test V3 Encrypted!
}