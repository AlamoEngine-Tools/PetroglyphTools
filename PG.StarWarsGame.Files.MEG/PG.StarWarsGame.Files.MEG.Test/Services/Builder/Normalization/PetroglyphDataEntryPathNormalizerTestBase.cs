using System;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Normalization;

public abstract class PetroglyphDataEntryPathNormalizerTestBase : DataEntryPathNormalizerTestBase
{
    protected sealed override IMegDataEntryPathNormalizer CreateNormalizer(IServiceProvider serviceProvider)
    {
        return CreatePGNormalizer(serviceProvider);
    }

    protected abstract PetroglyphDataEntryPathNormalizer CreatePGNormalizer(IServiceProvider serviceProvider);
}