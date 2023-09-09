using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.StarWarsGame.Files.MEG.Binary.V1;

namespace PG.StarWarsGame.Files.MEG.Test.Binary;

[TestClass]
public class MegFileBinaryServiceV1IntegrationTest
{
    private MegFileBinaryServiceV1 _binaryService = null!;

    [TestInitialize]
    public void SetUp()
    {
        var fs = new MockFileSystem();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);
        _binaryService = new MegFileBinaryServiceV1(sp.Object);
    }

    [TestMethod]
    public void Test__ReadBinary_EmptyMeg()
    {
        var emptyMeg = GetEmbeddedResource("Files.v1_empty.meg");
        var megMetadata = _binaryService.ReadBinary(emptyMeg);
        Assert.AreEqual(0, megMetadata.FileNameTable.Count);
        Assert.AreEqual(0, megMetadata.FileTable.Count);
        Assert.AreEqual(0, megMetadata.Header.FileNumber);
    }

    [TestMethod]
    public void Test__ReadBinary_OneFile()
    {
        var emptyMeg = GetEmbeddedResource("Files.v1_1_file_data.meg");
        var megMetadata = _binaryService.ReadBinary(emptyMeg);
        Assert.AreEqual(1, megMetadata.FileNameTable.Count);
        Assert.AreEqual(1, megMetadata.FileTable.Count);
        Assert.AreEqual(1, megMetadata.Header.FileNumber);

        var fileSizes = megMetadata.FileTable.Select(x => x.FileSize).Sum(x => x);
        Assert.AreEqual(3, fileSizes);
    }

    [TestMethod]
    public void Test__ReadBinary_TwoFiles()
    {
        var emptyMeg = GetEmbeddedResource("Files.v1_2_files_empty.meg");
        var megMetadata = _binaryService.ReadBinary(emptyMeg);
        Assert.AreEqual(2, megMetadata.FileNameTable.Count);
        Assert.AreEqual(2, megMetadata.FileTable.Count);
        Assert.AreEqual(2, megMetadata.Header.FileNumber);

        var fileSizes = megMetadata.FileTable.Select(x => x.FileSize).Sum(x => x);
        Assert.AreEqual(0, fileSizes);
    }

    [TestMethod]
    public void Test__ReadBinary_TwoFilesWithNonAsciiName()
    {
        var emptyMeg = GetEmbeddedResource("Files.v1_2_files_with_extended_ascii_name.meg");
        var megMetadata = _binaryService.ReadBinary(emptyMeg);
        Assert.AreEqual(2, megMetadata.FileNameTable.Count);
        Assert.AreEqual(2, megMetadata.FileTable.Count);
        Assert.AreEqual(2, megMetadata.Header.FileNumber);
    }

    [TestMethod]
    public void Test__ReadBinary_TwoFiles2()
    {
        var megMetadata = _binaryService.ReadBinary(new MemoryStream(MegTestConstants.CONTENT_MEG_FILE));
    }

    private static Stream GetEmbeddedResource(string path)
    {
        var currentAssembly = typeof(MegFileBinaryServiceV1IntegrationTest).Assembly;
        return currentAssembly.GetManifestResourceStream($"{currentAssembly.GetName().Name}.Resources.{path}")!;
    }
}