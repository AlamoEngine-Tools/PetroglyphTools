using System;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

public abstract class PetroglyphGameMegBuilderTest : MegBuilderTestSuite
{
    public const string BasePath = "/Games/Petroglyph/corruption/";
    protected override Type ExpectedFileInfoValidatorType => typeof(PetroglyphMegFileInformationValidator);
    protected override Type ExpectedDataEntryValidatorType => typeof(NotNullDataEntryValidator);
    protected override Type ExpectedDataEntryPathNormalizerType => typeof(PetroglyphDataEntryPathNormalizer);
    protected override bool? ExpectedOverwritesDuplicates => true;
    protected override bool? ExpectedAutomaticallyAddFileSizes => true;

    protected abstract PetroglyphGameMegBuilder CreatePetroBuilder(string basePath, IServiceProvider serviceProvider);

    protected override MegBuilderBase CreateBuilder()
    {
        return CreatePetroBuilder(BasePath, ServiceProvider);
    }

    [Fact]
    public void PetroglyphGameMegBuilderTest_Test_Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new EmpireAtWarMegBuilder(null!, ServiceProvider));
        Assert.Throws<ArgumentException>(() => new EmpireAtWarMegBuilder("", ServiceProvider));
    }

    [Fact]
    public void PetroglyphGameMegBuilderTest_Test_Ctor()
    {
        var builder = CreatePetroBuilder(BasePath, ServiceProvider);
        Assert.Equal(FileSystem.Path.GetFullPath(BasePath), builder.BaseDirectory);
    }

    [Fact]
    public void PetroglyphGameMegBuilderTest_Test_Ctor_BasePathIsTreatedAsDirectory()
    { 
        // Skipping trailing path separator on purpose
        var builder = CreatePetroBuilder("/game/corruption.dir", ServiceProvider);

        // Assert trailing path separator in instance.
        Assert.Equal(FileSystem.Path.GetFullPath("/game/corruption.dir/"), builder.BaseDirectory);
    }

    // TODO: Use test cases from resolver test
    //[Fact]
    //public void Test_ResolveEntryPath()
    //{
    //    var builder = CreatePetroBuilder(BasePath, ServiceProvider);
    //    EntryPathResolver.Setup(r => r.ResolvePath("somePath", builder.BaseDirectory))
    //        .Returns("someReturn");
    //    Assert.Equal("someReturn", builder.ResolveEntryPath("somePath"));
    //}
}