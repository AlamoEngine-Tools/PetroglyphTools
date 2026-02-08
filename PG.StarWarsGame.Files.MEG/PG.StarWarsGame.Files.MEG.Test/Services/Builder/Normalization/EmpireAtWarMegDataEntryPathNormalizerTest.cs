using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Normalization;

public class EmpireAtWarMegDataEntryPathNormalizerTest : PetroglyphMegDataEntryPathNormalizerTestBase
{
    protected override PetroglyphMegDataEntryPathNormalizer CreatePetroglyphNormalizer()
    {
        return new EmpireAtWarMegDataEntryPathNormalizer();
    }

    [Theory]
    [MemberData(nameof(EmpireAtWarTestNormalizeTestData))]
    public void Normalize_EmpireAtWar(string source, string expected)
    {
        TestNormalizePathPasses(source, expected);
    }

    public static IEnumerable<object[]> EmpireAtWarTestNormalizeTestData()
    {
        // Trim this directory but do not trim period-starting file name
        yield return [".\\MY\\TEST.txt", "MY\\TEST.TXT"];
        yield return ["./my/TEST", "MY\\TEST"];
        yield return [".\\", string.Empty];
        yield return ["\\", string.Empty];
        yield return ["\\\\", string.Empty];
        yield return [".TEST", ".TEST"];

        // Now, these are odd cases, but that's how the game behaves... 
        yield return ["/TEST.TXT", "TEST.TXT\\TEST.TXT"];
        yield return ["./TEST.TXT", "TEST.TXT\\TEST.TXT"];
        yield return ["c:/test.txt", ":\\TEST.TXT"];
        yield return ["c:/my/test.txt", ":\\MY\\TEST.TXT"];

        // Trim leading directory separator
        yield return ["/game/corruption/data/xml/entry2.txt", "GAME\\CORRUPTION\\DATA\\XML\\ENTRY2.TXT"];
        yield return ["./game/corruption/data/xml/entry2.txt", "GAME\\CORRUPTION\\DATA\\XML\\ENTRY2.TXT"];

        // The first and last slashes get removed, the middle slash remains
        // and an additional \ (+ empty file part) gets appended
        yield return [@"\\\", @"\\"];
    }
}