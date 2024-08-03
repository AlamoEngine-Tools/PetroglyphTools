using System.Collections.Generic;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Services;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services;

public class UnsortedDatModelServiceTest : DatModelServiceTest
{
    private IUnsortedDatModel CreateUnsorted(IList<DatStringEntry> entries)
    {
        return new UnsortedDatModel(entries);
    }

    protected override IDatModel CreateModel(IList<DatStringEntry> entries)
    {
        return CreateUnsorted(entries);
    }

    [Theory]
    [MemberData(nameof(MergeByIndexTestData))]
    public void Test_MergeUnsorted_ByIndex(
        IList<DatStringEntry> baseEntries, 
        IList<DatStringEntry> mergeEntries, 
        IList<DatStringEntry> expected,
        ICollection<MergedKeyResult> expectedMergedKeyResults)
    {
        var baseModel = CreateUnsorted(baseEntries);
        var toMergeModel = CreateUnsorted(mergeEntries);

        var merged = Service.MergeUnsorted(baseModel, toMergeModel, out var mergedEntries, UnsortedDatMergeOptions.ByIndex);

        Assert.Equal(expected, merged);
        Assert.Equivalent(expectedMergedKeyResults, mergedEntries);
    }

    public static IEnumerable<object[]> MergeByIndexTestData()
    {
        yield return
        [
            new List<DatStringEntry>(),
            new List<DatStringEntry>(),
            new List<DatStringEntry>(),
            new List<MergedKeyResult>()
        ];
        yield return
        [
            new List<DatStringEntry> { new("key", new Crc32(1), "value") },
            new List<DatStringEntry>(),
            new List<DatStringEntry> { new("key", new Crc32(1), "value") },
            new List<MergedKeyResult>()
        ];
        yield return
        [
            new List<DatStringEntry>(),
            new List<DatStringEntry> { new("key", new Crc32(1), "value") },
            new List<DatStringEntry> { new("key", new Crc32(1), "value") },
            new List<MergedKeyResult> { new(new("key", new Crc32(1), "value")) }
        ];
        yield return
        [
            new List<DatStringEntry> { new("key", new Crc32(1), "value1") },
            new List<DatStringEntry> { new("key", new Crc32(1), "value2") },
            new List<DatStringEntry> { new("key", new Crc32(1), "value2") },
            new List<MergedKeyResult>
            {
                new(new("key", new Crc32(1), "value2"),
                    new("key", new Crc32(1), "value1"))
            }
        ];
        yield return
        [
            new List<DatStringEntry> { new("key1", new Crc32(1), "value1") },
            new List<DatStringEntry> { new("key2", new Crc32(2), "value2") },
            new List<DatStringEntry> { new("key2", new Crc32(2), "value2") },
            new List<MergedKeyResult>
            {
                new(new("key2", new Crc32(2), "value2"),
                    new("key1", new Crc32(1), "value1"))
            }
        ];

        yield return
        [
            new List<DatStringEntry>
            {
                new("key1", new Crc32(1), "value1a"),
                new("key2", new Crc32(2), "value2a"),

            },
            new List<DatStringEntry>
            {
                new("key2", new Crc32(2), "value2b"),
                new("key1", new Crc32(1), "value1b"),
                new("key3", new Crc32(3), "value3"),

            },
            new List<DatStringEntry>
            {
                new("key2", new Crc32(2), "value2b"),
                new("key1", new Crc32(1), "value1b"),
                new("key3", new Crc32(3), "value3")

            },
            new List<MergedKeyResult>
            {
                new(new("key2", new Crc32(2), "value2b"),
                    new("key1", new Crc32(1), "value1a")),

                new(new("key1", new Crc32(1), "value1b"),
                    new("key2", new Crc32(2), "value2a")),

                new(new("key3", new Crc32(3), "value3")),
            }
        ];
    }

    [Theory]
    [MemberData(nameof(MergeOverwriteTestData))]
    public void Test_MergeUnsorted_Overwrite(
        IList<DatStringEntry> baseEntries,
        IList<DatStringEntry> mergeEntries,
        IList<DatStringEntry> expected,
        ICollection<MergedKeyResult> expectedMergedKeyResults)
    {
        var baseModel = CreateUnsorted(baseEntries);
        var toMergeModel = CreateUnsorted(mergeEntries);

        var merged = Service.MergeUnsorted(baseModel, toMergeModel, out var mergedEntries, UnsortedDatMergeOptions.Overwrite);

        Assert.Equal(expected, merged);
        Assert.Equivalent(expectedMergedKeyResults, mergedEntries);
    }

    public static IEnumerable<object[]> MergeOverwriteTestData()
    {
        yield return
        [
            new List<DatStringEntry>(),
            new List<DatStringEntry>(),
            new List<DatStringEntry>(),
            new List<MergedKeyResult>()
        ];
        yield return
        [
            new List<DatStringEntry> { new("key", new Crc32(1), "value") },
            new List<DatStringEntry>(),
            new List<DatStringEntry> { new("key", new Crc32(1), "value") },
            new List<MergedKeyResult>()
        ];

        yield return
        [
            new List<DatStringEntry>(),
            new List<DatStringEntry> { new("key", new Crc32(1), "value") },
            new List<DatStringEntry> { new("key", new Crc32(1), "value") },
            new List<MergedKeyResult> { new(new("key", new Crc32(1), "value")) }
        ];
        yield return
        [
            new List<DatStringEntry>
            {
                new("0", new Crc32(0), "a"),
                new("1", new Crc32(1), "b"),
                new("2", new Crc32(2), "a")

            },
            new List<DatStringEntry>
            {
                new("1", new Crc32(1), "x")
            },
            new List<DatStringEntry>
            {
                new("0", new Crc32(0), "a"),
                new("1", new Crc32(1), "x"),
                new("2", new Crc32(2), "a")
            },
            new List<MergedKeyResult>
            {
                new(new("1", new Crc32(1), "x"), new("1", new Crc32(1), "b"))
            }
        ];
        yield return
        [
            new List<DatStringEntry>
            {
                new("1", new Crc32(1), "a"),
                new("1", new Crc32(1), "b"),
                new("2", new Crc32(2), "c"),
                new("1", new Crc32(1), "d"),
            },
            new List<DatStringEntry>
            {
                new("1", new Crc32(1), "b"),
                new("1", new Crc32(1), "c"),
                new("1", new Crc32(1), "e"),
                new("2", new Crc32(2), "d"),
                new("3", new Crc32(3), "f"),
                new("1", new Crc32(1), "i"),
            },
            new List<DatStringEntry>
            {
                new("1", new Crc32(1), "b"),
                new("1", new Crc32(1), "c"),
                new("2", new Crc32(2), "c"),
                new("1", new Crc32(1), "e"),
                new("2", new Crc32(2), "d"),
                new("3", new Crc32(3), "f"),
                new("1", new Crc32(1), "i"),
            },
            new List<MergedKeyResult>
            {
                new(new("1", new Crc32(1), "b"), new("1", new Crc32(1), "a")),
                new(new("1", new Crc32(1), "c"), new("1", new Crc32(1), "b")),
                new(new("1", new Crc32(1), "e"), new("1", new Crc32(1), "d")),
                new(new("2", new Crc32(2), "d")),
                new(new("3", new Crc32(3), "f")),
                new(new("1", new Crc32(1), "i")),
            }
        ];
    }

    [Theory]
    [MemberData(nameof(MergeAppendTestData))]
    public void Test_MergeUnsorted_Append(
        IList<DatStringEntry> baseEntries,
        IList<DatStringEntry> mergeEntries,
        IList<DatStringEntry> expected,
        ICollection<MergedKeyResult> expectedMergedKeyResults)
    {
        var baseModel = CreateUnsorted(baseEntries);
        var toMergeModel = CreateUnsorted(mergeEntries);

        var merged = Service.MergeUnsorted(baseModel, toMergeModel, out var mergedEntries, UnsortedDatMergeOptions.Append);

        Assert.Equal(expected, merged);
        Assert.Equivalent(expectedMergedKeyResults, mergedEntries);
    }

    public static IEnumerable<object[]> MergeAppendTestData()
    {
        yield return
        [
            new List<DatStringEntry>(),
            new List<DatStringEntry>(),
            new List<DatStringEntry>(),
            new List<MergedKeyResult>()
        ];
        yield return
        [
            new List<DatStringEntry> { new("key", new Crc32(1), "value") },
            new List<DatStringEntry>(),
            new List<DatStringEntry> { new("key", new Crc32(1), "value") },
            new List<MergedKeyResult>()
        ];
        yield return
        [
            new List<DatStringEntry>(),
            new List<DatStringEntry> { new("key", new Crc32(1), "value") },
            new List<DatStringEntry> { new("key", new Crc32(1), "value") },
            new List<MergedKeyResult> { new(new("key", new Crc32(1), "value")) }
        ];
        yield return
        [
            new List<DatStringEntry> { new("key", new Crc32(1), "value") },
            new List<DatStringEntry> { new("key", new Crc32(1), "value") },
            new List<DatStringEntry>
            {
                new("key", new Crc32(1), "value"),
                new("key", new Crc32(1), "value"),
            },
            new List<MergedKeyResult> { new(new("key", new Crc32(1), "value")) }
        ];
    }
}