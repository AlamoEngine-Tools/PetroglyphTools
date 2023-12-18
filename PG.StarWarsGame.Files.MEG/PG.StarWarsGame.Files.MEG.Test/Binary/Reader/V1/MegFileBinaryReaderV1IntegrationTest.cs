using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.StarWarsGame.Files.MEG.Binary.V1;
using PG.Testing;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Reader.V1;

[TestClass]
public class MegFileBinaryReaderV1IntegrationTest
{
    private MegFileBinaryReaderV1 _binaryReader = null!;

    [TestInitialize]
    public void SetUp()
    {
        var fs = new MockFileSystem();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);
        _binaryReader = new MegFileBinaryReaderV1(sp.Object);
    }

    [TestMethod]
    public void Test__ReadBinary_EmptyMeg()
    {
        var emptyMeg = TestUtility.GetEmbeddedResource(typeof(MegFileBinaryReaderV1IntegrationTest), "Files.v1_empty.meg");
        var megMetadata = _binaryReader.ReadBinary(emptyMeg);
        Assert.AreEqual(0, megMetadata.FileNameTable.Count);
        Assert.AreEqual(0, megMetadata.FileTable.Count);
        Assert.AreEqual(0, megMetadata.Header.FileNumber);
    }

    [TestMethod]
    public void Test__ReadBinary_OneFile()
    {
        var emptyMeg = TestUtility.GetEmbeddedResource(typeof(MegFileBinaryReaderV1IntegrationTest), "Files.v1_1_file_data.meg");
        var megMetadata = _binaryReader.ReadBinary(emptyMeg);
        Assert.AreEqual(1, megMetadata.FileNameTable.Count);
        Assert.AreEqual(1, megMetadata.FileTable.Count);
        Assert.AreEqual(1, megMetadata.Header.FileNumber);

        var fileSizes = megMetadata.FileTable.Select(x => x.FileSize).Sum(x => x);
        Assert.AreEqual(3, fileSizes);

        Assert.AreEqual("TEST.TXT", megMetadata.FileNameTable[0].FileName);
        Assert.AreEqual("TEST.TXT", megMetadata.FileNameTable[0].OriginalFileName);
        Assert.AreEqual(3u, megMetadata.FileTable[0].FileSize);
    }

    [TestMethod]
    public void Test__ReadBinary_TwoFiles()
    {
        var emptyMeg = TestUtility.GetEmbeddedResource(typeof(MegFileBinaryReaderV1IntegrationTest), "Files.v1_2_files_empty.meg");
        var megMetadata = _binaryReader.ReadBinary(emptyMeg);
        Assert.AreEqual(2, megMetadata.FileNameTable.Count);
        Assert.AreEqual(2, megMetadata.FileTable.Count);
        Assert.AreEqual(2, megMetadata.Header.FileNumber);

        var fileSizes = megMetadata.FileTable.Select(x => x.FileSize).Sum(x => x);
        Assert.AreEqual(0, fileSizes);
    }

    [TestMethod]
    public void Test__ReadBinary_TwoFilesWithNonAsciiName()
    {
        var emptyMeg = TestUtility.GetEmbeddedResource(typeof(MegFileBinaryReaderV1IntegrationTest), "Files.v1_2_files_with_extended_ascii_name.meg");
        var megMetadata = _binaryReader.ReadBinary(emptyMeg);
        Assert.AreEqual(2, megMetadata.FileNameTable.Count);
        Assert.AreEqual(2, megMetadata.FileTable.Count);
        Assert.AreEqual(2, megMetadata.Header.FileNumber);

        Assert.AreEqual("TEST?.TXT", megMetadata.FileNameTable[0].FileName);
        Assert.AreEqual("TESTü.TXT", megMetadata.FileNameTable[0].OriginalFileName);
        Assert.AreEqual("TEST?.TXT", megMetadata.FileNameTable[1].FileName);
        Assert.AreEqual("TESTä.TXT", megMetadata.FileNameTable[1].OriginalFileName);

        // Not equal, cause MIKE uses Latin1 and thus CRC32 is calculated on the original file name, 
        Assert.AreNotEqual(megMetadata.FileTable[0].Crc32, megMetadata.FileTable[1].Crc32);
    }

    [TestMethod]
    public void Test__ReadBinary_TwoFiles2()
    {
        var megMetadata = _binaryReader.ReadBinary(new MemoryStream(MegTestConstants.CONTENT_MEG_FILE_V1));

        Assert.AreEqual("DATA/XML/GAMEOBJECTFILES.XML", megMetadata.FileNameTable[0].FileName);
        Assert.AreEqual("DATA/XML/GAMEOBJECTFILES.XML", megMetadata.FileNameTable[0].OriginalFileName);
        Assert.AreEqual("DATA/XML/CAMPAIGNFILES.XML", megMetadata.FileNameTable[1].FileName);
        Assert.AreEqual("DATA/XML/CAMPAIGNFILES.XML", megMetadata.FileNameTable[1].OriginalFileName);
    }
}