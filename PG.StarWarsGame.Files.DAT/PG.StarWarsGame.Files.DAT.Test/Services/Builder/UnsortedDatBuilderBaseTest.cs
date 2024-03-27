using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder;

namespace PG.StarWarsGame.Files.DAT.Test.Services.Builder;

[TestClass]
public class UnsortedDatBuilderBaseTest : DatBuilderBaseTest
{
    protected override Mock<DatBuilderBase> CreateBuilder()
    {
        var builder = new Mock<DatBuilderBase>(CreateServiceProvider());
        builder.SetupGet(b => b.TargetKeySortOrder).Returns(DatFileType.NotOrdered);
        builder.SetupGet(b => b.KeyValidator).Returns(KeyValidator.Object);
        return builder;
    }

    [TestMethod]
    public void Test_AddEntry_Unsorted()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>())).Returns(new ValidationResult());

        HashingService.Setup(h => h.GetCrc32("key1", Encoding.ASCII)).Returns(new Crc32(1));
        HashingService.Setup(h => h.GetCrc32("key2", Encoding.ASCII)).Returns(new Crc32(2));

        var builder = CreateBuilder();

        // Add 2 before 1
        var result2 = builder.Object.AddEntry("key2", "value2");
        var result1 = builder.Object.AddEntry("key1", "value1");


        Assert.IsTrue(result1.Added);
        Assert.IsTrue(result2.Added);

        CollectionAssert.AreEqual(
            new List<DatStringEntry>
            {
                new("key2", new Crc32(2), "value2"),
                new("key1", new Crc32(1), "value1"),
            },
            builder.Object.BuilderData.ToList());

        CollectionAssert.AreEqual(
            new List<DatStringEntry>
            {
                new("key1", new Crc32(1), "value1"),
                new("key2", new Crc32(2), "value2"),
            },
            builder.Object.SortedEntries.ToList());

        CollectionAssert.AreEqual(
            new List<DatStringEntry>
            {
                new("key2", new Crc32(2), "value2"),
                new("key1", new Crc32(1), "value1"),
            },
            builder.Object.Entries.ToList());
    }

    [TestMethod]
    public void Test_RemoveAllKeys_WithDuplicates()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>())).Returns(new ValidationResult());

        var builder = CreateBuilder();
        builder.SetupGet(b => b.KeyOverwriteBehavior).Returns(BuilderOverrideKind.AllowDuplicate);

        builder.Object.AddEntry("key", "value");
        builder.Object.AddEntry("key", "value1");
        
        Assert.IsTrue(builder.Object.RemoveAllKeys("key"));

        Assert.AreEqual(0, builder.Object.BuilderData.Count);
    }
}