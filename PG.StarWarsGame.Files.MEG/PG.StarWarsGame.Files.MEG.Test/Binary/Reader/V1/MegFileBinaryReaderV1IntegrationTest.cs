using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Binary.V1;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Reader.V1;

public class MegFileBinaryReaderV1IntegrationTest
{
    private readonly MegFileBinaryReaderV1 _binaryReader;
    private readonly MockFileSystem _fileSystem = new();

    public MegFileBinaryReaderV1IntegrationTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        sc.SupportMEG();
        _binaryReader = new MegFileBinaryReaderV1(sc.BuildServiceProvider());
    }

    [Fact]
    public void ReadBinary_EmptyMeg()
    {
        var emptyMeg = TestUtility.GetEmbeddedResource(typeof(MegFileBinaryReaderV1IntegrationTest), "Files.v1_empty.meg");
        var megMetadata = _binaryReader.ReadBinary(emptyMeg);
        Assert.Empty(megMetadata.FileNameTable);
        Assert.Empty(megMetadata.FileTable);
        Assert.Equal(0, megMetadata.Header.FileNumber);
    }

    [Fact]
    public void ReadBinary_OneFile()
    {
        var emptyMeg = TestUtility.GetEmbeddedResource(typeof(MegFileBinaryReaderV1IntegrationTest), "Files.v1_1_file_data.meg");
        var megMetadata = _binaryReader.ReadBinary(emptyMeg);
        Assert.Single(megMetadata.FileNameTable);
        Assert.Single(megMetadata.FileTable);
        Assert.Equal(1, megMetadata.Header.FileNumber);

        var fileSizes = megMetadata.FileTable.Select(x => x.FileSize).Sum(x => x);
        Assert.Equal(3, fileSizes);

        Assert.Equal("TEST.TXT", megMetadata.FileNameTable[0].FileName);
        Assert.Equal("TEST.TXT", megMetadata.FileNameTable[0].OriginalFilePath);
        Assert.Equal(3u, megMetadata.FileTable[0].FileSize);
    }

    [Fact]
    public void ReadBinary_TwoFiles()
    {
        var emptyMeg = TestUtility.GetEmbeddedResource(typeof(MegFileBinaryReaderV1IntegrationTest), "Files.v1_2_files_empty.meg");
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
        var emptyMeg = TestUtility.GetEmbeddedResource(typeof(MegFileBinaryReaderV1IntegrationTest), "Files.v1_2_files_with_extended_ascii_name.meg");
        var megMetadata = _binaryReader.ReadBinary(emptyMeg);
        Assert.Equal(2, megMetadata.FileNameTable.Count);
        Assert.Equal(2, megMetadata.FileTable.Count);
        Assert.Equal(2, megMetadata.Header.FileNumber);

        Assert.Equal("TEST?.TXT", megMetadata.FileNameTable[0].FileName);
        Assert.Equal("TESTü.TXT", megMetadata.FileNameTable[0].OriginalFilePath);
        Assert.Equal("TEST?.TXT", megMetadata.FileNameTable[1].FileName);
        Assert.Equal("TESTä.TXT", megMetadata.FileNameTable[1].OriginalFilePath);

        // Not equal, cause MIKE uses Latin1 and thus CRC32 is calculated on the original file name, 
        Assert.NotEqual(megMetadata.FileTable[0].Crc32, megMetadata.FileTable[1].Crc32);
    }

    [Fact]
    public void ReadBinary_TwoFiles2()
    {
        var megMetadata = _binaryReader.ReadBinary(new MemoryStream(MegTestConstants.ContentMegFileV1));

        Assert.Equal("DATA\\XML\\CAMPAIGNFILES.XML", megMetadata.FileNameTable[0].FileName);
        Assert.Equal("DATA\\XML\\CAMPAIGNFILES.XML", megMetadata.FileNameTable[0].OriginalFilePath);
        Assert.Equal("DATA\\XML\\GAMEOBJECTFILES.XML", megMetadata.FileNameTable[1].FileName);
        Assert.Equal("DATA\\XML\\GAMEOBJECTFILES.XML", megMetadata.FileNameTable[1].OriginalFilePath);
    }
}