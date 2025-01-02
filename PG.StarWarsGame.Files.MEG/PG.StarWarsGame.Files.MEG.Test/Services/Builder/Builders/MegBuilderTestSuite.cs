using System;
using System.IO.Abstractions;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

public abstract class MegBuilderTestSuite
{
    protected MockFileSystem FileSystem { get; } = new();

    protected abstract Type ExpectedFileInfoValidatorType { get; }
    protected abstract Type ExpectedDataEntryValidatorType { get; }
    protected abstract Type? ExpectedDataEntryPathNormalizerType { get; }
    protected abstract bool? ExpectedOverwritesDuplicates { get; }
    protected abstract bool? ExpectedAutomaticallyAddFileSizes { get; }

    protected abstract MegBuilderBase CreateBuilder(IServiceProvider serviceProvider);

    protected virtual void SetupServiceCollection(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IFileSystem>(FileSystem);
        serviceCollection.AddSingleton<IHashingService>(sp => new HashingService(sp));
        PetroglyphCommons.ContributeServices(serviceCollection);
        serviceCollection.SupportMEG();
    }

    protected IServiceProvider CreateServiceProvider()
    {
        var sc = new ServiceCollection();
        SetupServiceCollection(sc);
        return sc.BuildServiceProvider();
    }

    [Fact]
    public void MegBuilderTestSuite_Test_Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => CreateBuilder(null!));
    }

    [Fact]
    public void MegBuilderTestSuite_Test_Ctor()
    {
        var builder = CreateBuilder(CreateServiceProvider());

        Assert.Equal(ExpectedFileInfoValidatorType, builder.MegFileInformationValidator.GetType());
        Assert.Equal(ExpectedDataEntryValidatorType, builder.DataEntryValidator.GetType());
        Assert.Equal(ExpectedDataEntryPathNormalizerType, builder.DataEntryPathNormalizer?.GetType());
        Assert.Equal(ExpectedOverwritesDuplicates, builder.OverwritesDuplicateEntries);
        Assert.Equal(ExpectedAutomaticallyAddFileSizes, builder.AutomaticallyAddFileSizes);
    }
}