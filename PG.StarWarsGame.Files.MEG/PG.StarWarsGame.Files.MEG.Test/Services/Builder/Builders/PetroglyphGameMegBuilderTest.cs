using PG.StarWarsGame.Files.MEG.Services.Builder;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

public abstract class PetroglyphGameMegBuilderTest : MegBuilderTestBase<PetroglyphGameMegBuilder>
{
    public const string BasePath = "Games/Petroglyph/corruption/";
    
    protected override bool? ExpectedOverwritesDuplicates => true;

    protected abstract PetroglyphGameMegBuilder CreatePetroBuilder(string basePath);

    [Fact]
    public void PetroglyphGameMegBuilderTes_Ctor()
    {
        var builder = CreatePetroBuilder(BasePath);
        Assert.Equal(FileSystem.Path.GetFullPath(BasePath), builder.BaseDirectory);
    }

    [Fact]
    public void Ctor_BasePathIsTreatedAsDirectory()
    {
        // Skipping trailing path separator on purpose
        var builder = CreatePetroBuilder("/game/corruption.dir");

        // Assert trailing path separator in instance.
        Assert.Equal(FileSystem.Path.GetFullPath("/game/corruption.dir/"), builder.BaseDirectory);
    }
}