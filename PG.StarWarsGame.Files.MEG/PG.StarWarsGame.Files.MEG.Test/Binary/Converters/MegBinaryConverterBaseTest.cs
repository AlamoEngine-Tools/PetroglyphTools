using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Binary;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Converters;

[TestClass]
public class MegBinaryConverterBaseTest
{
    private Mock<IServiceProvider> _serviceProviderMock = null!;
    private IFileSystem _fileSystem = null!;

    [TestInitialize]
    public void SetUp()
    {
        _fileSystem = new MockFileSystem();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(_fileSystem);
        _serviceProviderMock = sp;
    }

    [TestMethod]
    public void Test__BinaryToModel_ThrowsArgs()
    {
        var serviceMock = new Mock<MegBinaryConverterBase<IMegFileMetadata>>(_serviceProviderMock.Object)
        {
            CallBase = true
        };

        Assert.ThrowsException<ArgumentNullException>(() => serviceMock.Object.BinaryToModel(null!));
    }

    [TestMethod]
    public void Test__BinaryToModel_EmptyModel()
    {
        var serviceMock = new Mock<MegBinaryConverterBase<IMegFileMetadata>>(_serviceProviderMock.Object)
        {
            CallBase = true
        };

        var bin = new Mock<IMegFileMetadata>();
        var header = new Mock<IMegHeader>();

        header.SetupGet(h => h.FileNumber).Returns(0);
        bin.SetupGet(b => b.Header).Returns(header.Object);

        var model = serviceMock.Object.BinaryToModel(bin.Object);
        Assert.AreEqual(0, model.Count);
    }

    [TestMethod]
    public void Test__BinaryToModel_ModelWithFiles()
    {
        var serviceMock = new Mock<MegBinaryConverterBase<IMegFileMetadata>>(_serviceProviderMock.Object)
        {
            CallBase = true
        };

        var bin = new Mock<IMegFileMetadata>();
        var header = new Mock<IMegHeader>();
        var fileTable = new Mock<IMegFileTable>();
        var fileNameTable = new Mock<IMegFileNameTable>();

        var fileDescriptor1 = new Mock<IMegFileDescriptor>();
        fileDescriptor1.SetupGet(d => d.Crc32).Returns(new Crc32(0));
        fileDescriptor1.SetupGet(d => d.FileNameIndex).Returns(1);

        var fileDescriptor2 = new Mock<IMegFileDescriptor>();
        fileDescriptor2.SetupGet(d => d.Crc32).Returns(new Crc32(1));
        fileDescriptor2.SetupGet(d => d.FileNameIndex).Returns(0);

        header.SetupGet(h => h.FileNumber).Returns(2);
        bin.SetupGet(b => b.Header).Returns(header.Object);
        bin.SetupGet(b => b.FileTable).Returns(fileTable.Object);
        bin.SetupGet(b => b.FileNameTable).Returns(fileNameTable.Object);
        fileTable.SetupGet(t => t[0]).Returns(fileDescriptor1.Object);
        fileTable.SetupGet(t => t[1]).Returns(fileDescriptor2.Object);

        fileNameTable.SetupGet(t => t[0]).Returns("B");
        fileNameTable.SetupGet(t => t[1]).Returns("A");


        var model = serviceMock.Object.BinaryToModel(bin.Object);
        Assert.AreEqual(2, model.Count);

        Assert.AreEqual("A", model[0].FilePath);
        Assert.AreEqual(new Crc32(0), model[0].Crc32);
        Assert.AreEqual("B", model[1].FilePath);
        Assert.AreEqual(new Crc32(1), model[1].Crc32);
    }

    [TestMethod]
    [ExpectedException(typeof(BinaryCorruptedException))]
    public void Test__BinaryToModel_ThrowsCrcNotSorted()
    {
        var serviceMock = new Mock<MegBinaryConverterBase<IMegFileMetadata>>(_serviceProviderMock.Object)
        {
            CallBase = true
        };

        var bin = new Mock<IMegFileMetadata>();
        var header = new Mock<IMegHeader>();
        var fileTable = new Mock<IMegFileTable>();
        var fileNameTable = new Mock<IMegFileNameTable>();

        var fileDescriptor1 = new Mock<IMegFileDescriptor>();
        fileDescriptor1.SetupGet(d => d.Crc32).Returns(new Crc32(0x99));
        fileDescriptor1.SetupGet(d => d.FileNameIndex).Returns(1);

        var fileDescriptor2 = new Mock<IMegFileDescriptor>();
        fileDescriptor2.SetupGet(d => d.Crc32).Returns(new Crc32(0x01));
        fileDescriptor2.SetupGet(d => d.FileNameIndex).Returns(0);

        header.SetupGet(h => h.FileNumber).Returns(2);
        bin.SetupGet(b => b.Header).Returns(header.Object);
        bin.SetupGet(b => b.FileTable).Returns(fileTable.Object);
        bin.SetupGet(b => b.FileNameTable).Returns(fileNameTable.Object);
        fileTable.SetupGet(t => t[0]).Returns(fileDescriptor1.Object);
        fileTable.SetupGet(t => t[1]).Returns(fileDescriptor2.Object);

        fileNameTable.SetupGet(t => t[0]).Returns("B");
        fileNameTable.SetupGet(t => t[1]).Returns("A");


        serviceMock.Object.BinaryToModel(bin.Object);
    }
}