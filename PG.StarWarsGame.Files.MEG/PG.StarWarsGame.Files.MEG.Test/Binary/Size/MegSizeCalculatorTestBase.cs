using System;
using System.Collections.Generic;
using System.Linq;
using PG.StarWarsGame.Files.MEG.Binary.Size;
using PG.StarWarsGame.Files.MEG.Data;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Size;

public abstract class MegSizeCalculatorTestBase : CommonMegTestBase
{
    private protected abstract IMegSizeCalculator CreateCalculator();

    protected abstract uint ExpectedHeaderSize { get; }

    protected abstract bool SupportsEncryption { get; }

    protected abstract uint GetExpectedFileTableRecordSize(MegDataEntryBuilderInfo entry);

    public static IEnumerable<object[]> UnencryptedTestCases
    {
        get
        {
            yield return
            [
                new SizeTestCase(
                    "EmptyEntry",
                    [new EntryDefinition("a", 0)],
                    3u,
                    0u)
            ];
            yield return
            [
                new SizeTestCase(
                    "SingleEntry",
                    [new EntryDefinition("a", 10)],
                    3u,
                    10u)
            ];
            yield return
            [
                new SizeTestCase(
                    "MultipleEntries",
                    [
                        new EntryDefinition("bb", 5),
                        new EntryDefinition("ccc", 1)
                    ],
                    9u,
                    6u)
            ];

            {
                var random = new Random(1138);
                const int smokeCount = 1000;
                var smokeEntries = new List<EntryDefinition>();
                uint expectedFileDataSize = 0;
                uint expectedRawFilenameTableSize = 0;
                for (var i = 0; i < smokeCount; i++)
                {
                    var path = $"file_{i}.txt";
                    var size = (uint)i * 1000 * (uint)random.NextDouble();
                    smokeEntries.Add(new EntryDefinition(path, size));
                    expectedFileDataSize += size;
                    expectedRawFilenameTableSize += (uint)(path.Length + 2);
                }

                yield return
                [
                    new SizeTestCase(
                        "SmokeManyEntries",
                        smokeEntries,
                        expectedRawFilenameTableSize,
                        expectedFileDataSize)
                ];
            }
        }
    }

    [Fact]
    public void InitialState()
    {
        var calculator = CreateCalculator();
        Assert.Equal(ExpectedHeaderSize, calculator.CurrentSize);
        Assert.Equal(ExpectedHeaderSize, calculator.MetadataSize);
    }

    [Fact]
    public void Reset_ResetsToInitialState()
    {
        var calculator = CreateCalculator();
        var entry = CreateEntry("test", 100);
        calculator.AddEntry(entry);
        
        Assert.NotEqual(ExpectedHeaderSize, calculator.CurrentSize);

        calculator.Reset();

        Assert.Equal(ExpectedHeaderSize, calculator.CurrentSize);
        Assert.Equal(ExpectedHeaderSize, calculator.MetadataSize);
    }

    [Fact]
    public void PreCalculateSize_SingleEntry()
    {
        var calculator = CreateCalculator();
        var entry = CreateEntry("test", 100);

        var preCalculated = calculator.PreCalculateSize(entry);
        calculator.AddEntry(entry);

        Assert.Equal(calculator.CurrentSize, preCalculated);
    }

    [Fact]
    public void PreCalculateSize_MultipleEntries()
    {
        var calculator = CreateCalculator();
        var entry1 = CreateEntry("test1", 100);
        var entry2 = CreateEntry("test2", 200);
        var entries = new[] { entry1, entry2 };

        var preCalculated = calculator.PreCalculateSize(entries);
        calculator.AddEntry(entry1);
        calculator.AddEntry(entry2);

        Assert.Equal(calculator.CurrentSize, preCalculated);
    }

    [Fact]
    public void PreCalculateSize_SingleEntry_WithExistingState()
    {
        var calculator = CreateCalculator();
        var entry1 = CreateEntry("test1", 10);
        var entry2 = CreateEntry("test2", 5);

        calculator.AddEntry(entry1);
        var preCalculated = calculator.PreCalculateSize(entry2);
        calculator.AddEntry(entry2);

        Assert.Equal(calculator.CurrentSize, preCalculated);
    }

    [Fact]
    public void PreCalculateSize_MultipleEntries_WithExistingState()
    {
        var calculator = CreateCalculator();
        var entry1 = CreateEntry("test1", 10);
        var entry2 = CreateEntry("test2", 5);
        var entry3 = CreateEntry("test3", 7);

        calculator.AddEntry(entry1);
        var preCalculated = calculator.PreCalculateSize([entry2, entry3]);
        calculator.AddEntry(entry2);
        calculator.AddEntry(entry3);

        Assert.Equal(calculator.CurrentSize, preCalculated);
    }

    [Fact]
    public void PreCalculateSize_DoesNotUpdateState()
    {
        var calculator = CreateCalculator();
        var entry1 = CreateEntry("state1", 10);
        var entry2 = CreateEntry("state2", 5);
        var entry3 = CreateEntry("state3", 7);

        calculator.AddEntry(entry1);

        var expectedCurrentSize = calculator.CurrentSize;
        var expectedMetadataSize = calculator.MetadataSize;

        calculator.PreCalculateSize(entry2);
        calculator.PreCalculateSize([entry2, entry3]);

        Assert.Equal(expectedCurrentSize, calculator.CurrentSize);
        Assert.Equal(expectedMetadataSize, calculator.MetadataSize);
    }

    [Fact]
    public void PreCalculateSize_MultipleCalls_MatchesEnumerablePrecalc()
    {
        var entry1 = CreateEntry("bulk1", 10);
        var entry2 = CreateEntry("bulk2", 5);
        var entry3 = CreateEntry("bulk3", 7);
        var entries = new[] { entry1, entry2, entry3 };

        var calculator = CreateCalculator();
        calculator.PreCalculateSize(entry1);
        calculator.AddEntry(entry1);
        calculator.PreCalculateSize(entry2);
        calculator.AddEntry(entry2);
        var multiplePrecalc = calculator.PreCalculateSize(entry3);

        var enumerablePrecalc = CreateCalculator().PreCalculateSize(entries);

        Assert.Equal(enumerablePrecalc, multiplePrecalc);
    }

    [Fact]
    public void AddEntry_UpdatesSizes()
    {
        var calculator = CreateCalculator();
        var entry = CreateEntry("a", 10);
        
        var expectedRecordSize = GetExpectedFileTableRecordSize(entry);
        const int expectedFileNameSize = 2 + 1; // uint16 length + "a"
        const uint expectedFileSize = 10u;

        var expectedMetadataSize = ExpectedHeaderSize + expectedFileNameSize + expectedRecordSize;
        var expectedTotalSize = expectedMetadataSize + expectedFileSize;

        calculator.AddEntry(entry);

        Assert.Equal(expectedMetadataSize, calculator.MetadataSize);
        Assert.Equal(expectedTotalSize, calculator.CurrentSize);
    }

    [Fact]
    public void EncryptedEntry_ThrowsNotSupportedException()
    {
        if (SupportsEncryption)
            return;
        var calculator = CreateCalculator();
        var entry = CreateEntry("test", 1, true);

        Assert.Throws<System.NotSupportedException>(() => calculator.PreCalculateSize(entry));
        Assert.Throws<System.NotSupportedException>(() => calculator.PreCalculateSize([entry]));
        Assert.Throws<System.NotSupportedException>(() => calculator.AddEntry(entry));
    }

    [Theory]
    [MemberData(nameof(UnencryptedTestCases))]
    public void CalculateSize_Unencrypted_MatchesExpected(SizeTestCase testCase)
    {
        var calculator = CreateCalculator();
        var entries = testCase.Entries
            .Select(entry => CreateEntry(entry.Path, entry.Size))
            .ToArray();

        var expectedFileTableSize = (ulong)entries.Length * GetExpectedFileTableRecordSize(entries[0]);
        var expectedMetadataSize = ExpectedHeaderSize + testCase.ExpectedRelativeMetadataSize + expectedFileTableSize;
        var expectedTotalSize = expectedMetadataSize + testCase.ExpectedFileDataSize;

        var preCalculated = calculator.PreCalculateSize(entries);
        foreach (var entry in entries)
            calculator.AddEntry(entry);

        Assert.Equal(expectedMetadataSize, calculator.MetadataSize);
        Assert.Equal(expectedTotalSize, calculator.CurrentSize);
        Assert.Equal(expectedTotalSize, preCalculated);
        
        calculator.Reset();

        foreach (var entry in entries)
            calculator.AddEntry(entry);

        Assert.Equal(expectedTotalSize, calculator.CurrentSize);
    }

    protected MegDataEntryBuilderInfo CreateEntry(string path, uint size, bool encrypted = false)
    {
        var file = FileSystem.FileInfo.New(path);
        FileSystem.File.WriteAllBytes(file.FullName, new byte[size]);

        return MegDataEntryBuilderInfo.FromFile(file, path, encrypted);
    }

    public readonly record struct EntryDefinition(string Path, uint Size);

    public readonly record struct SizeTestCase(
        string Name,
        IReadOnlyCollection<EntryDefinition> Entries,
        uint ExpectedRelativeMetadataSize,
        uint ExpectedFileDataSize)
    {
        public override string ToString() => Name;
    }
}
