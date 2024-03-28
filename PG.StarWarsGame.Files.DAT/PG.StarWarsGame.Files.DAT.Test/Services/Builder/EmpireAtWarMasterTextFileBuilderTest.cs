using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services.Builder;

public class EmpireAtWarMasterTextFileBuilderTest
{
    private readonly MockFileSystem _fileSystem = new();

    private IServiceProvider CreateServiceProvider()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_ => _fileSystem);
        DatDomain.RegisterServices(sc);
        return sc.BuildServiceProvider();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Ctor(bool overwrite)
    {
        var builder = new EmpireAtWarMasterTextFileBuilder(overwrite, CreateServiceProvider());

        Assert.Equal(overwrite ? BuilderOverrideKind.Overwrite : BuilderOverrideKind.NoOverwrite, builder.KeyOverwriteBehavior);
        Assert.Equal(DatFileType.OrderedByCrc32, builder.TargetKeySortOrder);
        Assert.IsType<EmpireAtWarKeyValidator>(builder.KeyValidator);
    }
}