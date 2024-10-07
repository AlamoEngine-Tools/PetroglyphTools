using System.IO.Abstractions;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons;
using PG.StarWarsGame.Files.MTD.Services;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MTD.Test.Services;

public class MtdFileServiceIntegrationTest
{
    private readonly IMtdFileService _mtdFileService;
    private readonly MockFileSystem _fileSystem = new();

    public MtdFileServiceIntegrationTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        sc.AddMtdServices();
        new PGServiceContribution().ContributeServices(sc);


        var sp = sc.BuildServiceProvider();
        _mtdFileService = sp.GetRequiredService<IMtdFileService>();
    }

    [Fact]
    public void Test_Load_FocMtd()
    {
        var focFile = TestUtility.GetEmbeddedResource(typeof(MtdFileServiceIntegrationTest), "Files.MT_COMMANDBAR.MTD");
        ExceptionUtilities.AssertDoesNotThrowException(() => _mtdFileService.Load(new TestMegDataStream("MT_COMMANDBAR.MTD", focFile)));
    }
}