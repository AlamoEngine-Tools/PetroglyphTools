using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Normalization;

[TestClass]
public class PetroglyphDataEntryPathNormalizerTest : DataEntryPathNormalizerTestBase
{
    protected override IMegDataEntryPathNormalizer CreateNormalizer()
    {
        return new PetroglyphDataEntryPathNormalizer();
    }

    [TestMethod]
    [DynamicData(nameof(ValidPathsToNormalize), DynamicDataSourceType.Method)]
    public void Test_Normalize_Success(string source, string expected)
    {
        TestNormalizePathPasses(source, expected);
    }

    [TestMethod]
    [DynamicData(nameof(InvalidPathsToNormalize), DynamicDataSourceType.Method)]
    public void Test_Normalize_Fails(string source)
    {
        TestNormalizePathFails(source);
    }


    public static IEnumerable<object[]> ValidPathsToNormalize()
    {
        yield return ["file", "FILE"];
        yield return ["fiLE.TxT", "FILE.TXT"];
    }

    public static IEnumerable<object[]> InvalidPathsToNormalize()
    {
        // no directories
        yield return ["file/"];

        // No path operators
        yield return ["./file"];
        yield return ["../file"];

        // Only valid characters
        yield return ["fileÖ"];
        yield return ["file?"];
        yield return ["file*"];
    }
}