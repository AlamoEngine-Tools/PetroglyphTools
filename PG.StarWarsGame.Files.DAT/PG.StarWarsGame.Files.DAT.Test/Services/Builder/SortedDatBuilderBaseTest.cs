using System.Linq;
using System.Text;
using Moq;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services.Builder;

public class SortedDatBuilderBaseTest : DatBuilderBaseTest
{
    protected override Mock<DatBuilderBase> CreateBuilder()
    {
        var builder = new Mock<DatBuilderBase>(CreateServiceProvider());
        builder.SetupGet(b => b.TargetKeySortOrder).Returns(DatFileType.OrderedByCrc32);
        builder.SetupGet(b => b.KeyValidator).Returns(KeyValidator.Object);
        return builder;
    }

    [Fact]
    public void Test_AddEntry_Sorted()
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
                new("key1", new Crc32(1), "value1"),
                new("key2", new Crc32(2), "value2")
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
}