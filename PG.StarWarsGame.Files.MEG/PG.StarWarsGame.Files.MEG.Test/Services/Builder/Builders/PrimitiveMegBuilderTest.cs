using System;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

public class PrimitiveMegBuilderTest : MegBuilderTestSuite
{
    protected override Type ExpectedFileInfoValidatorType => typeof(DefaultMegFileInformationValidator);
    protected override Type ExpectedDataEntryValidatorType => typeof(NotNullDataEntryValidator);
    protected override Type? ExpectedDataEntryPathNormalizerType => null;
    protected override bool? ExpectedOverwritesDuplicates => true;
    protected override bool? ExpectedAutomaticallyAddFileSizes => false;

    protected override MegBuilderBase CreateBuilder(IServiceProvider serviceProvider)
    {
        return new PrimitiveMegBuilder(serviceProvider);
    }
}