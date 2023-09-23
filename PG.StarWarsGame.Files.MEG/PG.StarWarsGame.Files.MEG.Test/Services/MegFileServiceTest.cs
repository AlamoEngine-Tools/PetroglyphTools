using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Binary;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

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
    [ExpectedException(typeof(BinaryCorruptedException))]
    public void Test_Load__()
    {
        var fileData = "some random data";
        bool encrypted = false;
        var version = MegFileVersion.V2;

        var metadata = new Mock<IMegFileMetadata>();

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


        _megFileService.Load("test.meg");


        _versionIdentifier.Verify(r => r.GetMegFileVersion(It.IsAny<Stream>(), out encrypted), Times.Once);
        _megBinaryReader.Verify(r => r.ReadBinary(It.IsAny<Stream>()), Times.Once);
        _sizeValidator.Verify(r => r.Validate(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<IMegFileMetadata>()), Times.Once);
    }
}