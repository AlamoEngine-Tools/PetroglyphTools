using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Services.Builder;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

[TestClass]
public class PetroglyphGameMegBuilderBaseTest : PetroglyphGameMegBuilderTest
{
    protected override PetroglyphGameMegBuilder CreatePetroBuilder(string basePath, IServiceProvider serviceProvider)
    {
        return new TestPetroglyphGameMegBuilder(basePath, serviceProvider);
    }

    private class TestPetroglyphGameMegBuilder(string baseDirectory, IServiceProvider services)
        : PetroglyphGameMegBuilder(baseDirectory, services);
}