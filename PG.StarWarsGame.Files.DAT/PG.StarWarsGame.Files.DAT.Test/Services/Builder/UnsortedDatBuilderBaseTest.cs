//using System.Linq;
//using Moq;
//using PG.StarWarsGame.Files.DAT.Files;
//using PG.StarWarsGame.Files.DAT.Services.Builder;
//using Xunit;

//namespace PG.StarWarsGame.Files.DAT.Test.Services.Builder;

//public class UnsortedDatBuilderBaseTest : DatBuilderBaseTest
//{
//    protected override Mock<DatBuilderBase> CreateBuilder(BuilderOverrideKind overrideKind)
//    {
//        var builder = new Mock<DatBuilderBase>(overrideKind, CreateServiceProvider());
//        builder.SetupGet(b => b.TargetKeySortOrder).Returns(DatFileType.NotOrdered);
//        builder.SetupGet(b => b.KeyValidator).Returns(KeyValidator);
//        return builder;
//    }

//    [Theory]
//    [InlineData(BuilderOverrideKind.NoOverwrite)]
//    [InlineData(BuilderOverrideKind.Overwrite)]
//    [InlineData(BuilderOverrideKind.AllowDuplicate)]
//    public void Test_AddEntry_Unsorted(BuilderOverrideKind overrideKind)
//    {
//        var builder = CreateBuilder(overrideKind);

//        // Crc32 of 'other' > 'key'
//        var added1 = builder.Object.AddEntry("other", "value2");
//        var added2 = builder.Object.AddEntry("key", "value1");


//        Assert.True(added1.Added);
//        Assert.True(added2.Added);

//        Assert.Equal(
//            [
//                "other", "key"
//            ],
//            builder.Object.BuilderData.Select(x => x.Key).ToList());

//        Assert.Equal(
//            [
//                "key", "other"
//            ],
//            builder.Object.SortedEntries.Select(x => x.Key).ToList());

//        Assert.Equal(
//            [
//                "other", "key"
//            ],
//            builder.Object.Entries.Select(x => x.Key).ToList());
//    }

//    [Fact]
//    public void Test_RemoveAllKeys_WithDuplicates()
//    {
//        var builder = CreateBuilder(BuilderOverrideKind.AllowDuplicate);

//        builder.Object.AddEntry("key", "value");
//        builder.Object.AddEntry("key", "value1");
        
//        Assert.True(builder.Object.RemoveAllKeys("key"));

//        Assert.Empty(builder.Object.BuilderData);
//    }
//}