using System;
using System.IO.Abstractions;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Extensibility;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services.Builder;

public class EmpireAtWarCreditsTextFileBuilderTest
{
    private readonly MockFileSystem _fileSystem = new();

    private IServiceProvider CreateServiceProvider()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_ => _fileSystem);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        sc.CollectPgServiceContributions();
        return sc.BuildServiceProvider();
    }

    [Fact]
    public void Test_Ctor()
    {
        var builder = new EmpireAtWarCreditsTextFileBuilder(CreateServiceProvider());
        
        Assert.Equal(BuilderOverrideKind.AllowDuplicate,builder.KeyOverwriteBehavior);
        Assert.Equal(DatFileType.NotOrdered, builder.TargetKeySortOrder);
        Assert.IsType<EmpireAtWarKeyValidator>(builder.KeyValidator);
    }
}