﻿using System;
using Microsoft.Extensions.DependencyInjection;

using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;


public class EmpireAtWarMegBuilderTest : PetroglyphGameMegBuilderTest
{
    protected override Type ExpectedDataEntryValidatorType => typeof(EmpireAtWarMegBuilderDataEntryValidator);

    protected override Type ExpectedFileInfoValidatorType => typeof(EmpireAtWarMegFileInformationValidator);

    protected override Type ExpectedDataEntryPathNormalizerType => typeof(EmpireAtWarMegDataEntryPathNormalizer);

    protected override void SetupServiceCollection(IServiceCollection serviceCollection)
    {
        base.SetupServiceCollection(serviceCollection);
        serviceCollection.AddSingleton(sp => new EmpireAtWarMegBuilderDataEntryValidator(sp));
        serviceCollection.AddSingleton(sp => new EmpireAtWarMegFileInformationValidator(sp));
    }

    private EmpireAtWarMegBuilder CreateEaWBuilder(IServiceProvider serviceProvider)
    {
        return new EmpireAtWarMegBuilder(BasePath, serviceProvider);
    }

    private EmpireAtWarMegBuilder CreateEaWBuilder(string path, IServiceProvider serviceProvider)
    {
        return new EmpireAtWarMegBuilder(path, serviceProvider);
    }

    protected override PetroglyphGameMegBuilder CreatePetroBuilder(string basePath, IServiceProvider serviceProvider)
    {
        return CreateEaWBuilder(basePath, serviceProvider);
    }

    protected override MegBuilderBase CreateBuilder(IServiceProvider serviceProvider)
    {
        return CreateEaWBuilder(BasePath, serviceProvider);
    }
}