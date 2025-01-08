using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Normalization;

public abstract class PetroglyphDataEntryPathNormalizerTestBase : DataEntryPathNormalizerTestBase
{
    protected sealed override IMegDataEntryPathNormalizer CreateNormalizer()
    {
        return CreatePGNormalizer();
    }

    protected abstract PetroglyphDataEntryPathNormalizer CreatePGNormalizer();
}