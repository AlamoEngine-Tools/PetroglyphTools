using System;
using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Normalization;


public class PetroglyphDataEntryPathNormalizerTest : DataEntryPathNormalizerTestBase
{
    protected override IMegDataEntryPathNormalizer CreateNormalizer(IServiceProvider serviceProvider)
    {
        return new PetroglyphDataEntryPathNormalizer(serviceProvider);
    }

    [Theory]
    [MemberData(nameof(ValidPathsToNormalize))]
    public void Test_Normalize_Success(string source, string expected)
    {
        TestNormalizePathPasses(source, expected);
    }

    [Theory]
    [MemberData(nameof(DefaultDataEntryPathNormalizerTest.InvalidPathsToNormalize), MemberType = typeof(DefaultDataEntryPathNormalizerTest))]
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