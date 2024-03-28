
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Normalization;


public class DefaultDataEntryPathNormalizerTest : DataEntryPathNormalizerTestBase
{
    protected override IMegDataEntryPathNormalizer CreateNormalizer(IServiceProvider serviceProvider)
    {
        return new DefaultDataEntryPathNormalizer(serviceProvider);
    }

    [Theory]
    [MemberData(nameof(ValidPathsToNormalize))]
    public void Test_Normalize_Success(string source, string expected)
    {
        TestNormalizePathPasses(source, expected);
    }

    [Theory]
    [MemberData(nameof(InvalidPathsToNormalize))]
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