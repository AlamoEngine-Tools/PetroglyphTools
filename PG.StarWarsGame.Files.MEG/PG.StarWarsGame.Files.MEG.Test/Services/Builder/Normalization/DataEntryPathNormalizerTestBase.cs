using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.Testing;
using Testably.Abstractions.Testing;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Normalization;

public abstract class DataEntryPathNormalizerTestBase
{
    protected abstract IMegDataEntryPathNormalizer CreateNormalizer(IServiceProvider serviceProvider);

    protected void TestNormalizePathFails(string source)
    {
        var normalizer = CreateNormalizer(CreateServiceProvider());

        ExceptionUtilities.AssertThrowsAny(() => normalizer.Normalize(source));

        var copy = source;
        var success = normalizer.TryNormalize(ref copy, out _);
        Assert.IsFalse(success);
    }

    protected void TestNormalizePathPasses(string source, string expected)
    {
        var normalizer = CreateNormalizer(CreateServiceProvider());

        var actual = normalizer.Normalize(source);
        Assert.AreEqual(expected, actual);

        var copy = source;
        var success = normalizer.TryNormalize(ref copy, out _);
        Assert.IsTrue(success);
        Assert.AreEqual(copy, expected);
    }

    private IServiceProvider CreateServiceProvider()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        return sc.BuildServiceProvider();
    }
}