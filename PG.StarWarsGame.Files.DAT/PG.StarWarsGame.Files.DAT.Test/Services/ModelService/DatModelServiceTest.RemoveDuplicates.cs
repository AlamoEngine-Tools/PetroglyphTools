using System;
using System.Collections.Generic;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services;

public partial class DatModelServiceTest
{
    [Fact]
    public void Test_RemoveDuplicates_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Service.RemoveDuplicates(null!));
    }

    [Theory]
    [MemberData(nameof(RemoveDuplicates_TestData))]
    public void Test_RemoveDuplicates(IList<DatStringEntry> entries, IList<DatStringEntry> expected)
    {
        var model = CreateModel(entries);
        if (model.KeySortOder == DatFileType.OrderedByCrc32)
            expected = Crc32Utilities.SortByCrc32(expected);

        var newModel = Service.RemoveDuplicates(model);
        Assert.Equal(expected, newModel);
    }

    public static IEnumerable<object[]> RemoveDuplicates_TestData()
    {
        yield return
        [
            new List<DatStringEntry>(),
            new List<DatStringEntry>()
        ];
        yield return
        [
            new List<DatStringEntry>
            {
                new("key", new Crc32(1), "value"),
            },
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
                new("key1", new Crc32(1), "value1a"),
                new("key1", new Crc32(1), "value1b"),
                new("key2", new Crc32(2), "value2a"),
                new("key2", new Crc32(2), "value2b"),
                new("key2", new Crc32(2), "value2c"),
            },
            new List<DatStringEntry>
            {
                new("key1", new Crc32(1), "value1a"),
                new("key2", new Crc32(2), "value2a"),
            },
        ];

        yield return
        [
            new List<DatStringEntry>
            {
                new("key2", new Crc32(2), "value2a"),
				new("key1", new Crc32(1), "value1a"),
				new("key1", new Crc32(1), "value1b"),
                new("key2", new Crc32(2), "value2b"),
                new("key2", new Crc32(2), "value2c"),
            },
            new List<DatStringEntry>
            {
                new("key2", new Crc32(2), "value2a"),
				new("key1", new Crc32(1), "value1a"),
            },
        ];
	}
}