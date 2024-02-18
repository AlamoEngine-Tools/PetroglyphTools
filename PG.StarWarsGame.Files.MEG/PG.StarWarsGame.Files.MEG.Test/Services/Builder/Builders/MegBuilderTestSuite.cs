using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using Testably.Abstractions.Testing;

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
    }

    protected IServiceProvider CreateServiceProvider()
    {
        var sc = new ServiceCollection();
        SetupServiceCollection(sc);
        return sc.BuildServiceProvider();
    }

    [TestMethod]
    public void MegBuilderTestSuite_Test_Ctor_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() => CreateBuilder(null!));
    }

    [TestMethod]
    public void MegBuilderTestSuite_Test_Ctor()
    {
        var builder = CreateBuilder(CreateServiceProvider());

        Assert.AreEqual(ExpectedFileInfoValidatorType, builder.MegFileInformationValidator.GetType());
        Assert.AreEqual(ExpectedDataEntryValidatorType, builder.DataEntryValidator.GetType());
        Assert.AreEqual(ExpectedDataEntryPathNormalizerType, builder.DataEntryPathNormalizer?.GetType());
        Assert.AreEqual(ExpectedOverwritesDuplicates, builder.OverwritesDuplicateEntries);
        Assert.AreEqual(ExpectedAutomaticallyAddFileSizes, builder.AutomaticallyAddFileSizes);
    }
}