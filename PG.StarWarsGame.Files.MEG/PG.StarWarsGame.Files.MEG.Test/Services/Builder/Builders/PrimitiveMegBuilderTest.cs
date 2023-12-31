using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Services.Builder;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

[TestClass]
public class PrimitiveMegBuilderTest : MegBuilderTestSuite
{
    protected override Type ExpectedFileInfoValidatorType => typeof(MegBuilderBase.DefaultFileInformationValidator);
    protected override Type ExpectedDataEntryValidatorType => typeof(MegBuilderBase.NotNullDataEntryValidator);
    protected override Type? ExpectedDataEntryPathNormalizerType => null;
    protected override bool? ExpectedOverwritesDuplicates => true;
    protected override bool? ExpectedAutomaticallyAddFileSizes => false;

    protected override MegBuilderBase CreateBuilder(IServiceProvider serviceProvider)
    {
        return new PrimitiveMegBuilder(serviceProvider);
    }
}