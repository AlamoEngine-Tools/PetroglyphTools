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
    private readonly IServiceCollection _serviceCollection = new ServiceCollection();
    private readonly MockFileSystem _fileSystem = new();

    [TestInitialize]
    public void Init()
    {
        _serviceCollection.AddSingleton<IFileSystem>(_fileSystem);
        MegDomain.RegisterServices(_serviceCollection);
    }

    [TestMethod]
    public void Test_Load_MegWithFiles_V1()
    {
        var sp = _serviceCollection.BuildServiceProvider();

        var service = sp.GetRequiredService<IMegFileService>();

        _fileSystem.AddFile("test.meg", new MockFileData(MegTestConstants.CONTENT_MEG_FILE_V1));

        var meg = service.Load("test.meg");

        Assert.IsNotNull(meg);
        Assert.AreEqual(2, meg.Content.Count);
        Assert.AreEqual(MegFileVersion.V1, meg.FileVersion);
    }

    [TestMethod]
    public void Test_Load_MegWithFiles_V1_NonAsciiDoesNotCrash()
    {
        var sp = _serviceCollection.BuildServiceProvider();

        var service = sp.GetRequiredService<IMegFileService>();

        using var megFileData = TestUtility.GetEmbeddedResource(GetType(), "Entries.v1_2_files_with_extended_ascii_name.meg");
        using var ms = new MemoryStream();
        megFileData.CopyTo(ms);

        _fileSystem.AddFile("test.meg", new MockFileData(ms.ToArray()));

        var meg = service.Load("test.meg");

        Assert.IsNotNull(meg);
        Assert.AreEqual(2, meg.Content.Count);
        Assert.AreEqual(MegFileVersion.V1, meg.FileVersion);
    }


    // TODO: Need to test V3 Encrypted!
}