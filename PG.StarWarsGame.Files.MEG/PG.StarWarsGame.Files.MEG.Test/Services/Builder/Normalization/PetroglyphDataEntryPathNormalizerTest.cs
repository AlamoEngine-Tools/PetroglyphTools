using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Normalization;

[TestClass]
public class PetroglyphDataEntryPathNormalizerTest : DataEntryPathNormalizerTestBase
{
    protected override IMegDataEntryPathNormalizer CreateNormalizer(IServiceProvider serviceProvider)
    {
        return new PetroglyphDataEntryPathNormalizer(serviceProvider);
    }

    [TestMethod]
    [DynamicData(nameof(ValidPathsToNormalize), DynamicDataSourceType.Method)]
    public void Test_Normalize_Success(string source, string expected)
    {
        TestNormalizePathPasses(source, expected);
    }

    [TestMethod]
    [DynamicData(nameof(DefaultDataEntryPathNormalizerTest.InvalidPathsToNormalize), typeof(DefaultDataEntryPathNormalizerTest), DynamicDataSourceType.Method)]
    public void Test_Normalize_Fails(string source)
    {
        TestNormalizePathFails(source);
    }


    public static IEnumerable<object[]> ValidPathsToNormalize()
    {
        yield return ["fileÖ?", "FILEÖ?"];
        yield return ["fiLE.TxT", "FILE.TXT"];
        yield return [".\\../fiLE.tXt", ".\\..\\FILE.TXT"];
    }
}