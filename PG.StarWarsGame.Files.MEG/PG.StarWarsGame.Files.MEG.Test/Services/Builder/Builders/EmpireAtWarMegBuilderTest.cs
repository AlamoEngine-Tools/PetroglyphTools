using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

[TestClass]
public class EmpireAtWarMegBuilderTest : PetroglyphGameMegBuilderTest
{
    protected override Type ExpectedDataEntryValidatorType => typeof(EmpireAtWarMegDataEntryValidator);

    protected override Type ExpectedFileInfoValidatorType => typeof(EmpireAtWarMegFileInformationValidator);

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