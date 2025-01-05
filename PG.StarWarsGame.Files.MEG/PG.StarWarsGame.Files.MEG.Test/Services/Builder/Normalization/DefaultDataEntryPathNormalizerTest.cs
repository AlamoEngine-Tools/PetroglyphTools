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

    
    public static IEnumerable<object[]> ValidPathsToNormalize()
    {
        yield return [null, ""];
        yield return ["", ""];

        yield return ["file.öäü", "FILE.ÖÄÜ"];
        yield return [new string('a', 270), new string('A', 270)];

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            yield return [".\\../fiLE.tXt", ".\\..\\FILE.TXT"];
        else
            yield return [".\\../fiLE.tXt", "./../FILE.TXT"];
    }
}