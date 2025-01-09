using System.Collections.Generic;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services.Builder;

public class EmpireAtWarMasterTextBuilderTest_Overriding : EmpireAtWarMasterTextBuilderTestBase
{
    protected override BuilderOverrideKind OverrideKind => BuilderOverrideKind.Overwrite;
}

public class EmpireAtWarMasterTextBuilderTest_NotOverriding : EmpireAtWarMasterTextBuilderTestBase
{
    protected override BuilderOverrideKind OverrideKind => BuilderOverrideKind.NoOverwrite;
}

public abstract class EmpireAtWarMasterTextBuilderTestBase : PetroglyphStarWarsGameDatBuilder
{
    protected override bool IsOrderedBuilder => true;

    protected override DatBuilderBase CreateBuilder()
    {
        return new EmpireAtWarMasterTextBuilder(OverrideKind == BuilderOverrideKind.Overwrite, ServiceProvider);
    }

    protected override (IReadOnlyList<DatStringEntry> Data, byte[] Bytes) CreateValidData()
    {
        var bytes = DatTestData.CreateSortedBinary().Bytes;
        var model = DatTestData.CreateSortedModel();
        return (model, bytes);
    }

    [Fact]
    public void Ctor()
    {
        var builder = CreateBuilder();
        Assert.NotNull(builder.BuilderData);
        Assert.NotNull(builder.SortedEntries);
        Assert.NotNull(builder.Entries);
        Assert.Equal(OverrideKind, builder.KeyOverwriteBehavior);
        Assert.Equal(DatFileType.OrderedByCrc32, builder.TargetKeySortOrder);
        Assert.IsType<EmpireAtWarKeyValidator>(builder.KeyValidator);
    }
}