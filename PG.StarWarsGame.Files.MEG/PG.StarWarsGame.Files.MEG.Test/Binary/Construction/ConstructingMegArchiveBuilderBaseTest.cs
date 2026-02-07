using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using PG.Testing.Hashing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Construction;

public abstract class ConstructingMegArchiveBuilderBaseTest : CommonMegTestBase
{ 
    private protected abstract ConstructingMegArchiveBuilderBase CreateBuilder();

    protected abstract int GetExpectedHeaderSize();

    protected abstract MegFileVersion GetExpectedFileVersion();

    protected override void SetupServices(IServiceCollection serviceCollection)
    {
        base.SetupServices(serviceCollection);
        serviceCollection.AddSingleton<ICrc32HashingService>(_ => new ParseIntCrc32HashingService());
    }

    private protected abstract SmallMaxFileSizeConstructingService CreateSmallMaxFileSizeConstructingService(uint maxEntrySize, uint maxFileSize);

    protected abstract uint GetMaxEntrySizeForTooLargeTest(MegDataEntryBuilderInfo entry);

    protected abstract uint GetTotalMegSizeForTooLargeTest(IEnumerable<MegDataEntryBuilderInfo> entries);

    [Fact]
    public void MaxEntryFileSize_IsBinarySize()
    {
        var builder = CreateBuilder();
        Assert.Equal(MegFileConstants.MegMaxEntrySize, builder.MaxEntryFileSize);
    }

    [Fact]
    public void MaxFileSize_IsLibraryMaxSize()
    {
        var builder = CreateBuilder();
        Assert.Equal(MegFileConstants.MegMaxFileSize, builder.MaxFileSize);
    }

    [Fact]
    public void BuildConstructingMegArchive_ThrowsArgs()
    {
        var service = CreateBuilder();
        Assert.Throws<ArgumentNullException>(() => service.BuildConstructingMegArchive(null!));
    }

    [Fact]
    public void BuildConstructingMegArchive_FileNotFound_Throws()
    {
        var service = CreateBuilder();

        FileSystem.File.WriteAllBytes("test.xml", []);

        var builderEntries = new List<MegDataEntryBuilderInfo>
        {
            new(new MegDataEntryOriginInfo(FileSystem.FileInfo.New("test.xml")), "test.xml", false),
        };

        FileSystem.File.Delete("test.xml");

        Assert.Throws<FileNotFoundException>(() => service.BuildConstructingMegArchive(builderEntries));
    }

    [Fact]
    public void BuildConstructingMegArchive_EntryTooLarge_ThrowsMegEntrySizeException()
    {
        FileSystem.File.WriteAllBytes("A", [1, 2, 3, 4, 5, 6]);

        var builderEntries = new List<MegDataEntryBuilderInfo>
        {
            new(new MegDataEntryOriginInfo(FileSystem.FileInfo.New("A")), "0", false),
        };

        var maxEntrySize = GetMaxEntrySizeForTooLargeTest(builderEntries[0]);
        var service = CreateSmallMaxFileSizeConstructingService(maxEntrySize, uint.MaxValue);
        
        Assert.Throws<MegEntrySizeException>(() => service.BuildConstructingMegArchive(builderEntries));
    }

    [Fact]
    public void BuildConstructingMegArchive_TotalMegTooLarge_ThrowsMegSizeException()
    {
        FileSystem.File.WriteAllBytes("A", [1, 2, 3]);
        FileSystem.File.WriteAllBytes("B", [1, 2, 3]);

        var builderEntries = new List<MegDataEntryBuilderInfo>
        {
            new(new MegDataEntryOriginInfo(FileSystem.FileInfo.New("A")), "0", false),
            new(new MegDataEntryOriginInfo(FileSystem.FileInfo.New("B")), "1", false),
        };

        var maxFileSize = GetTotalMegSizeForTooLargeTest(builderEntries);
        var service = CreateSmallMaxFileSizeConstructingService(uint.MaxValue, maxFileSize);
        Assert.Throws<MegSizeException>(() => service.BuildConstructingMegArchive(builderEntries));
    }
    
    [Fact]
    public void BuildConstructingMegArchive_NonASCIITreatment()
    {
        var expectedCrc = new Crc32(0x003F + 0x003F + 0x003F); // \u003F is '?'

        var service = CreateBuilder();

        FileSystem.File.Create("file.meg");

        var entry = MegDataEntryTest.CreateEntry("A", default, 0, 5);
        var megFile = new MegFile(new MegArchive([entry]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);

        var builderEntries = new List<MegDataEntryBuilderInfo>
        {
            MegDataEntryBuilderInfo.FromEntry(megFile, entry, "ÄÖÜ")
        };

        var archive = service.BuildConstructingMegArchive(builderEntries);

        // Check that filename gets encoded
        Assert.Equal("???", archive[0].Path);
        Assert.Equal("ÄÖÜ", archive[0].DataEntry.OriginalPath);
        Assert.Equal("???", archive.Archive[0].Path);
        Assert.Equal("ÄÖÜ", archive.Archive[0].OriginalPath);

        // Ensures that ASCII encoding was used for creating the CRC
        Assert.Equal(expectedCrc, archive.Archive[0].Crc32);
        Assert.Equal(expectedCrc, archive[0].Crc32);

        var calc = ServiceProvider.GetRequiredService<IMegBinaryServiceFactory>()
            .GetMegSizeCalculator(GetExpectedFileVersion());

        Assert.Equal(calc.PreCalculateSize(builderEntries), archive.ExpectedFileSize);
    }

    [Fact]
    public void BuildConstructingMegArchive_RefreshesEntrySize()
    {
        FileSystem.File.WriteAllBytes("A", [1, 2, 3]);

        var service = CreateBuilder();
        var builderEntries = new List<MegDataEntryBuilderInfo>
        {
            new(new MegDataEntryOriginInfo(FileSystem.FileInfo.New("A")), "0", false),
        };

        var calc = ServiceProvider.GetRequiredService<IMegBinaryServiceFactory>()
            .GetMegSizeCalculator(GetExpectedFileVersion());

        var oldSize = calc.PreCalculateSize(builderEntries);

        FileSystem.File.WriteAllBytes("A", [1, 2, 3, 4]);

        var archive = service.BuildConstructingMegArchive(builderEntries);

        var newSize = calc.PreCalculateSize(builderEntries);

        Assert.NotEqual(oldSize, newSize);
        Assert.NotEqual(oldSize, archive.ExpectedFileSize);
    }

    [Theory]
    [MemberData(nameof(ConstructingMegArchiveBuilderTestCollections.MegConstructionTestData_NotEncrypted),
        MemberType = typeof(ConstructingMegArchiveBuilderTestCollections))]
    public void BuildConstructingMegArchive_Normal(ConstructingMegArchiveBuilderTestCollections.ConstructingMegTestData testDataInput)
    {
        var service = CreateBuilder();

        var builderEntries = new List<MegDataEntryBuilderInfo>();
        foreach (var entrySource in testDataInput.BuilderEntries)
        {
            var path = entrySource.OriginPath;

            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
                FileSystem.Directory.CreateDirectory(dir);
            FileSystem.File.WriteAllBytes(path, entrySource.Data);
            builderEntries.Add(MegDataEntryBuilderInfo.FromFile(FileSystem.FileInfo.New(path), entrySource.EntryPath, entrySource.Encrypted));
        }

        if (builderEntries.Any(entry => entry.Encrypted))
            throw new InvalidOperationException("Test does not support encryption");

        Assert.Equal(builderEntries.Count, testDataInput.ExpectedData.Count);

        var calc = ServiceProvider.GetRequiredService<IMegBinaryServiceFactory>()
            .GetMegSizeCalculator(GetExpectedFileVersion());

        var expectedSize = calc.PreCalculateSize(builderEntries);

        var archive = service.BuildConstructingMegArchive(builderEntries);

        Assert.Equal(GetExpectedFileVersion(), archive.MegVersion);
        Assert.Equal(testDataInput.ExpectedData.Count, archive.Count);
        Assert.False(archive.Encrypted);
        Assert.Equal(expectedSize, archive.ExpectedFileSize);

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

            Assert.Equal(expectedData.FilePath, binaryEntry.Path);
            Assert.Equal(expectedData.FilePath, virtualEntry.Path);

            Assert.Equal(expectedData.Crc, binaryEntry.Crc32);
            Assert.Equal(expectedData.Crc, virtualEntry.Crc32);

            Assert.False(binaryEntry.Encrypted);
        }
    }

    internal abstract class SmallMaxFileSizeConstructingService(
        uint maxEntrySize, 
        uint maxFileSize,
        IServiceProvider services) : ConstructingMegArchiveBuilderBase(services)
    {
        internal override uint MaxEntryFileSize => maxEntrySize;

        internal override uint MaxFileSize => maxFileSize;

        protected abstract override MegFileVersion FileVersion { get; }
    }
}