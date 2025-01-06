using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

public class NormalizingMegBuilderTest : MegBuilderTestBase<NormalizingMegBuilder>
{
    protected override Type ExpectedFileInfoValidatorType => typeof(DefaultMegFileInformationValidator);
    protected override Type ExpectedDataEntryValidatorType => typeof(NotNullDataEntryValidator);
    protected override Type? ExpectedDataEntryPathNormalizerType => typeof(DefaultDataEntryPathNormalizer);
    protected override bool? ExpectedOverwritesDuplicates => true;
    protected override bool? ExpectedAutomaticallyAddFileSizes => false;

    protected override void SetupServices(ServiceCollection serviceCollection)
    {
        base.SetupServices(serviceCollection);
        serviceCollection.AddSingleton(sp => new DefaultDataEntryPathNormalizer(sp));
    }

    protected override NormalizingMegBuilder CreateBuilder()
    {
        return new NormalizingMegBuilder(ServiceProvider);
    }

    protected override MegFileInformation CreateFileInfo(bool valid, string path)
    {
        throw new NotImplementedException();
    }

    protected override void AddDataToBuilder(IReadOnlyCollection<MegFileDataEntryBuilderInfo> data, NormalizingMegBuilder builder)
    {
        throw new NotImplementedException();
    }

    protected override (IReadOnlyCollection<MegFileDataEntryBuilderInfo> Data, byte[] Bytes) CreateValidData()
    {
        throw new NotImplementedException();
    }
}