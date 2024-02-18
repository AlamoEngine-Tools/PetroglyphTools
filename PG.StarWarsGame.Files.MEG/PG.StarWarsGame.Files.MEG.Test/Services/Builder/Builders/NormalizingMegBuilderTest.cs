using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

[TestClass]
public class NormalizingMegBuilderTest : MegBuilderTestSuite
{
    protected override Type ExpectedFileInfoValidatorType => typeof(DefaultMegFileInformationValidator);
    protected override Type ExpectedDataEntryValidatorType => typeof(NotNullDataEntryValidator);
    protected override Type? ExpectedDataEntryPathNormalizerType => typeof(DefaultDataEntryPathNormalizer);
    protected override bool? ExpectedOverwritesDuplicates => true;
    protected override bool? ExpectedAutomaticallyAddFileSizes => false;

    protected override MegBuilderBase CreateBuilder(IServiceProvider serviceProvider)
    {
        return new NormalizingMegBuilder(serviceProvider);
    }

    protected override void SetupServiceCollection(IServiceCollection serviceCollection)
    {
        base.SetupServiceCollection(serviceCollection);
        serviceCollection.AddSingleton(sp => new DefaultDataEntryPathNormalizer(sp));
    }
}