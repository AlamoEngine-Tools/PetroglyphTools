using System.Collections.Generic;
using System.Linq;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Data;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services;

public abstract partial class DatModelServiceTest
{
    [Theory]
    [MemberData(nameof(GetDuplicateEntries_ModelsWithoutDuplicates))]
    public void Test_GetDuplicateEntries_NoDuplicates(IList<DatStringEntry> entries)
    {
        var model = CreateModel(entries);
        var duplicates = Service.GetDuplicateEntries(model);
        Assert.Empty(duplicates.Keys);
    }

    public static IEnumerable<object[]> GetDuplicateEntries_ModelsWithoutDuplicates()
    {
        yield return
        [
            new List<DatStringEntry>()
        ];
        yield return
        [
            new List<DatStringEntry>
            {
                new("key", new Crc32(1), "value")
            }
        ];
        yield return
        [
            new List<DatStringEntry>
            {
                new("key1", new Crc32(1), "value1"),
                new("key2", new Crc32(2), "value2"),
            }
        ];
    }

    [Fact]
    public void Test_GetDuplicateEntries_HasDuplicates()
    {
        var entries = new List<DatStringEntry>
        {
            new("key1", new Crc32(1), "value1"),
            new("key1", new Crc32(1), "value2"),
            new("key2", new Crc32(2), "other1"),
            new("key2", new Crc32(2), "other2"),
            new("key1", new Crc32(1), "value3"),
        };

        var model = CreateModel(entries);
        var duplicates = Service.GetDuplicateEntries(model);

        Assert.Equivalent(new HashSet<string> { "key1", "key2" }, duplicates.Keys);

        var key1Entries = duplicates["key1"];
        Assert.Equal(new List<string> { "value1", "value2", "value3" }, key1Entries.Select(x => x.Value));

        var key2Entries = duplicates["key2"];
        Assert.Equal(new List<string> { "other1", "other2" }, key2Entries.Select(x => x.Value));
    }
}
