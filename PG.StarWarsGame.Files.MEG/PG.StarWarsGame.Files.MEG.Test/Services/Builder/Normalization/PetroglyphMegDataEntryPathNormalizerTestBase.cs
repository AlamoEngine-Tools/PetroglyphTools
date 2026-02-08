using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using System.Collections.Generic;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Normalization;

public abstract class PetroglyphMegDataEntryPathNormalizerTestBase : MegDataEntryPathNormalizerTestBase
{
    protected sealed override IMegDataEntryPathNormalizer CreateNormalizer()
    {
        return CreatePetroglyphNormalizer();
    }

    protected abstract PetroglyphMegDataEntryPathNormalizer CreatePetroglyphNormalizer();
    
    public static IEnumerable<object[]> PetroglyphTestNormalize()
    {
        // Null and empty handling
        yield return [null!, ""];
        yield return ["", ""];
        yield return [".", "."];

        // Allow spaces
        yield return ["  ", "  "];
        yield return ["TEST   ", "TEST   "];

        // Do nothing
        yield return ["TEST", "TEST"];

        // Separator at end
        yield return ["TEST\\", "TEST\\"];
        yield return ["MY\\TEST\\", "MY\\TEST\\"];

        // Uppercase
        yield return ["fiLE.TxT", "FILE.TXT"];

        // This normalizer does not encode
        yield return ["file.öäü", "FILE.ÖÄÜ"];
        
        // Long paths are allowed
        yield return [new string('a', 270), new string('A', 270)];

        // Normalize path separator
        yield return ["MY/PATH\\FILE.TXT", "MY\\PATH\\FILE.TXT"];

        // Parent directory control sequence is allowed
        yield return ["MY\\..\\FILE.TXT", "MY\\..\\FILE.TXT"];
        
        // Combined
        yield return ["my path/test/..\\FILE.äÖ", "MY PATH\\TEST\\..\\FILE.ÄÖ"];
    }

    [Theory]
    [MemberData(nameof(PetroglyphTestNormalize))]
    public void Normalize_UpperCaseAndWindowsDirectorySeparator(string source, string expected)
    {
        TestNormalizePathPasses(source, expected);
    }
}