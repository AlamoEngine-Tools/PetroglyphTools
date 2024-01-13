using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.Testing;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Normalization;

public abstract class DataEntryPathNormalizerTestBase
{
    protected abstract IMegDataEntryPathNormalizer CreateNormalizer();

    public void TestNormalizePathFails(string source)
    {
        var normalizer = CreateNormalizer();

        ExceptionUtilities.AssertThrowsAny(() => normalizer.NormalizePath(source));

        var copy = source;
        var success = normalizer.TryNormalizePath(ref copy, out _);
        Assert.IsFalse(success);
    }

    public void TestNormalizePathPasses(string source, string expected)
    {
        var normalizer = CreateNormalizer();

        var actual = normalizer.NormalizePath(source);
        Assert.AreEqual(expected, actual);

        var copy = source;
        var success = normalizer.TryNormalizePath(ref copy, out _);
        Assert.IsTrue(success);
        Assert.AreEqual(copy, expected);
    }
}