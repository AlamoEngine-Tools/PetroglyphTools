using System.Collections.Generic;
using PG.Commons.Hashing;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Construction;

public static class ConstructingMegArchiveBuilderTestCollections
{
    public static IEnumerable<object[]> MegConstructionTestData_NotEncrypted()
    {
        yield return [EmptyMeg()];
        yield return [SingleFileMeg()];
        yield return [UnsortedWithDuplicateCrcDueToNonASCIIFilePath()];
        yield return [OnlyTwoEmptyFiles()];
        yield return [TwoEmptyFilesFirstThenData()];
        yield return [DataThenTwoEmptyFiles()];
        yield return [DataThenEmptyThenData()];
    }

    public static ConstructingMegTestData EmptyMeg()
    {
        return new ConstructingMegTestData(
            nameof(EmptyMeg),
            // Input
            [],
            // Expected
            []
        );
    }

    public static ConstructingMegTestData SingleFileMeg()
    {
        // (N * 2 + Ni(length)) + (N * 20)
        const int fileNameAndFileTableSize = 3 + 20;

        return new ConstructingMegTestData(
            nameof(SingleFileMeg),
            // Input
            [
                new DataEntryBuilderSource("A", [48, 48, 48], "0", false)
            ],
            // Expected
            [
                new ExpectedEntryData("0", new Crc32(48), 3, fileNameAndFileTableSize + 0)
            ]
        );
    }

    public static ConstructingMegTestData UnsortedWithDuplicateCrcDueToNonASCIIFilePath()
    {
        // (N * 2 + Ni(length)) + (N * 20)
        const int fileNameAndFileTableSize = 9 + 60;

        return new ConstructingMegTestData(
            nameof(UnsortedWithDuplicateCrcDueToNonASCIIFilePath),
            // Input
            [
                new DataEntryBuilderSource("A", [1, 2, 3], "0", false),
                new DataEntryBuilderSource("B", [1], "1", false),
                new DataEntryBuilderSource("C", [1, 2, 3, 4, 5, 6], "0", false),
            ],
            // Expected
            [
                new ExpectedEntryData("0", new Crc32(48), 3, fileNameAndFileTableSize + 0),
                new ExpectedEntryData("0", new Crc32(48), 6, fileNameAndFileTableSize + 3),
                new ExpectedEntryData("1", new Crc32(49), 1, fileNameAndFileTableSize + 3 + 6),
            ]
        );
    }

    public static ConstructingMegTestData OnlyTwoEmptyFiles()
    {
        // (N * 2 + Ni(length)) + (N * 20)
        const int fileNameAndFileTableSize = 6 + 40;

        return new ConstructingMegTestData(
            nameof(OnlyTwoEmptyFiles),
            // Input
            [
                new DataEntryBuilderSource("A", [], "1", false),
                new DataEntryBuilderSource("B", [], "0", false)
            ],
            // Expected
            [
                new ExpectedEntryData("0", new Crc32(48), 0, fileNameAndFileTableSize + 0),
                new ExpectedEntryData("1", new Crc32(49), 0, fileNameAndFileTableSize + 0),
            ]
        );
    }

    public static ConstructingMegTestData TwoEmptyFilesFirstThenData()
    {
        // (N * 2 + Ni(length)) + (N * 20)
        const int fileNameAndFileTableSize = 9 + 60;

        return new ConstructingMegTestData(
            nameof(TwoEmptyFilesFirstThenData),
            // Input
            [
                new DataEntryBuilderSource("A", [], "1", false),
                new DataEntryBuilderSource("B", [], "2", false),
                new DataEntryBuilderSource("C", [1, 2, 3], "3", false),
            ],
            // Expected
            [
                new ExpectedEntryData("1", new Crc32(49), 0, fileNameAndFileTableSize + 0),
                new ExpectedEntryData("2", new Crc32(50), 0, fileNameAndFileTableSize + 0),
                new ExpectedEntryData("3", new Crc32(51), 3, fileNameAndFileTableSize + 0),
            ]
        );
    }

    public static ConstructingMegTestData DataThenTwoEmptyFiles()
    {
        // (N * 2 + Ni(length)) + (N * 20)
        const int fileNameAndFileTableSize = 9 + 60;

        return new ConstructingMegTestData(
            nameof(DataThenTwoEmptyFiles),
            // Input
            [
                new DataEntryBuilderSource("A", [1, 2, 3], "1", false),
                new DataEntryBuilderSource("B", [], "2", false),
                new DataEntryBuilderSource("C", [], "3", false),
            ],
            // Expected
            [
                new ExpectedEntryData("1", new Crc32(49), 3, fileNameAndFileTableSize + 0),
                new ExpectedEntryData("2", new Crc32(50), 0, fileNameAndFileTableSize + 3),
                new ExpectedEntryData("3", new Crc32(51), 0, fileNameAndFileTableSize + 3),
            ]
        );
    }

    public static ConstructingMegTestData DataThenEmptyThenData()
    {
        // (N * 2 + Ni(length)) + (N * 20)
        const int fileNameAndFileTableSize = 9 + 60;

        return new ConstructingMegTestData(
            nameof(DataThenEmptyThenData),
            // Input
            [
                new DataEntryBuilderSource("A", [1, 2, 3], "1", false),
                new DataEntryBuilderSource("B", [], "2", false),
                new DataEntryBuilderSource("C", [1, 2, 3], "3", false),
            ],
            // Expected
            [
                new ExpectedEntryData("1", new Crc32(49), 3, fileNameAndFileTableSize + 0),
                new ExpectedEntryData("2", new Crc32(50), 0, fileNameAndFileTableSize + 3),
                new ExpectedEntryData("3", new Crc32(51), 3, fileNameAndFileTableSize + 3),
            ]
        );
    }

    public readonly struct ConstructingMegTestData(
        string testName,
        IEnumerable<DataEntryBuilderSource> builderEntries,
        IList<ExpectedEntryData> expectedData)
    {
        public IEnumerable<DataEntryBuilderSource> BuilderEntries { get; } = builderEntries;
        public IList<ExpectedEntryData> ExpectedData { get; } = expectedData;
        public string TestName { get; } = testName;

        public override string ToString() => TestName;
    }

    public readonly struct DataEntryBuilderSource(string originPath, byte[] data, string entryPath, bool encrypted)
    {
        public string OriginPath { get; } = originPath;
        public byte[] Data { get; } = data;
        public string EntryPath { get; } = entryPath;
        public bool Encrypted { get; } = encrypted;
    }

    public readonly struct ExpectedEntryData(string filePath, Crc32 crc, uint size, uint relativeOffset)
    {
        public string FilePath { get; } = filePath;
        public Crc32 Crc { get; } = crc;
        public uint Size { get; } = size;

        // Offset with (FileNameTable, FileTable) size but without header size.
        public uint RelativeOffset { get; } = relativeOffset;
    }
}
