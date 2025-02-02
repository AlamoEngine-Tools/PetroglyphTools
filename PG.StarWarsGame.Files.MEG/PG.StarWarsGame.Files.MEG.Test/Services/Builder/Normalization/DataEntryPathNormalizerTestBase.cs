using System;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Normalization;

public abstract class DataEntryPathNormalizerTestBase
{
    protected abstract IMegDataEntryPathNormalizer CreateNormalizer();

    [Fact]
    public void TestTryNormalizePathFails()
    {
        var normalizer = CreateNormalizer();

        Span<char> tooShortBuffer = new char[5];

        Assert.False(normalizer.TryNormalize("TooLongPath".AsSpan(), tooShortBuffer, out _));
    }

    protected void TestNormalizePathPasses(string source, string expected)
    {
        var normalizer = CreateNormalizer();

        TestNormalizePass(normalizer, source, expected);
        TestNormalizePassSpan(normalizer, source, expected);
    }

    private void TestNormalizePassSpan(IMegDataEntryPathNormalizer normalizer, string source, string expected)
    {
        Span<char> buffer = new char[source.AsSpan().Length * 2 + 1];
        
        var success = normalizer.TryNormalize(source.AsSpan(), buffer, out var length);
        Assert.True(success);
        Assert.Equal(expected, buffer.Slice(0, length).ToString());
    }

    private void TestNormalizePass(IMegDataEntryPathNormalizer normalizer, string source, string expected)
    {
        var actual = normalizer.Normalize(source);
        Assert.Equal(expected, actual);

        actual = normalizer.Normalize(source.AsSpan());
        Assert.Equal(expected, actual);
    }
}