using System;
using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Normalization;

public class EmpireAtWarMegDataEntryPathNormalizerTest : PetroglyphDataEntryPathNormalizerTestBase
{
    protected override PetroglyphDataEntryPathNormalizer CreatePGNormalizer(IServiceProvider serviceProvider)
    {
        return new EmpireAtWarMegDataEntryPathNormalizer(serviceProvider);
    }

    [Theory]
    [MemberData(nameof(ValidPathsToNormalize))]
    public void Normalize_Success(string source, string expected)
    {
        TestNormalizePathPasses(source, expected);
    }
    
    public static IEnumerable<object[]> ValidPathsToNormalize()
    {
        // Null and empty handling
        yield return [null!, string.Empty];
        yield return ["", string.Empty];
        yield return [".", "."];

        // Do nothing
        yield return ["TEST", "TEST"];

        // Separator at end
        yield return ["TEST\\", "TEST\\"];
        yield return ["MY\\TEST\\", "MY\\TEST\\"];

        // The normalizer of the engine does not check or ensure the max length and works with any size
        yield return [new string('a', 270), new string('A', 270)];

        // This normalizer does not encode
        yield return ["fileÖ?", "FILEÖ?"];

        // Uppercase
        yield return ["fiLE.TxT", "FILE.TXT"];

        // Normalize directory separators
        yield return ["MY/PATH\\FILE", "MY\\PATH\\FILE"];

        // Trim this directory but do not trim period-starting file name
        yield return [".\\MY\\TEST.txt", "MY\\TEST.TXT"];
        yield return ["./my/TEST", "MY\\TEST"];
        yield return [".\\", string.Empty];
        yield return ["\\", string.Empty];
        yield return ["\\\\", string.Empty];
        yield return [".TEST", ".TEST"];

        // Now these are odd cases, but that's how the game behaves... 
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