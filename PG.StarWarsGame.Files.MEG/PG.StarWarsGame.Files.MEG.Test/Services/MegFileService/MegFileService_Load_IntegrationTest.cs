using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        MegDomain.RegisterServices(sc);
        var sp = sc.BuildServiceProvider();
        _megFileService = sp.GetRequiredService<IMegFileService>();
    }



    [TestMethod]
    public void Test_MegV1_WithEntries()
    {
        const string megFileName = "test.meg";

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
            }
        };

        TestMegFiles(megFileName, expectedData);
    }

    [TestMethod]
    public void Test_MegV1_EntriesHaveNonAsciiNames()
    {
        const string megFileName = "test.meg";

        using var megFileData = TestUtility.GetEmbeddedResource(GetType(), "Files.v1_2_files_with_extended_ascii_name.meg");
        using var ms = new MemoryStream();
        megFileData.CopyTo(ms);

        _fileSystem.AddFile(megFileName, new MockFileData(ms.ToArray()));


        var expectedData = new ExpectedMegTestData
        {
            IsMegFileVersion = MegFileVersion.V1,
            IsMegEncrypted = false,
            MegFileCount = 2,
            EntryNames = new List<string>
            {
                "TEST?.TXT",
                "TEST?.TXT"
            }
        };

        TestMegFiles(megFileName, expectedData);
    }


    private void TestMegFiles(string megFilePath, ExpectedMegTestData expectedData)
    {
        var megVersion = _megFileService.GetMegFileVersion(megFilePath, out var encrypted);
        Assert.AreEqual(expectedData.IsMegFileVersion, megVersion);
        Assert.AreEqual(expectedData.IsMegEncrypted, encrypted);

        var meg = _megFileService.Load(megFilePath);
        Assert.IsNotNull(meg);
        Assert.AreEqual(expectedData.MegFileCount, meg.Content.Count);
        Assert.AreEqual(expectedData.IsMegFileVersion, meg.FileVersion);
        Assert.AreEqual(expectedData.IsMegEncrypted, meg.HasEncryption);
        Assert.AreEqual(expectedData.EntryNames.Count, meg.Archive.Count);

        for (int i = 0; i < meg.Archive.Count; i++)
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
    }


    // TODO: Need to test V3 Encrypted!
}