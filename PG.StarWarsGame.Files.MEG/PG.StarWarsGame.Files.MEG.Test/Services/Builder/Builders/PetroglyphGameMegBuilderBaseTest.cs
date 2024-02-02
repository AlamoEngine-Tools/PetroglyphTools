using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

[TestClass]
public class PetroglyphGameMegBuilderBaseTest : PetroglyphGameMegBuilderTest
{
    protected override Type ExpectedDataEntryValidatorType => typeof(TestPetroglyphMegDataEntryValidator);

    protected override Type ExpectedFileInfoValidatorType => typeof(TestPetroglyphMegFileInformationValidator);

    protected override PetroglyphGameMegBuilder CreatePetroBuilder(string basePath, IServiceProvider serviceProvider)
    {
        return new TestPetroglyphGameMegBuilder(basePath, serviceProvider);
    }

    private class TestPetroglyphGameMegBuilder(string baseDirectory, IServiceProvider services) 
        : PetroglyphGameMegBuilder(baseDirectory, services)
    {
        protected override PetroglyphMegDataEntryValidator PetroDataEntryValidator { get; } =
            new TestPetroglyphMegDataEntryValidator(services);

        protected override PetroglyphMegFileInformationValidator PetroMegFileInformationValidator { get; } =
            new TestPetroglyphMegFileInformationValidator(services);
    }

    private class TestPetroglyphMegDataEntryValidator(IServiceProvider serviceProvider)
        : PetroglyphMegDataEntryValidator(serviceProvider);

    private class TestPetroglyphMegFileInformationValidator(IServiceProvider serviceProvider)
        : PetroglyphMegFileInformationValidator(serviceProvider);
}