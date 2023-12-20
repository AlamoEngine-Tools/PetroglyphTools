using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using PG.Testing;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

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
        sc.AddSingleton<IChecksumService>(new ChecksumService());
        MegDomain.RegisterServices(sc);
        var sp = sc.BuildServiceProvider();
        _megFileService = sp.GetRequiredService<IMegFileService>();
    }
    
    [TestMethod]
    public void Test_CreateMegArchive_EntryNotFoundInMeg_Throws()
    {
        const string megFileName = "/test.meg";
        const string newFileName = "/new.meg";

        _fileSystem.AddFile(megFileName, new MockFileData(MegTestConstants.CONTENT_MEG_FILE_V1));
        _fileSystem.AddFile(megFileName, new MockFileData(MegTestConstants.CONTENT_MEG_FILE_V1));

        var meg = _megFileService.Load(megFileName);
        var dummyMeg = new Mock<IMegFile>();
        dummyMeg.SetupGet(m => m.Archive).Returns(new MegArchive(new List<MegDataEntry>()));

        var builderInfo = new List<MegFileDataEntryBuilderInfo>
        {
            MegFileDataEntryBuilderInfo.FromEntry(dummyMeg.Object, meg.Archive[0])
        };

        Assert.ThrowsException<FileNotInMegException>(() => _megFileService.CreateMegArchive(
            new MegFileHolderParam { FilePath = newFileName, FileVersion = meg.FileVersion },
            builderInfo,
            false));
    }

    [TestMethod]
    public void Test_CreateMegArchive_EntryNotFoundOnFileSystem_Throws()
    {
        const string megFileName = "/test.meg";
        const string newFileName = "/new.meg";

        _fileSystem.AddFile(megFileName, new MockFileData(MegTestConstants.CONTENT_MEG_FILE_V1));
        _fileSystem.AddFile(megFileName, new MockFileData(MegTestConstants.CONTENT_MEG_FILE_V1));

        var meg = _megFileService.Load(megFileName);

        var builderInfo = new List<MegFileDataEntryBuilderInfo>
        {
            MegFileDataEntryBuilderInfo.FromFile("notFound.txt", null)
        };

        Assert.ThrowsException<FileNotFoundException>(() => _megFileService.CreateMegArchive(
            new MegFileHolderParam { FilePath = newFileName, FileVersion = meg.FileVersion },
            builderInfo,
            false));
    }

    [TestMethod]
    public void Test_MegV1_WithEntries()
    {
        const string megFileName = "/test.meg";

        _fileSystem.AddFile(megFileName, new MockFileData(MegTestConstants.CONTENT_MEG_FILE_V1));

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

        _fileSystem.AddFile(megFileName, new MockFileData(TestUtility.GetEmbeddedResourceAsByteArray(GetType(), megResource)));

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

        _fileSystem.AddFile(megFileName, new MockFileData(TestUtility.GetEmbeddedResourceAsByteArray(GetType(), megResource)));

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

    
    [TestMethod]
    public void Test_CreateMegArchive_RealFileSystem_OverrideCurrentMeg()
    {
        // Relative paths are OK for this test.
        const string megFileName = "test.meg";
        const string copyMeg = "copy.meg";

        // This Test does not use the MockFileSystem but the actual FileSystem
        var fs = new FileSystem();

        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(fs);
        sc.AddSingleton<IChecksumService>(new ChecksumService());
        MegDomain.RegisterServices(sc);

        var megFileService = new MegFileService(sc.BuildServiceProvider());

        try
        {
            fs.File.WriteAllBytes(megFileName, MegTestConstants.CONTENT_MEG_FILE_V1);
            fs.File.WriteAllBytes(copyMeg, MegTestConstants.CONTENT_MEG_FILE_V1);

            var meg = megFileService.Load(megFileName);

            megFileService.CreateMegArchive(
                new MegFileHolderParam { FilePath = meg.FilePath, FileVersion = meg.FileVersion },
                meg.Archive.Select(e => MegFileDataEntryBuilderInfo.FromEntry(meg, e)),
                true);

            var actualBytes = fs.File.ReadAllBytes(megFileName);
            var expectedBytes = fs.File.ReadAllBytes(copyMeg);

            CollectionAssert.AreEqual(expectedBytes, actualBytes);
        }
        finally
        {
            // Make sure to delete the test files again.
            try
            {
                fs.File.Delete(megFileName);
            }
            catch
            {
                // Ignore
            }
            try
            {
                fs.File.Delete(copyMeg);
            }
            catch
            { 
                // Ignore
            }
        }
    }

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

        using var param = new MegFileHolderParam
        {
            FilePath = expectedData.NewMegFilePath,
            FileVersion = expectedData.NewMegFileVersion,
            Key = expectedData.NewKey,
            IV = expectedData.NewIV
        };

        var builderInformation = meg.Archive.Select(e =>
            new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(new MegDataEntryLocationReference(meg, e))));

        _megFileService.CreateMegArchive(param, builderInformation, false);

        Assert.IsTrue(_fileSystem.FileExists(expectedData.NewMegFilePath));

        var createdVersion = _megFileService.GetMegFileVersion(expectedData.NewMegFilePath, out var newEncrypted);
        Assert.AreEqual(expectedData.NewMegFileVersion, createdVersion);
        Assert.AreEqual(expectedData.NewKey is null, !newEncrypted);


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
        Assert.AreEqual(expectedData.IsMegFileVersion, meg.FileVersion);
        Assert.AreEqual(expectedData.EntryNames.Count, meg.Archive.Count);

        if (isNewMeg)
            Assert.AreEqual(expectedData.NewKey is null, !meg.HasEncryption);
        else
            Assert.AreEqual(expectedData.IsMegEncrypted, meg.HasEncryption);

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

        public IList<string> EntryNames { get; init; }

        
        public string NewMegFilePath { get; init; }

        public bool NewMegIsBinaryEqual { get; init; }

        public MegFileVersion NewMegFileVersion { get; init; }

        public byte[]? NewKey { get; init; }

        public byte[]? NewIV { get; init; }
    }


    // TODO: Need to test V3 Encrypted!
}