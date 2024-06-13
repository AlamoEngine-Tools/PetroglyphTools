using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Normalization;

public abstract class DataEntryPathNormalizerTestBase
{
    protected abstract IMegDataEntryPathNormalizer CreateNormalizer(IServiceProvider serviceProvider);

    protected void TestNormalizePathFails(string source)
    {
        var normalizer = CreateNormalizer(CreateServiceProvider());

        var buffer = new char[source?.Length ?? 0];
        Assert.ThrowsAny<Exception>(() => normalizer.Normalize(source));
        Assert.ThrowsAny<Exception>(() => normalizer.Normalize(source.AsSpan(), buffer));

        var copy = source;
        Assert.False(normalizer.TryNormalize(ref copy, out _));
        Assert.False(normalizer.TryNormalize(source.AsSpan(), buffer, out _, out _));
    }

    protected void TestNormalizePathPasses(string source, string expected)
    {
        var normalizer = CreateNormalizer(CreateServiceProvider());

        Test_Normalize_Pass(normalizer, source, expected);
        Test_Normalize_Pass_Span(normalizer, source, expected);
    }

    private void Test_Normalize_Pass_Span(IMegDataEntryPathNormalizer normalizer, string source, string expected)
    {
        var buffer = new char[source.Length];
        var length = normalizer.Normalize(source.AsSpan(), buffer);
        Assert.Equal(expected, buffer.AsSpan().Slice(0, length).ToString());

        var success = normalizer.TryNormalize(source.AsSpan(), buffer, out length, out _);
        Assert.True(success);
        Assert.Equal(expected, buffer.AsSpan().Slice(0, length).ToString());
    }

    private void Test_Normalize_Pass(IMegDataEntryPathNormalizer normalizer, string source, string expected)
    {
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