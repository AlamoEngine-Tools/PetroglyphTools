using System.Linq;
using System.Text;
using Moq;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services.Builder;

public class UnsortedDatBuilderBaseTest : DatBuilderBaseTest
{
    protected override Mock<DatBuilderBase> CreateBuilder()
    {
        var builder = new Mock<DatBuilderBase>(CreateServiceProvider());
        builder.SetupGet(b => b.TargetKeySortOrder).Returns(DatFileType.NotOrdered);
        builder.SetupGet(b => b.KeyValidator).Returns(KeyValidator.Object);
        return builder;
    }

    [Fact]
    public void Test_AddEntry_Unsorted()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>())).Returns(true);

        HashingService.Setup(h => h.GetCrc32("key1", Encoding.ASCII)).Returns(new Crc32(1));
        HashingService.Setup(h => h.GetCrc32("key2", Encoding.ASCII)).Returns(new Crc32(2));

        var builder = CreateBuilder();

        // Add 2 before 1
        var result2 = builder.Object.AddEntry("key2", "value2");
        var result1 = builder.Object.AddEntry("key1", "value1");


        Assert.True(result1.Added);
        Assert.True(result2.Added);

        Assert.Equal(
            [
                new("key2", new Crc32(2), "value2"),
                new("key1", new Crc32(1), "value1")
            ],
            builder.Object.BuilderData.ToList());

        Assert.Equal(
            [
                new("key1", new Crc32(1), "value1"),
                new("key2", new Crc32(2), "value2")
            ],
            builder.Object.SortedEntries.ToList());

        Assert.Equal(
            [
                new("key2", new Crc32(2), "value2"),
                new("key1", new Crc32(1), "value1")
            ],
            builder.Object.Entries.ToList());
    }

    [Fact]
    public void Test_RemoveAllKeys_WithDuplicates()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>())).Returns(true);

        var builder = CreateBuilder();
        builder.SetupGet(b => b.KeyOverwriteBehavior).Returns(BuilderOverrideKind.AllowDuplicate);

        builder.Object.AddEntry("key", "value");
        builder.Object.AddEntry("key", "value1");
        
        Assert.True(builder.Object.RemoveAllKeys("key"));

        Assert.Equal(0, builder.Object.BuilderData.Count);
    }
}