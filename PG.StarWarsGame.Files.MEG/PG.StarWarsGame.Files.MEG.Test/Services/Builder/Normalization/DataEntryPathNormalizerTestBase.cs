using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Normalization;

public abstract class DataEntryPathNormalizerTestBase
{
    protected abstract IMegDataEntryPathNormalizer CreateNormalizer(IServiceProvider serviceProvider);

    [Fact]
    public void TestTryNormalizePathFails()
    {
        var normalizer = CreateNormalizer(CreateServiceProvider());

        Span<char> tooShortBuffer = new char[5];

        Assert.False(normalizer.TryNormalize("TooLongPath".AsSpan(), tooShortBuffer, out _));
    }

    protected void TestNormalizePathPasses(string source, string expected)
    {
        var normalizer = CreateNormalizer(CreateServiceProvider());

        Test_Normalize_Pass(normalizer, source, expected);
        Test_Normalize_Pass_Span(normalizer, source, expected);
    }

    private void Test_Normalize_Pass_Span(IMegDataEntryPathNormalizer normalizer, string source, string expected)
    {
        Span<char> buffer = new char[source.Length * 2 + 1];

        var sb = new ValueStringBuilder();
        
        normalizer.Normalize(source.AsSpan(), ref sb);
        Assert.Equal(expected, sb.ToString());

        var success = normalizer.TryNormalize(source.AsSpan(), buffer, out var length);
        Assert.True(success);
        Assert.Equal(expected, buffer.Slice(0, length).ToString());
    }

    private void Test_Normalize_Pass(IMegDataEntryPathNormalizer normalizer, string source, string expected)
    {
        var actual = normalizer.Normalize(source);
        Assert.Equal(expected, actual);
    }

    private IServiceProvider CreateServiceProvider()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        return sc.BuildServiceProvider();
    }
}