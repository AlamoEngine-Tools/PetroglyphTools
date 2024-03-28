using System.Collections.Generic;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Data;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services;

public abstract partial class DatModelServiceTest
{
    [Theory]
    [MemberData(nameof(GetMissingKeysFromBase_TestData))]
    public void Test_GetMissingKeysFromBase(
        IList<DatStringEntry> baseEntries,
        IList<DatStringEntry> compareEntries,
        ISet<string> expectedMissingKeys)
    {
        var baseModel = CreateModel(baseEntries);
        var compareModel = CreateModel(compareEntries);

        var missingKeys = Service.GetMissingKeysFromBase(baseModel, compareModel);

        Assert.Equivalent(expectedMissingKeys, missingKeys);
    }

    public static IEnumerable<object[]> GetMissingKeysFromBase_TestData()
    {
        yield return
        [
            // Base
            new List<DatStringEntry>(),
            // Compare
            new List<DatStringEntry>(),
            // Expected
            new HashSet<string>(),
        ];

        yield return
        [
            // Base
            new List<DatStringEntry>
            {
                new("key", new Crc32(1), "value")
            },
            // Compare
            new List<DatStringEntry>(),
            // Expected
            new HashSet<string>{"key"},
        ];
        yield return
        [
            // Base
            new List<DatStringEntry>(),
            // Compare
            new List<DatStringEntry>
            {
                new("key", new Crc32(1), "value")
            },
            // Expected
            new HashSet<string>( ),
        ];
        yield return
        [
            // Base
            new List<DatStringEntry>
            {
                new("key", new Crc32(1), "value")
            },
            // Compare
            new List<DatStringEntry>
            {
                new("key", new Crc32(1), "value")
            },
            // Expected
            new HashSet<string>( ),
        ];
        yield return
        [
            // Base
            new List<DatStringEntry>
            {
                new("key", new Crc32(1), "value"),
                new("key", new Crc32(1), "value1"),
            },
            // Compare
            new List<DatStringEntry>
            {
                new("key", new Crc32(1), "value")
            },
            // Expected
            new HashSet<string>( ),
        ];
        yield return
        [
            // Base
            new List<DatStringEntry>
            {
                new("key", new Crc32(1), "value"),
            },
            // Compare
            new List<DatStringEntry>
            {
                new("key", new Crc32(1), "value"),
                new("key", new Crc32(1), "value1"),
            },
            // Expected
            new HashSet<string>( ),
        ];
        yield return
        [
            // Base
            new List<DatStringEntry>
            {
                new("key1", new Crc32(1), "value1"),
                new("key2", new Crc32(2), "value2"),
            },
            // Compare
            new List<DatStringEntry>
            {
                new("key1", new Crc32(2), "value1")
            },
            // Expected
            new HashSet<string>{"key2"}
        ];
        yield return
        [
            // Base
            new List<DatStringEntry>
            {
                new("key1", new Crc32(1), "value1"),
                new("key2", new Crc32(2), "value2"),
            },
            // Compare
            new List<DatStringEntry>
            {
                new("key3", new Crc32(3), "value3"),
            },
            // Expected
            new HashSet<string>{"key1", "key2"}
        ];
    }
}
