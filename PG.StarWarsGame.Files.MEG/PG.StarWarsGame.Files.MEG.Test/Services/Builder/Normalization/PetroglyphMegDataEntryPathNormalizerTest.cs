using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Normalization;

public class PetroglyphMegDataEntryPathNormalizerTest : PetroglyphMegDataEntryPathNormalizerTestBase
{
    protected override PetroglyphMegDataEntryPathNormalizer CreatePetroglyphNormalizer()
    {
        return new PetroglyphMegDataEntryPathNormalizer();
    }
}