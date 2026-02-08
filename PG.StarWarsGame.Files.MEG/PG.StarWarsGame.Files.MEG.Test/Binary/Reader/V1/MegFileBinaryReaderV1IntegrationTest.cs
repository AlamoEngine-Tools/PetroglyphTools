using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AnakinRaW.CommonUtilities.Testing;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary.V1;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Reader.V1;

public class MegFileBinaryReaderV1IntegrationTest : CommonMegTestBase
{
    // ReSharper disable InconsistentNaming
    private const uint OneAndHalfGB = 1536u * 1024 * 1024;
    private const uint OneGB = 1024u * 1024 * 1024;
    private const long TwoGB = 2L * 1024 * 1024 * 1024;
    private const uint ThreeGB = 3u * 1024 * 1024 * 1024;
    private const long FiveGB = 5L * 1024 * 1024 * 1024;
    // ReSharper restore InconsistentNaming

    private readonly MegFileBinaryReaderV1 _binaryReader;

    public MegFileBinaryReaderV1IntegrationTest()
    {
        _binaryReader = new MegFileBinaryReaderV1(ServiceProvider);
    }

    [Fact]
    public void ReadBinary_EmptyMeg()
    {
        var emptyMeg = TestingHelpers.GetEmbeddedResource(typeof(MegFileBinaryReaderV1IntegrationTest), "Files.v1_empty.meg");
        var megMetadata = _binaryReader.ReadBinary(emptyMeg);
        Assert.Empty(megMetadata.FileNameTable);
        Assert.Empty(megMetadata.FileTable);
        Assert.Equal(0, megMetadata.Header.FileNumber);
    }

    [Fact]
    public void ReadBinary_OneFile()
    {
        var emptyMeg = TestingHelpers.GetEmbeddedResource(typeof(MegFileBinaryReaderV1IntegrationTest), "Files.v1_1_file_data.meg");
        var megMetadata = _binaryReader.ReadBinary(emptyMeg);
        Assert.Single(megMetadata.FileNameTable);
        Assert.Single(megMetadata.FileTable);
        Assert.Equal(1, megMetadata.Header.FileNumber);

        var fileSizes = megMetadata.FileTable.Select(x => x.FileSize).Sum(x => x);
        Assert.Equal(3, fileSizes);

        Assert.Equal("TEST.TXT", megMetadata.FileNameTable[0].FileName);
        Assert.Equal("TEST.TXT", megMetadata.FileNameTable[0].OriginalFileName);
        Assert.Equal(3u, megMetadata.FileTable[0].FileSize);
    }

    [Fact]
    public void ReadBinary_TwoFiles()
    {
        var emptyMeg = TestingHelpers.GetEmbeddedResource(typeof(MegFileBinaryReaderV1IntegrationTest), "Files.v1_2_files_empty.meg");
        var megMetadata = _binaryReader.ReadBinary(emptyMeg);
        Assert.Equal(2, megMetadata.FileNameTable.Count);
        Assert.Equal(2, megMetadata.FileTable.Count);
        Assert.Equal(2, megMetadata.Header.FileNumber);

        var fileSizes = megMetadata.FileTable.Select(x => x.FileSize).Sum(x => x);
        Assert.Equal(0, fileSizes);
    }

    [Fact]
    public void ReadBinary_TwoFilesWithNonAsciiName()
    {
        var emptyMeg = TestingHelpers.GetEmbeddedResource(typeof(MegFileBinaryReaderV1IntegrationTest), "Files.v1_2_files_with_extended_ascii_name.meg");
        var megMetadata = _binaryReader.ReadBinary(emptyMeg);
        Assert.Equal(2, megMetadata.FileNameTable.Count);
        Assert.Equal(2, megMetadata.FileTable.Count);
        Assert.Equal(2, megMetadata.Header.FileNumber);

        Assert.Equal("TEST?.TXT", megMetadata.FileNameTable[0].FileName);
        Assert.Equal("TESTü.TXT", megMetadata.FileNameTable[0].OriginalFileName);
        Assert.Equal("TEST?.TXT", megMetadata.FileNameTable[1].FileName);
        Assert.Equal("TESTä.TXT", megMetadata.FileNameTable[1].OriginalFileName);

        // Not equal, cause MIKE uses Latin1 and thus CRC32 is calculated on the original file name, 
        Assert.NotEqual(megMetadata.FileTable[0].Crc32, megMetadata.FileTable[1].Crc32);
    }

    [Fact]
    public void ReadBinary_UnsortedEntryContent()
    {
        var unorderedMeg = TestingHelpers.GetEmbeddedResource(typeof(MegFileBinaryReaderV1IntegrationTest), "Files.v1_out_of_order.meg");
        var megMetadata = _binaryReader.ReadBinary(unorderedMeg);
        Assert.Equal(2, megMetadata.FileNameTable.Count);
        Assert.Equal(2, megMetadata.FileTable.Count);
        Assert.Equal(2, megMetadata.Header.FileNumber);

        Assert.Equal("FileB.txt", megMetadata.FileNameTable[0].FileName);
        Assert.Equal("FileA.txt", megMetadata.FileNameTable[1].FileName);

        // Content of first entry starts after content of second entry.
        Assert.True(megMetadata.FileTable[0].FileOffset > megMetadata.FileTable[1].FileOffset);
    }


    [Fact]
    public void ReadBinary_TwoFiles2()
    {
        var megMetadata = _binaryReader.ReadBinary(new MemoryStream(MegTestConstants.ContentMegFileV1));

        Assert.Equal("DATA\\XML\\CAMPAIGNFILES.XML", megMetadata.FileNameTable[0].FileName);
        Assert.Equal("DATA\\XML\\CAMPAIGNFILES.XML", megMetadata.FileNameTable[0].OriginalFileName);
        Assert.Equal("DATA\\XML\\GAMEOBJECTFILES.XML", megMetadata.FileNameTable[1].FileName);
        Assert.Equal("DATA\\XML\\GAMEOBJECTFILES.XML", megMetadata.FileNameTable[1].OriginalFileName);
    }

    public static IEnumerable<object[]> LargeButValidMegsReadTestData()
    {
        yield return [new[] { ("FILE1.DAT", (long)OneAndHalfGB), ("FILE2.DAT", OneGB) }];
        yield return [new[] { ("FILE1.DAT", (long)ThreeGB) }];
        yield return [new[] { ("FILE1.DAT", (long)ThreeGB), ("FILE2.DAT", TwoGB) }];
        // FILE1 Size: uint.MaxValue - 70 means FILE2 starts exactly at uint.MaxValue
        yield return [new[] { ("FILE1.DAT", (long)uint.MaxValue - 70), ("FILE2.DAT", uint.MaxValue) }];
    }

    [Theory]
    [MemberData(nameof(LargeButValidMegsReadTestData))]
    public void ReadBinary_LargeButValidToReadMegs((string fileName, long fileSize)[] files)
    {
        var entries = files.Select((f, i) => new MegFileEntry(
            f.fileName,
            f.fileSize,
            new Crc32(i)
        )).ToArray();

        var megData = CreateMeg(entries);
        var fakeLength = megData.Length + files.Sum(f => f.fileSize);
        using var stream = new LargeMegMemoryStream(megData, fakeLength);

        var megMetadata = _binaryReader.ReadBinary(stream);
        Assert.Equal(files.Length, megMetadata.FileTable.Count);
        Assert.Equal(files.Length, megMetadata.Header.FileNumber);
    }

    [Fact]
    public void ReadBinary_OutOfOrderFileContents_Succeeds()
    {
        // Create MEG with records sorted by CRC (B, A)
        // But file contents will be A then B (out of record order)
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        // Header
        writer.Write(2u);
        writer.Write(2u);

        // Filename Table
        writer.Write((ushort)5);
        writer.Write("B.TXT"u8.ToArray());
        writer.Write((ushort)5);
        writer.Write("A.TXT"u8.ToArray());

        var metadataSize = (uint)ms.Position + 20 * 2;

        // Record for B (CRC 1)
        writer.Write(1u); // CRC
        writer.Write(0u); // Index
        writer.Write(20u); // Size
        writer.Write(metadataSize + 10u); // Offset (after A)
        writer.Write(0u); // Name Index

        // Record for A (CRC 2)
        writer.Write(2u); // CRC
        writer.Write(1u); // Index
        writer.Write(10u); // Size
        writer.Write(metadataSize); // Offset (before B)
        writer.Write(1u); // Name Index

        var megData = ms.ToArray();
        var fakeLength = megData.Length + 30;
        using var stream = new LargeMegMemoryStream(megData, fakeLength);

        var megMetadata = _binaryReader.ReadBinary(stream);
        Assert.Equal(2, megMetadata.FileTable.Count);
    }


    // The following test cases produce invalid Metadata due to uint overflows
    public static IEnumerable<object[]> MegFilesGreaterThan4GB_Corrupt()
    {
        yield return [new[] { ("LARGEFILE.DAT", FiveGB) }];

        // Two files, where the second one starts at an offset that overflows uint32.
        // File 1: 3GB
        // File 2: 2GB
        // Metadata: ~100 bytes
        // Expected File 2 Offset: ~3GB + 100 bytes (fits in uint32)
        // Expected Archive Size: ~5GB + 100 bytes:
        // File 3: 1GB
        // Expected File 3 Offset: ~5GB + 100 bytes -> overflows to ~1GB + 100 bytes.
        yield return [new[]
        {
            ("FILE1.DAT", (long)ThreeGB), 
            ("FILE2.DAT", TwoGB),
            ("FILE3.DAT", OneGB)
        }];

        // 5 files of 1GB each.
        yield return [Enumerable.Range(1, 5).Select(i => ($"FILE{i}.DAT", (long)OneGB)).ToArray()];

        // Case where offsets wrap around but technically look "ordered" if not careful.
        // File 1: 4GB - 200 bytes
        // File 2: 300 bytes
        // Total: 4GB + 100 bytes
        // Offset 1: ~100 bytes
        // Offset 2: (~100 + 4GB - 200) = 4GB - 100 bytes (fits in uint32)
        // Offset 3: (4GB - 100 + 300) = 4GB + 200 -> 200 bytes (wraps!)
        yield return [new[]
        {
            ("F1.DAT", 4L * 1024 * 1024 * 1024 - 200),
            ("F2.DAT", 300L),
            ("F3.DAT", 100L)
        }];
    }

    [Theory]
    [MemberData(nameof(MegFilesGreaterThan4GB_Corrupt))]
    public void ReadBinary_MegFileGreaterThan4GB_CorruptMetadata_ThrowsBinaryCorruptedException((string fileName, long fileSize)[] input)
    {
        var entries = input.Select((f, i) => new MegFileEntry(
            f.fileName,
            f.fileSize,
            new Crc32(i)
        )).ToArray();

        var megData = CreateMeg(entries);
        var fakeLength = megData.Length + input.Sum(f => f.fileSize);
        using var stream = new LargeMegMemoryStream(megData, fakeLength);

        Assert.Throws<BinaryCorruptedException>(() => _binaryReader.ReadBinary(stream));
    }

    [Fact]
    public void ReadBinary_GapsBetweenFiles_ThrowsBinaryCorruptedException()
    {
        var entries = new[]
        {
            new MegFileEntry("A.TXT", 10, new Crc32(1)),
        };

        var megData = CreateMeg(entries);
        var fakeLength = megData.Length + 10 + 1; // 1 byte gap
        using var stream = new LargeMegMemoryStream(megData, fakeLength);

        Assert.Throws<BinaryCorruptedException>(() => _binaryReader.ReadBinary(stream));
    }

    [Fact]
    public void ReadBinary_OverlappingFiles_ThrowsBinaryCorruptedException()
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        writer.Write(2u);
        writer.Write(2u);
        
        writer.Write((ushort)5);
        writer.Write("A.TXT"u8.ToArray());
        writer.Write((ushort)5);
        writer.Write("B.TXT"u8.ToArray());

        var metadataSize = (uint)ms.Position + 20 * 2;
        
        // Record for A
        writer.Write(1u); writer.Write(0u); writer.Write(10u); writer.Write(metadataSize); writer.Write(0u);
        // Record for B - overlaps with A
        writer.Write(2u); writer.Write(1u); writer.Write(10u); writer.Write(metadataSize + 5u); writer.Write(1u);

        var megData = ms.ToArray();
        var fakeLength = megData.Length + 15; // Total size if A+B overlap by 5
        using var stream = new LargeMegMemoryStream(megData, fakeLength);

        Assert.Throws<BinaryCorruptedException>(() => _binaryReader.ReadBinary(stream));
    }
    
    [Fact]
    public void ReadBinary_FilenameTableOrderDiffersFromFileTableOrder()
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        writer.Write(2u); 
        writer.Write(2u);

        // Filename Table: [B, A]
        writer.Write((ushort)5);
        writer.Write("B.TXT"u8.ToArray());
        writer.Write((ushort)5);
        writer.Write("A.TXT"u8.ToArray());

        var metadataEndOffset = (uint)ms.Position + 20 * 2;

        // Record for A
        writer.Write(0x100u);
        writer.Write(0u);
        writer.Write(10u);
        writer.Write(metadataEndOffset);
        writer.Write(1u);      // Index in Filename Table (A.txt is at index 1)
        
        // Record for B
        writer.Write(0x200u);
        writer.Write(1u);
        writer.Write(20u);
        writer.Write(metadataEndOffset + 10u);
        writer.Write(0u);      // Index in Filename Table (B.txt is at index 0)
        
        // Data for A and B
        writer.Write(new byte[10 + 20]);

        var megData = ms.ToArray();
        using var stream = new MemoryStream(megData);
        
        var metadata = _binaryReader.ReadBinary(stream);
        
        Assert.Equal(2, metadata.FileTable.Count);
        
        // Record 0 should be A
        Assert.Equal(0x100u, (uint)metadata.FileTable[0].Crc32);
        Assert.Equal(1, metadata.FileTable[0].FileNameIndex);
        Assert.Equal("A.TXT", metadata.FileNameTable[metadata.FileTable[0].FileNameIndex].FileName);

        // Record 1 should be B
        Assert.Equal(0x200u, (uint)metadata.FileTable[1].Crc32);
        Assert.Equal(0, metadata.FileTable[1].FileNameIndex);
        Assert.Equal("B.TXT", metadata.FileNameTable[metadata.FileTable[1].FileNameIndex].FileName);
    }

    private static byte[] CreateMeg(params MegFileEntry[] files)
    {
        var numFiles = (uint)files.Length;
        
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        
        writer.Write(numFiles);
        writer.Write(numFiles);
        
        foreach (var file in files)
        {
            writer.Write((ushort)file.FileName.Length);
            writer.Write(Encoding.ASCII.GetBytes(file.FileName));
        }
        
        var metadataEndOffset = (uint)ms.Position + 20 * numFiles;
        
        var currentOffset = metadataEndOffset;
        for (uint i = 0; i < files.Length; i++)
        {
            writer.Write((uint)files[i].Crc);
            writer.Write(i);
            writer.Write((uint)files[i].FileSize);
            writer.Write(currentOffset);
            writer.Write(i);
            currentOffset += (uint)files[i].FileSize;
        }
        return ms.ToArray();
    }

    private readonly record struct MegFileEntry(string FileName, long FileSize, Crc32 Crc);

    private class LargeMegMemoryStream(byte[] data, long fakeLength) : MemoryStream(data, false)
    {
        public override long Length => fakeLength;
    }
}