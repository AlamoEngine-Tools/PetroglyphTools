using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

public abstract class PetroglyphGameMegBuilderTest : MegBuilderTestSuite
{
    public const string BasePath = "/Games/Petroglyph/corruption/";

    private protected Mock<IDataEntryPathResolver> EntryPathResolver { get; } = new();

    protected override Type ExpectedFileInfoValidatorType => typeof(PetroglyphMegFileInformationValidator);
    protected override Type ExpectedDataEntryValidatorType => typeof(NotNullDataEntryValidator);
    protected override Type ExpectedDataEntryPathNormalizerType => typeof(PetroglyphDataEntryPathNormalizer);
    protected override bool? ExpectedOverwritesDuplicates => true;
    protected override bool? ExpectedAutomaticallyAddFileSizes => true;

    protected abstract PetroglyphGameMegBuilder CreatePetroBuilder(string basePath, IServiceProvider serviceProvider);

    protected override void SetupServiceCollection(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton(EntryPathResolver.Object);
        serviceCollection.AddSingleton(sp => new PetroglyphDataEntryPathNormalizer(sp));
        base.SetupServiceCollection(serviceCollection);
    }

    protected override MegBuilderBase CreateBuilder(IServiceProvider serviceProvider)
    {
        return CreatePetroBuilder(BasePath, serviceProvider);
    }

    [Fact]
    public void PetroglyphGameMegBuilderTest_Test_Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new EmpireAtWarMegBuilder(null!, CreateServiceProvider()));
        Assert.Throws<ArgumentException>(() => new EmpireAtWarMegBuilder("", CreateServiceProvider()));
    }

    [Fact]
    public void PetroglyphGameMegBuilderTest_Test_Ctor()
    {
        var builder = CreatePetroBuilder(BasePath, CreateServiceProvider());
        Assert.Equal(FileSystem.Path.GetFullPath(BasePath), builder.BaseDirectory);
    }

    [Fact]
    public void PetroglyphGameMegBuilderTest_Test_Ctor_BasePathIsTreatedAsDirectory()
    { 
        // Skipping trailing path separator on purpose
        var builder = CreatePetroBuilder("/game/corruption.dir", CreateServiceProvider());

        // Assert trailing path separator in instance.
        Assert.Equal(FileSystem.Path.GetFullPath("/game/corruption.dir/"), builder.BaseDirectory);
    }

    [Fact]
    public void Test_ResolveEntryPath()
    {
        var builder = CreatePetroBuilder(BasePath, CreateServiceProvider());
        EntryPathResolver.Setup(r => r.ResolvePath("somePath", builder.BaseDirectory)).Returns("someReturn");
        Assert.Equal("someReturn", builder.ResolveEntryPath("somePath"));
    }
}