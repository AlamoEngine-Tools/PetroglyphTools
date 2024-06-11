using System.Linq;
using Moq;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services.Builder;

public class SortedDatBuilderBaseTest : DatBuilderBaseTest
{
    protected override Mock<DatBuilderBase> CreateBuilder(BuilderOverrideKind overrideKind)
    {
        var builder = new Mock<DatBuilderBase>(overrideKind, CreateServiceProvider());
        builder.SetupGet(b => b.TargetKeySortOrder).Returns(DatFileType.OrderedByCrc32);
        builder.SetupGet(b => b.KeyValidator).Returns(KeyValidator);
        return builder;
    }

    [Theory]
    [InlineData(BuilderOverrideKind.NoOverwrite)]
    [InlineData(BuilderOverrideKind.Overwrite)]
    [InlineData(BuilderOverrideKind.AllowDuplicate)]
    public void Test_AddEntry_Sorted(BuilderOverrideKind overrideKind)
    {
        var builder = CreateBuilder(overrideKind);

        // Crc32 of 'key' < 'other'
        var entry1 = builder.Object.AddEntry("other", "value2");
        var entry2 = builder.Object.AddEntry("key", "value1");


        Assert.True(entry1.Added);
        Assert.True(entry2.Added);

        Assert.Equal(
            [
                "key", "other"
            ],
            builder.Object.BuilderData.Select(x => x.Key).ToList());

        Assert.Equal(
            [
                "key", "other"
            ],
            builder.Object.SortedEntries.Select(x => x.Key).ToList());

        Assert.Equal(
            [
                "other", "key"
            ],
            builder.Object.Entries.Select(x => x.Key).ToList());
    }
}