using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using PG.Testing;
using PG.Testing.Hashing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Construction;

public abstract class ConstructingMegArchiveBuilderBaseTest
{
    protected readonly IServiceProvider ServiceProvider;
    protected readonly MockFileSystem FileSystem = new();
    
    private protected abstract ConstructingMegArchiveBuilderBase CreateService();

    protected abstract int GetExpectedHeaderSize();

    protected abstract MegFileVersion GetExpectedFileVersion();

    protected ConstructingMegArchiveBuilderBaseTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(FileSystem);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        PetroglyphCommons.ContributeServices(sc);
        sc.SupportMEG();

        sc.AddSingleton<ICrc32HashingService>(_ => new ParseIntCrc32HashingService());

        ServiceProvider = sc.BuildServiceProvider();
    }

    [Fact]
    public void MaxEntryFileSize_Is4GB()
    {
        var builder = CreateService();
        Assert.Equal(uint.MaxValue, builder.MaxEntryFileSize);
    }

    [Fact]
    public void Test_BuildConstructingMegArchive_ThrowsArgs()
    {
        var service = CreateService();
        Assert.Throws<ArgumentNullException>(() => service.BuildConstructingMegArchive(null!));
    }

    [Fact]
    public void Test_BuildConstructingMegArchive_FileNotFound_Throws()
    {
        var service = CreateService();
        var builderEntries = new List<MegFileDataEntryBuilderInfo>
        {
            new(new MegDataEntryOriginInfo("A"), "0"),
        };
        Assert.Throws<FileNotFoundException>(() => service.BuildConstructingMegArchive(builderEntries));
    }

    [Fact]
    public void Test_BuildConstructingMegArchive_FileTooLarge_Throws()
    {
        const uint maxFileSize = 6u;

        FileSystem.File.WriteAllBytes("A", [1, 2, 3, 4, 5, 6, 7]);

        var service = new SmallMaxFileSizeConstructingService(maxFileSize, ServiceProvider);
        var builderEntries = new List<MegFileDataEntryBuilderInfo>
        {
            new(new MegDataEntryOriginInfo("A"), "0"),
        };
        Assert.Throws<NotSupportedException>(() => service.BuildConstructingMegArchive(builderEntries));
    }

    [Fact]
    public void Test_BuildConstructingMegArchive_BinarySizeOverflows_Throws()
    {
        var service = new BinarySizeOverflowingConstructingService(ServiceProvider);

        FileSystem.File.Create("file.meg");
        var megFile = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);

        var builderEntries = new List<MegFileDataEntryBuilderInfo>
        {
            new(new MegDataEntryOriginInfo(
                    new MegDataEntryLocationReference(
                        megFile,
                        MegDataEntryTest.CreateEntry("A", default, 0, 5)
                    )),
                "0")
        };

        Assert.Throws<InvalidOperationException>(() => service.BuildConstructingMegArchive(builderEntries));
    }

    [Fact]
    public void Test_BuildConstructingMegArchive_NonASCIITreatment()
    {
        var expectedCrc = new Crc32(63 + 63 + 63); // 63 == '?'

        var service = CreateService();

        FileSystem.File.Create("file.meg");
        var megFile = new MegFile(new MegArchive([]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);

        var builderEntries = new List<MegFileDataEntryBuilderInfo>
        {
            new(new MegDataEntryOriginInfo(
                    new MegDataEntryLocationReference(
                        megFile,
                        MegDataEntryTest.CreateEntry("A", default, 0, 5)
                    )),
                "ÄÖÜ")
        };

        var archive = service.BuildConstructingMegArchive(builderEntries);

        // Check that filename gets encoded
        Assert.Equal("???", archive[0].FilePath);
        Assert.Equal("ÄÖÜ", archive[0].DataEntry.OriginalFilePath);
        Assert.Equal("???", archive.Archive[0].FilePath);
        Assert.Equal("ÄÖÜ", archive.Archive[0].OriginalFilePath);

        // Ensures that ASCII encoding was used for creating the CRC
        Assert.Equal(expectedCrc, archive.Archive[0].Crc32);
        Assert.Equal(expectedCrc, archive[0].Crc32);
    }

    [Fact]
    public void Test_BuildConstructingMegArchive_GetSizeAtRuntime()
    {
        var testData = "test data";
        FileSystem.Initialize().WithFile("A").Which(m => m.HasStringContent(testData));
        var service = CreateService();
        var builderEntries = new List<MegFileDataEntryBuilderInfo>
        {
            new(new MegDataEntryOriginInfo("A"), "0"),
        };

        var archive = service.BuildConstructingMegArchive(builderEntries);
        Assert.Equal(testData.Length, (int)archive.Archive[0].Location.Size);
    }

    [Theory]
    [MemberData(nameof(MegConstructionTestData_NotEncrypted), MemberType = typeof(ConstructingMegArchiveBuilderBaseTest))]
    public void Test_BuildConstructingMegArchive_Normal(ConstructingMegTestData testDataInput)
    {
        Assert.Equal(testDataInput.BuilderEntries.Count(), testDataInput.ExpectedData.Count);
        
        // Prepare FileSystem
        foreach (var entry in testDataInput.BuilderEntries)
        {
            if (entry.Size is null)
                throw new InvalidOperationException("Test requires fixed size entries");
            if (entry.Encrypted)
                throw new InvalidOperationException("Test does not support encryption");

            if (entry.OriginInfo.IsLocalFile)
            {
                FileSystem.Initialize().WithFile(entry.OriginInfo.FilePath)
                    .Which(m => m.HasStringContent(TestUtility.GetRandomStringOfLength((int)entry.Size)));
            }
        }
        
        var service = CreateService();

        var archive = service.BuildConstructingMegArchive(testDataInput.BuilderEntries);
        
        Assert.Equal(GetExpectedFileVersion(), archive.MegVersion);
        Assert.Equal(testDataInput.ExpectedData.Count, archive.Count);
        Assert.False(archive.Encrypted);

        Crc32Utilities.EnsureSortedByCrc32(archive.Archive);

        var expectedHeaderSize = GetExpectedHeaderSize();

        for (var i = 0; i < archive.Count; i++)
        {
            var virtualEntry = archive[i];
            var binaryEntry = archive.Archive[i];
            var expectedData = testDataInput.ExpectedData[i];

            Assert.Equal(expectedData.Size, binaryEntry.Location.Size);

            var expectedAbsoluteOffset = expectedHeaderSize + expectedData.RelativeOffset;
            Assert.Equal(expectedAbsoluteOffset, binaryEntry.Location.Offset);
            
            Assert.Equal(expectedData.FilePath, binaryEntry.FilePath);
            Assert.Equal(expectedData.FilePath, virtualEntry.FilePath);
            
            Assert.Equal(expectedData.Crc, binaryEntry.Crc32);
            Assert.Equal(expectedData.Crc, virtualEntry.Crc32);
            
            Assert.False(binaryEntry.Encrypted);
        }
    }

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

    public static string GetTestDisplayNames(MethodInfo _, object[] values)
    {
        return ((ConstructingMegTestData)values[0]).TestName;
    }


    private static ConstructingMegTestData EmptyMeg()
    {
        return new(
            nameof(EmptyMeg),
            // Input
            new List<MegFileDataEntryBuilderInfo>(),
            // Expected
            new List<ExpectedEntryData>()
        );
    }

    private static ConstructingMegTestData SingleFileMeg()
    {
        // (N * 2 + Ni(length)) + (N * 20)
        const int fileNameAndFileTableSize = 3 + 20;
        
        return new(
            nameof(SingleFileMeg),
            // Input
            new List<MegFileDataEntryBuilderInfo>
            {
                new(new MegDataEntryOriginInfo("A"), "0", 3)
            },
            // Expected
            new List<ExpectedEntryData>
            {
                new("0", new Crc32(48),3, fileNameAndFileTableSize + 0)
            }
        );
    }

    private static ConstructingMegTestData UnsortedWithDuplicateCrcDueToNonASCIIFilePath()
    {
        // (N * 2 + Ni(length)) + (N * 20)
        const int fileNameAndFileTableSize = 9 + 60;

        return new(
            nameof(UnsortedWithDuplicateCrcDueToNonASCIIFilePath),
            // Input
            new List<MegFileDataEntryBuilderInfo>
            {
                new(new MegDataEntryOriginInfo("A"), "0", 3),
                new(new MegDataEntryOriginInfo("B"), "1", 1),
                new(new MegDataEntryOriginInfo("C"), "0", 6),

            },
            // Expected
            new List<ExpectedEntryData>
            {
                new("0", new Crc32(48), 3, fileNameAndFileTableSize + 0),
                new("0", new Crc32(48), 6, fileNameAndFileTableSize + 3),
                new("1", new Crc32(49), 1, fileNameAndFileTableSize + 3 + 6),
            }
        );
    }

    private static ConstructingMegTestData OnlyTwoEmptyFiles()
    {
        // (N * 2 + Ni(length)) + (N * 20)
        const int fileNameAndFileTableSize = 6 + 40;

        return new(
            nameof(OnlyTwoEmptyFiles),
            // Input
            new List<MegFileDataEntryBuilderInfo>
            {
                new(new MegDataEntryOriginInfo("A"), "1", 0),
                new(new MegDataEntryOriginInfo("B"), "0", 0)
            },
            // Expected
            new List<ExpectedEntryData>
            {
                new("0", new Crc32(48),0, fileNameAndFileTableSize + 0),
                new("1", new Crc32(49),0, fileNameAndFileTableSize + 0),
            }
        );
    }

    private static ConstructingMegTestData TwoEmptyFilesFirstThenData()
    {
        // (N * 2 + Ni(length)) + (N * 20)
        const int fileNameAndFileTableSize = 9 + 60;

        return new(
            nameof(TwoEmptyFilesFirstThenData),
            // Input
            new List<MegFileDataEntryBuilderInfo>
            {
                new(new MegDataEntryOriginInfo("A"), "1", 0),
                new(new MegDataEntryOriginInfo("B"), "2", 0),
                new(new MegDataEntryOriginInfo("C"), "3", 3),
            },
            // Expected
            new List<ExpectedEntryData>
            {
                new("1", new Crc32(49),0, fileNameAndFileTableSize + 0),
                new("2", new Crc32(50),0, fileNameAndFileTableSize + 0),
                new("3", new Crc32(51),3, fileNameAndFileTableSize + 0),
            }
        );
    }

    private static ConstructingMegTestData DataThenTwoEmptyFiles()
    {
        // (N * 2 + Ni(length)) + (N * 20)
        const int fileNameAndFileTableSize = 9 + 60;

        return new(
            nameof(DataThenTwoEmptyFiles),
            // Input
            new List<MegFileDataEntryBuilderInfo>
            {
                new(new MegDataEntryOriginInfo("A"), "1", 3),
                new(new MegDataEntryOriginInfo("B"), "2", 0),
                new(new MegDataEntryOriginInfo("C"), "3", 0),
            },
            // Expected
            new List<ExpectedEntryData>
            {
                new("1", new Crc32(49),3, fileNameAndFileTableSize + 0),
                new("2", new Crc32(50),0, fileNameAndFileTableSize + 3),
                new("3", new Crc32(51),0, fileNameAndFileTableSize + 3),
            }
        );
    }

    private static ConstructingMegTestData DataThenEmptyThenData()
    {
        // (N * 2 + Ni(length)) + (N * 20)
        const int fileNameAndFileTableSize = 9 + 60;

        return new(
            nameof(DataThenEmptyThenData),
            // Input
            new List<MegFileDataEntryBuilderInfo>
            {
                new(new MegDataEntryOriginInfo("A"), "1", 3),
                new(new MegDataEntryOriginInfo("B"), "2", 0),
                new(new MegDataEntryOriginInfo("C"), "3", 3),
            },
            // Expected
            new List<ExpectedEntryData>
            {
                new("1", new Crc32(49),3, fileNameAndFileTableSize + 0),
                new("2", new Crc32(50),0, fileNameAndFileTableSize + 3),
                new("3", new Crc32(51),3, fileNameAndFileTableSize + 3),
            }
        );
    }


    public readonly struct ConstructingMegTestData(string testName, IEnumerable<MegFileDataEntryBuilderInfo> builderEntries, IList<ExpectedEntryData> expectedData)
    {
        public IEnumerable<MegFileDataEntryBuilderInfo> BuilderEntries { get; } = builderEntries;
        public IList<ExpectedEntryData> ExpectedData { get; } = expectedData;
        public string TestName { get; } = testName;
    }
    
    public readonly struct ExpectedEntryData(string filePath, Crc32 crc, uint size, uint relativeOffset)
    {
        public string FilePath { get; } = filePath;
        public Crc32 Crc { get; } = crc;
        public uint Size { get; } = size;
        
        // Offset with (FileNameTable, FileTable) size but without header size.
        public uint RelativeOffset { get; } = relativeOffset;
    }

    private class SmallMaxFileSizeConstructingService(uint maxEntrySize, IServiceProvider services) : ConstructingMegArchiveBuilderBase(services)
    {
        internal override uint MaxEntryFileSize => maxEntrySize;

        protected override MegFileVersion FileVersion => MegFileVersion.V1;

        protected override int GetFileDescriptorSize(bool entryGetsEncrypted)
        {
            return MEG.Binary.Metadata.V1.MegFileTableRecord.SizeValue;
        }

        protected override int GetHeaderSize()
        {
            return MEG.Binary.Metadata.V1.MegHeader.SizeValue;
        }
    }

    private class BinarySizeOverflowingConstructingService(IServiceProvider services) : ConstructingMegArchiveBuilderBase(services)
    {
        protected override MegFileVersion FileVersion => MegFileVersion.V1;
        
        protected override int GetFileDescriptorSize(bool entryGetsEncrypted) => 0;
        protected override int GetHeaderSize() => 0;

        protected override uint GetBinarySize(uint dataSize, bool entryGetsEncrypted)
        {
            if (dataSize == 0)
                throw new InvalidOperationException();
            unchecked
            {
                return dataSize + uint.MaxValue;
            }
        }
    }
}