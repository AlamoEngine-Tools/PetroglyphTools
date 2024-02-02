using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Normalization;

[TestClass]
public class DefaultDataEntryPathNormalizerTest : DataEntryPathNormalizerTestBase
{
    protected override IMegDataEntryPathNormalizer CreateNormalizer(IServiceProvider serviceProvider)
    {
        return new DefaultDataEntryPathNormalizer(serviceProvider);
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
        yield return ["file.öäü", "FILE.ÖÄÜ"];
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            yield return [".\\../fiLE.tXt", ".\\..\\FILE.TXT"];
        else
            yield return [".\\../fiLE.tXt", "./../FILE.TXT"];
    }

    public static IEnumerable<object[]> InvalidPathsToNormalize()
    {
        yield return [null!];
        yield return [""];
    }
}