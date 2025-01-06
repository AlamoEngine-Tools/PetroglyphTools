using System;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

public abstract class PetroglyphGameMegBuilderTest : MegBuilderTestBase<PetroglyphGameMegBuilder>
{
    public const string BasePath = "Games/Petroglyph/corruption/";
    
    protected override Type ExpectedFileInfoValidatorType => typeof(PetroglyphMegFileInformationValidator);
    protected override Type ExpectedDataEntryValidatorType => typeof(NotNullDataEntryValidator);
    protected override Type ExpectedDataEntryPathNormalizerType => typeof(PetroglyphDataEntryPathNormalizer);
    protected override bool? ExpectedOverwritesDuplicates => true;
    protected override bool? ExpectedAutomaticallyAddFileSizes => true;

    protected abstract PetroglyphGameMegBuilder CreatePetroBuilder(string basePath);

    [Fact]
    public void PetroglyphGameMegBuilderTest_Test_Ctor()
    {
        var builder = CreatePetroBuilder(BasePath);
        Assert.Equal(FileSystem.Path.GetFullPath(BasePath), builder.BaseDirectory);
    }

    [Fact]
    public void PetroglyphGameMegBuilderTest_Test_Ctor_BasePathIsTreatedAsDirectory()
    { 
        // Skipping trailing path separator on purpose
        var builder = CreatePetroBuilder("/game/corruption.dir");

        // Assert trailing path separator in instance.
        Assert.Equal(FileSystem.Path.GetFullPath("/game/corruption.dir/"), builder.BaseDirectory);
    }
}