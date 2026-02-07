using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AnakinRaW.CommonUtilities.Testing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Hashing;
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
    private readonly ICrc32HashingService _crc32HashingService;

    public MegFileBinaryReaderV1IntegrationTest()
    {
        _binaryReader = new MegFileBinaryReaderV1(ServiceProvider);
        _crc32HashingService = ServiceProvider.GetRequiredService<ICrc32HashingService>();
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
    public void ReadBinary_TwoFiles2()
    {
        var megMetadata = _binaryReader.ReadBinary(new MemoryStream(MegTestConstants.ContentMegFileV1));

        Assert.Equal("DATA\\XML\\CAMPAIGNFILES.XML", megMetadata.FileNameTable[0].FileName);
        Assert.Equal("DATA\\XML\\CAMPAIGNFILES.XML", megMetadata.FileNameTable[0].OriginalFileName);
        Assert.Equal("DATA\\XML\\GAMEOBJECTFILES.XML", megMetadata.FileNameTable[1].FileName);
        Assert.Equal("DATA\\XML\\GAMEOBJECTFILES.XML", megMetadata.FileNameTable[1].OriginalFileName);
    }

    public static IEnumerable<object[]> MegFilesBetween2GBAnd4GB()
    {
        yield return [new[] { ("FILE1.DAT", (long)OneAndHalfGB), ("FILE2.DAT", OneGB) }];
        yield return [new[] { ("LARGEFILE.DAT", (long)ThreeGB) }];
    }

    [Theory]
    [MemberData(nameof(MegFilesBetween2GBAnd4GB))]
    public void ReadBinary_MegFileBetween2GBAnd4GB_Succeeds((string fileName, long fileSize)[] files)
    {
        var entries = files.Select(f => new MegFileEntry(
            f.fileName,
            f.fileSize,
            _crc32HashingService.GetCrc32(f.fileName, Encoding.ASCII)
        )).ToArray();

        var megData = CreateMeg(entries);
        var fakeLength = megData.Length + files.Sum(f => f.fileSize);
        using var stream = new LargeMegMemoryStream(megData, fakeLength);

        var megMetadata = _binaryReader.ReadBinary(stream);
        Assert.Equal(files.Length, megMetadata.FileTable.Count);
        Assert.Equal(files.Length, megMetadata.Header.FileNumber);
    }

    public static IEnumerable<object[]> MegFilesGreaterThan4GB()
    {
        yield return [new[] { ("FILE1.DAT", (long)ThreeGB), ("FILE2.DAT", TwoGB) }];
        yield return [new[] { ("LARGEFILE.DAT", FiveGB) }];
    }

    [Theory]
    [MemberData(nameof(MegFilesGreaterThan4GB))]
    public void ReadBinary_MegFileGreaterThan4GB_ThrowsMegSizeException((string fileName, long fileSize)[] files)
    {
        var entries = files.Select(f => new MegFileEntry(
            f.fileName,
            f.fileSize,
            _crc32HashingService.GetCrc32(f.fileName, Encoding.ASCII)
        )).ToArray();

        var megData = CreateMeg(entries);
        var fakeLength = megData.Length + files.Sum(f => f.fileSize);
        using var stream = new LargeMegMemoryStream(megData, fakeLength);

        var exception = Assert.Throws<MegSizeException>(() => _binaryReader.ReadBinary(stream));
        Assert.Contains("4GB", exception.Message, StringComparison.OrdinalIgnoreCase);
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