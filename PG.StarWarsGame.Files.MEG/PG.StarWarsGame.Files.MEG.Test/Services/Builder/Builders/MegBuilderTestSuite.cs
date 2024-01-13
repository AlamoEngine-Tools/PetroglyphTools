using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Services.Builder;

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

    protected virtual IServiceProvider CreateServiceProvider()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(FileSystem);
        return sc.BuildServiceProvider();
    }

    [TestMethod]
    public void Test_Ctor_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() => CreateBuilder(null!));
    }

    [TestMethod]
    public void Test_Ctor()
    {
        var builder = CreateBuilder(CreateServiceProvider());

        Assert.AreEqual(ExpectedFileInfoValidatorType, builder.FileInformationValidator.GetType());
        Assert.AreEqual(ExpectedDataEntryValidatorType, builder.DataEntryValidator.GetType());
        Assert.AreEqual(ExpectedDataEntryPathNormalizerType, builder.DataEntryPathNormalizer?.GetType());
        Assert.AreEqual(ExpectedOverwritesDuplicates, builder.OverwritesDuplicateEntries);
        Assert.AreEqual(ExpectedAutomaticallyAddFileSizes, builder.AutomaticallyAddFileSizes);
    }
}