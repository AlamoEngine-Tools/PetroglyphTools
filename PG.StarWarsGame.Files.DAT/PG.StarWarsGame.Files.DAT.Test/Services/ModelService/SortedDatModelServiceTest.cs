using System.Collections.Generic;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Services;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services;

public class SortedDatModelServiceTest : DatModelServiceTest
{
    private ISortedDatModel CreateSorted(IList<DatStringEntry> entries)
    {
        var sorted = Crc32Utilities.SortByCrc32(entries);
        return new SortedDatModel(sorted);
    }

    protected override IDatModel CreateModel(IList<DatStringEntry> entries)
    {
        return CreateSorted(entries);
    }


    [Theory]
    [MemberData(nameof(MergeKeepExistingTestData))]
    public void Test_MergeSorted_KeepExisting(
        IList<DatStringEntry> baseEntries,
        IList<DatStringEntry> mergeEntries,
        IList<DatStringEntry> expected,
        ICollection<MergedKeyResult> expectedMergedKeyResults)
    {
        var baseModel = CreateSorted(baseEntries);
        var toMergeModel = CreateSorted(mergeEntries);

        var merged = Service.MergeSorted(baseModel, toMergeModel, out var mergedEntries, SortedDatMergeOptions.KeepExisting);

        Assert.Equal(expected, merged);
        Assert.Equivalent(expectedMergedKeyResults, mergedEntries);
    }

    public static IEnumerable<object[]> MergeKeepExistingTestData()
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
            new List<DatStringEntry> { new("1", new Crc32(1), "a") },
            new List<DatStringEntry> { new("1", new Crc32(1), "b") },
            new List<DatStringEntry> { new("1", new Crc32(1), "a") },
            new List<MergedKeyResult>()
        ];

        yield return
        [
            new List<DatStringEntry>
            {
                new("1", new Crc32(1), "a"),
                new("1", new Crc32(1), "b")
            },
            new List<DatStringEntry>
            {
                new("0", new Crc32(0), "x"),
                new("0", new Crc32(0), "y"),
                new("1", new Crc32(1), "f"),
                new("2", new Crc32(2), "c"),
            },
            new List<DatStringEntry>
            {
                new("0", new Crc32(0), "x"),
                new("1", new Crc32(1), "a"),
                new("1", new Crc32(1), "b"),
                new("2", new Crc32(2), "c"),

            },
            new List<MergedKeyResult>
            {
                new(new("0", new Crc32(0), "x")),
                new(new("2", new Crc32(2), "c")),
            }
        ];
    }

    [Theory]
    [MemberData(nameof(MergeOverwriteTestData))]
    public void Test_MergeSorted_Overwrite(
        IList<DatStringEntry> baseEntries,
        IList<DatStringEntry> mergeEntries,
        IList<DatStringEntry> expected,
        ICollection<MergedKeyResult> expectedMergedKeyResults)
    {
        var baseModel = CreateSorted(baseEntries);
        var toMergeModel = CreateSorted(mergeEntries);

        var merged = Service.MergeSorted(baseModel, toMergeModel, out var mergedEntries, SortedDatMergeOptions.Overwrite);

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
            new List<DatStringEntry> { new("1", new Crc32(1), "a") },
            new List<DatStringEntry> { new("1", new Crc32(1), "b") },
            new List<DatStringEntry> { new("1", new Crc32(1), "b") },
            new List<MergedKeyResult> { new(new("1", new Crc32(1), "b"), new("1", new Crc32(1), "a")) }
        ];

        yield return
        [
            new List<DatStringEntry>
            {
                new("1", new Crc32(1), "a"),
                new("1", new Crc32(1), "b")
            },
            new List<DatStringEntry>
            {
                new("0", new Crc32(0), "x"),
                new("0", new Crc32(0), "y"),
                new("1", new Crc32(1), "f"),
                new("2", new Crc32(2), "c"),
            },
            new List<DatStringEntry>
            {
                new("0", new Crc32(0), "x"),
                new("1", new Crc32(1), "f"),
                new("1", new Crc32(1), "b"),
                new("2", new Crc32(2), "c"),

            },
            new List<MergedKeyResult>
            {
                new(new("0", new Crc32(0), "x")),
                new(new("1", new Crc32(1), "f"), new("1", new Crc32(1), "a")),
                new(new("2", new Crc32(2), "c")),
            }
        ];
    }
}