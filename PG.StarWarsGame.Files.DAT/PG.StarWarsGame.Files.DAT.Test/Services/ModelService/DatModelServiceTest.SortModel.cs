using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services;

public abstract partial class DatModelServiceTest
{
    [Fact]
    public void Test_SortModel_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Service.SortModel(null!));
    }

    [Theory]
    [MemberData(nameof(GetUnsortedEntriesTestData))]
    public void Test_SortModel(IList<DatStringEntry> entries, IList<DatStringEntry> expectedList)
    {
        var model = CreateModel(entries);
        var sorted = Service.SortModel(model);
        Assert.Equal(DatFileType.OrderedByCrc32, sorted.KeySortOrder);
        Assert.Equal(expectedList, sorted.ToList());


        var modelMock = new Mock<IDatModel>();
        modelMock.Setup(m => m.GetEnumerator()).Returns(entries.GetEnumerator());
        var sortedMock = Service.SortModel(modelMock.Object);
        Assert.Equal(DatFileType.OrderedByCrc32, sortedMock.KeySortOrder);
        Assert.Equal(expectedList, sortedMock.ToList());
    }

    public static IEnumerable<object[]> GetUnsortedEntriesTestData()
    {
        yield return
        [
            new List<DatStringEntry>(),
            // Expected
            new List<DatStringEntry>()
        ];
        yield return
        [
            new List<DatStringEntry>
            {
                new("key", new Crc32(1), "value"),
            },
            // Expected
            new List<DatStringEntry>
            {
                new("key", new Crc32(1), "value"),
            }
        ];
        yield return
        [
            new List<DatStringEntry>
            {
                new("key1", new Crc32(1), "value1"),
                new("key2", new Crc32(2), "value2"),
            },
            // Expected
            new List<DatStringEntry>
            {
                new("key1", new Crc32(1), "value1"),
                new("key2", new Crc32(2), "value2"),
            }

        ];
        yield return
        [
            new List<DatStringEntry>
            {
                new("key2", new Crc32(2), "value2"),
                new("key1", new Crc32(1), "value1"),
            },
            // Expected
            new List<DatStringEntry>
            {
                new("key1", new Crc32(1), "value1"),
                new("key2", new Crc32(2), "value2"),
            },
        ];
        yield return
        [
            new List<DatStringEntry>
            {
                new("key2", new Crc32(2), "value2a"),
                new("key2", new Crc32(2), "value2b"),
                new("key1", new Crc32(1), "value1a"),
                new("key1", new Crc32(1), "value1b"),
            },
            // Expected
            new List<DatStringEntry>
            {
                new("key1", new Crc32(1), "value1a"),
                new("key1", new Crc32(1), "value1b"),
                new("key2", new Crc32(2), "value2a"),
                new("key2", new Crc32(2), "value2b"),
            },
        ];
        yield return
        [
            new List<DatStringEntry>
            {
                new("key2", new Crc32(2), "value2a"),
                new("key3", new Crc32(3), "value3"),
                new("key2", new Crc32(2), "value2b"),
                new("key1", new Crc32(1), "value1a"),
                new("key4", new Crc32(4), "value4"),
                new("key1", new Crc32(1), "value1b"),
            },
            // Expected
            new List<DatStringEntry>
            {
                new("key1", new Crc32(1), "value1a"),
                new("key1", new Crc32(1), "value1b"),
                new("key2", new Crc32(2), "value2a"),
                new("key2", new Crc32(2), "value2b"),
                new("key3", new Crc32(3), "value3"),
                new("key4", new Crc32(4), "value4"),
            },
        ];
    }
}
