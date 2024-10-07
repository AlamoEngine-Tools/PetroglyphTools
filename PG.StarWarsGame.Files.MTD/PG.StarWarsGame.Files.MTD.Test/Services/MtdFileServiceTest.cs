using System;
using System.IO.Abstractions;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons;
using PG.StarWarsGame.Files.MTD.Services;
using Testably.Abstractions.Testing;

namespace PG.StarWarsGame.Files.MTD.Test.Services;

public partial class MtdFileServiceTest
{
    private readonly MtdFileService _mtdFileService;
    private readonly IServiceProvider _serviceProvider;
    private readonly MockFileSystem _fileSystem = new();

    public MtdFileServiceTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        sc.AddMtdServices();
        new PGServiceContribution().ContributeServices(sc);

        
        _serviceProvider = sc.BuildServiceProvider();
        _mtdFileService = new MtdFileService(_serviceProvider);
    }
}