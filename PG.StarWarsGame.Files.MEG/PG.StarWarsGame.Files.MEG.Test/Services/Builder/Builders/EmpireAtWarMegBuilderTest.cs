using System;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

public class EmpireAtWarMegBuilderTest : PetroglyphGameMegBuilderTest
{
    protected override Type ExpectedDataEntryValidatorType => typeof(EmpireAtWarMegBuilderDataEntryValidator);

    protected override Type ExpectedFileInfoValidatorType => typeof(EmpireAtWarMegFileInformationValidator);

    protected override Type ExpectedDataEntryPathNormalizerType => typeof(EmpireAtWarMegDataEntryPathNormalizer);

    private EmpireAtWarMegBuilder CreateEaWBuilder(string path, IServiceProvider serviceProvider)
    {
        return new EmpireAtWarMegBuilder(path, serviceProvider);
    }

    protected override PetroglyphGameMegBuilder CreatePetroBuilder(string basePath, IServiceProvider serviceProvider)
    {
        return CreateEaWBuilder(basePath, serviceProvider);
    }

    protected override MegBuilderBase CreateBuilder()
    {
        return CreateEaWBuilder(BasePath,  ServiceProvider);
    }
}