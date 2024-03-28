using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

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
        Assert.False(success);
    }

    protected void TestNormalizePathPasses(string source, string expected)
    {
        var normalizer = CreateNormalizer(CreateServiceProvider());

        var actual = normalizer.Normalize(source);
        Assert.Equal(expected, actual);

        var copy = source;
        var success = normalizer.TryNormalize(ref copy, out _);
        Assert.True(success);
        Assert.Equal(copy, expected);
    }

    private IServiceProvider CreateServiceProvider()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        return sc.BuildServiceProvider();
    }
}