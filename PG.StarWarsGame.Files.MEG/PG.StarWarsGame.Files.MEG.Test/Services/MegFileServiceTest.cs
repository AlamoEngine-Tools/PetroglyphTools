using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Binary;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

// Note: It's not possible to use Span<T> with Moq. Thus we cannot test encrypted meg files using it.
// This needs to be tested by integration tests.     
[TestClass]
public class MegFileServiceTest
{
    private MegFileService _megFileService = null!;
    private readonly Mock<IServiceProvider> _serviceProvider = new();
    private readonly MockFileSystem _fileSystem = new();
    private readonly Mock<IMegBinaryServiceFactory> _binaryServiceFactory = new();
    private readonly Mock<IMegFileBinaryReader> _megBinaryReader = new();
    private readonly Mock<IMegFileSizeValidator> _sizeValidator = new();
    private readonly Mock<IMegVersionIdentifier> _versionIdentifier = new();

    [TestInitialize]
    public void SetupTest()
    {
        _serviceProvider.Setup(sp => sp.GetService(typeof(IFileSystem))).Returns(_fileSystem);
        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegBinaryServiceFactory))).Returns(_binaryServiceFactory.Object);
        _megFileService = new MegFileService(_serviceProvider.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(FileNotFoundException))]
    public void Test_Load__ThrowFileNotFound()
    {
        _megFileService.Load("test.meg");
    }

    [TestMethod]
    [ExpectedException(typeof(FileNotFoundException))]
    public void Test_Load__ThrowFileNotFound2()
    {
        _megFileService.Load("test.meg", ReadOnlySpan<byte>.Empty, ReadOnlySpan<byte>.Empty);
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void Test_Load__ThrowNotSupported_Encrypted()
    {
        var encrypted = true;

        var versionIdentifier = new Mock<IMegVersionIdentifier>();
        versionIdentifier.Setup(v => v.GetMegFileVersion(It.IsAny<Stream>(), out encrypted));

        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegVersionIdentifier))).Returns(versionIdentifier.Object);

        _fileSystem.AddEmptyFile("test.meg");

        _megFileService.Load("test.meg");
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void Test_Load__ThrowNotSupported_NotEncrypted()
    {
        var encrypted = false;

        var versionIdentifier = new Mock<IMegVersionIdentifier>();
        versionIdentifier.Setup(v => v.GetMegFileVersion(It.IsAny<Stream>(), out encrypted));

        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegVersionIdentifier))).Returns(versionIdentifier.Object);

        _fileSystem.AddEmptyFile("test.meg");

        _megFileService.Load("test.meg", ReadOnlySpan<byte>.Empty, ReadOnlySpan<byte>.Empty);
    }

    [TestMethod]
    [ExpectedException(typeof(BinaryCorruptedException))]
    public void Test_Load__SizeValidationFails()
    {
        var encrypted = false;
        var version = MegFileVersion.V2;

        _versionIdentifier.Setup(v => v.GetMegFileVersion(It.IsAny<Stream>(), out encrypted)).Returns(version);

        _binaryServiceFactory.Setup(f => f.GetReader(version)).Returns(_megBinaryReader.Object);
        _binaryServiceFactory.Setup(f => f.GetSizeValidator(version, encrypted)).Returns(_sizeValidator.Object);

        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegVersionIdentifier))).Returns(_versionIdentifier.Object);

        _fileSystem.AddEmptyFile("test.meg");

        _megFileService.Load("test.meg");
    }

    [TestMethod]
    public void Test_Load__BinaryReaderThrows()
    {
        var fileData = "some random data";
        bool encrypted = false;
        var version = MegFileVersion.V2;

        _versionIdentifier.Setup(v => v.GetMegFileVersion(It.IsAny<Stream>(), out encrypted)).Returns(version);

        _sizeValidator.Setup(v => v.Validate(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<IMegFileMetadata>()))
            .Returns(true);

        _binaryServiceFactory.Setup(f => f.GetReader(version)).Returns(_megBinaryReader.Object);
        _binaryServiceFactory.Setup(f => f.GetSizeValidator(version, encrypted)).Returns(_sizeValidator.Object);

        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegVersionIdentifier))).Returns(_versionIdentifier.Object);

        _megBinaryReader.Setup(r => r.ReadBinary(It.IsAny<Stream>())).Throws<InvalidOperationException>();

        _fileSystem.AddFile("test.meg", new MockFileData(fileData));

        var e1 = Assert.ThrowsException<BinaryCorruptedException>(() => _megFileService.Load("test.meg"));
        Assert.IsInstanceOfType<InvalidOperationException>(e1.InnerException);


        _megBinaryReader.Setup(r => r.ReadBinary(It.IsAny<Stream>())).Throws<BinaryCorruptedException>();

        var e2 = Assert.ThrowsException<BinaryCorruptedException>(() => _megFileService.Load("test.meg"));
        Assert.IsNull(e2.InnerException);
    }

    [TestMethod]
    public void Test_Load__InvalidSize()
    {
        var fileData = "some random data";
        bool encrypted = false;
        var version = MegFileVersion.V2;

        var metadata = new Mock<IMegFileMetadata>();

        _versionIdentifier.Setup(v => v.GetMegFileVersion(It.IsAny<Stream>(), out encrypted)).Returns(version);

        _sizeValidator.Setup(v => v.Validate(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<IMegFileMetadata>()))
            .Returns(false);

        _binaryServiceFactory.Setup(f => f.GetReader(version)).Returns(_megBinaryReader.Object);
        _binaryServiceFactory.Setup(f => f.GetSizeValidator(version, encrypted)).Returns(_sizeValidator.Object);

        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegVersionIdentifier))).Returns(_versionIdentifier.Object);

        _megBinaryReader.Setup(r => r.ReadBinary(It.IsAny<Stream>())).Returns(metadata.Object);

        _fileSystem.AddFile("test.meg", new MockFileData(fileData));
        
        Assert.ThrowsException<BinaryCorruptedException>(() => _megFileService.Load("test.meg"));
    }

    [TestMethod]
    public void Test_Load__EmptyMeg()
    {
        var fileData = "some random data";
        bool encrypted = false;
        var version = MegFileVersion.V2;

        var header = new Mock<IMegHeader>();
        var metadata = new Mock<IMegFileMetadata>();
        metadata.SetupGet(m => m.Header).Returns(header.Object);

        _versionIdentifier.Setup(v => v.GetMegFileVersion(It.IsAny<Stream>(), out encrypted))
            .Callback((Stream s, out bool e) => 
            { 
                e = false;
                Assert.AreEqual(0, s.Position);
            }).Returns(version);

        _sizeValidator.Setup(v => v.Validate(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<IMegFileMetadata>()))
            .Returns(true);

        _binaryServiceFactory.Setup(f => f.GetReader(version)).Returns(_megBinaryReader.Object);
        _binaryServiceFactory.Setup(f => f.GetSizeValidator(version, encrypted)).Returns(_sizeValidator.Object);

        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegVersionIdentifier))).Returns(_versionIdentifier.Object);

        _megBinaryReader.Setup(r => r.ReadBinary(It.IsAny<Stream>())).Callback((Stream s) =>
        {
            Assert.AreEqual(0, s.Position);
        }).Returns(metadata.Object);

        _fileSystem.AddFile("test.meg", new MockFileData(fileData));

        var megFile = _megFileService.Load("test.meg");

        _versionIdentifier.Verify(r => r.GetMegFileVersion(It.IsAny<Stream>(), out encrypted), Times.Once);
        _megBinaryReader.Verify(r => r.ReadBinary(It.IsAny<Stream>()), Times.Once);
        _sizeValidator.Verify(r => r.Validate(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<IMegFileMetadata>()), Times.Once);

        Assert.AreEqual(megFile.FileVersion, version);
        Assert.IsFalse(megFile.HasEncryption);
        Assert.AreEqual("test", megFile.FileName);
        Assert.AreEqual("test.meg", megFile.FilePath);
        Assert.AreEqual("", megFile.Directory);

        Assert.AreEqual(0, megFile.Content.Count);
    }
}