using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Binary;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using PG.StarWarsGame.Files.MEG.Data;
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
    private readonly Mock<IMegBinaryConverter> _megBinaryConverter = new();
    private readonly Mock<IMegBinaryValidator> _binaryValidator = new();
    private readonly Mock<IMegVersionIdentifier> _versionIdentifier = new();

    [TestInitialize]
    public void SetupTest()
    {
        _serviceProvider.Setup(sp => sp.GetService(typeof(IFileSystem))).Returns(_fileSystem);
        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegBinaryServiceFactory))).Returns(_binaryServiceFactory.Object);
        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegBinaryValidator))).Returns(_binaryValidator.Object);
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

        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegVersionIdentifier))).Returns(_versionIdentifier.Object);

        _fileSystem.AddEmptyFile("test.meg");

        _megFileService.Load("test.meg");
    }

    [TestMethod]
    public void Test_Load__BinaryReaderThrows()
    {
        const string fileData = "some random data";
        var encrypted = false;
        const MegFileVersion version = MegFileVersion.V2;

        _versionIdentifier.Setup(v => v.GetMegFileVersion(It.IsAny<Stream>(), out encrypted)).Returns(version);

        _binaryValidator.Setup(v => v.Validate(It.IsAny<IMegBinaryValidationInformation>()))
            .Returns(new ValidationResult());

        _binaryServiceFactory.Setup(f => f.GetReader(version)).Returns(_megBinaryReader.Object);

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
    public void Test_Load__InvalidBinary()
    {
        const string fileData = "some random data";
        var encrypted = false;
        const MegFileVersion version = MegFileVersion.V2;

        var metadata = new Mock<IMegFileMetadata>();

        _versionIdentifier.Setup(v => v.GetMegFileVersion(It.IsAny<Stream>(), out encrypted)).Returns(version);

        _binaryValidator.Setup(v => v.Validate(It.IsAny<IMegBinaryValidationInformation>()))
            .Returns(new ValidationResult(new List<ValidationFailure>{ new("name", "error") }));

        _binaryServiceFactory.Setup(f => f.GetReader(version)).Returns(_megBinaryReader.Object);

        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegVersionIdentifier))).Returns(_versionIdentifier.Object);

        _megBinaryReader.Setup(r => r.ReadBinary(It.IsAny<Stream>())).Returns(metadata.Object);

        _fileSystem.AddFile("test.meg", new MockFileData(fileData));
        
        Assert.ThrowsException<BinaryCorruptedException>(() => _megFileService.Load("test.meg"));

        _binaryValidator.Verify(r => r.Validate(It.IsAny<IMegBinaryValidationInformation>()), Times.Once);
    }

    [TestMethod]
    public void Test_Load()
    {
        const string fileData = "some random data";
        const MegFileVersion version = MegFileVersion.V2;

        var encrypted = false;

        var header = new Mock<IMegHeader>();
        var metadata = new Mock<IMegFileMetadata>();
        metadata.SetupGet(m => m.Header).Returns(header.Object);

        _versionIdentifier.Setup(v => v.GetMegFileVersion(It.IsAny<Stream>(), out encrypted))
            .Callback((Stream s, out bool e) => 
            { 
                e = false;
                Assert.AreEqual(0, s.Position);
            }).Returns(version);

        _binaryValidator.Setup(v => v.Validate(It.IsAny<IMegBinaryValidationInformation>()))
            .Returns(new ValidationResult());

        _binaryServiceFactory.Setup(f => f.GetReader(version)).Returns(_megBinaryReader.Object);
        _binaryServiceFactory.Setup(f => f.GetConverter(version)).Returns(_megBinaryConverter.Object);

        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegVersionIdentifier))).Returns(_versionIdentifier.Object);

        _megBinaryReader.Setup(r => r.ReadBinary(It.IsAny<Stream>())).Callback((Stream s) =>
        {
            Assert.AreEqual(0, s.Position);
        }).Returns(metadata.Object);


        var megArchive = new Mock<IMegArchive>();
        _megBinaryConverter.Setup(c => c.BinaryToModel(metadata.Object))
            .Returns(megArchive.Object);

        _fileSystem.AddFile("test.meg", new MockFileData(fileData));

        var megFile = _megFileService.Load("test.meg");

        Assert.AreSame(megArchive.Object, megFile.Content);

        Assert.AreEqual(megFile.FileVersion, version);
        Assert.IsFalse(megFile.HasEncryption);
        Assert.AreEqual("test", megFile.FileName);
        Assert.AreEqual("test.meg", megFile.FilePath);
        Assert.AreEqual("", megFile.Directory);

        _versionIdentifier.Verify(r => r.GetMegFileVersion(It.IsAny<Stream>(), out encrypted), Times.Once);
        _megBinaryReader.Verify(r => r.ReadBinary(It.IsAny<Stream>()), Times.Once);
        _binaryValidator.Verify(r => r.Validate(It.IsAny<IMegBinaryValidationInformation>()), Times.Once);
        _megBinaryConverter.Verify(r => r.BinaryToModel(It.IsAny<IMegFileMetadata>()), Times.Once);
    }

    [TestMethod]
    public void Test_GetMegFileVersion_Throws()
    {
        _serviceProvider.Setup(s => s.GetService(typeof(IMegVersionIdentifier))).Returns(_versionIdentifier.Object);

       Assert.ThrowsException<ArgumentNullException>(() => _megFileService.GetMegFileVersion((string)null!, out _));
       Assert.ThrowsException<ArgumentNullException>(() => _megFileService.GetMegFileVersion("", out _));
       Assert.ThrowsException<ArgumentNullException>(() => _megFileService.GetMegFileVersion("   ", out _));
       Assert.ThrowsException<ArgumentNullException>(() => _megFileService.GetMegFileVersion((Stream)null!, out _));
    }

    [TestMethod]
    public void Test_GetMegFileVersion_FromStream()
    {
        _serviceProvider.Setup(s => s.GetService(typeof(IMegVersionIdentifier))).Returns(_versionIdentifier.Object);

        var data = new MemoryStream();
        var encrypted = true;
        _versionIdentifier.Setup(v => v.GetMegFileVersion(data, out encrypted)).Returns(MegFileVersion.V3);

        var megVersion = _megFileService.GetMegFileVersion(data, out var isEncrypted);
        Assert.IsTrue(isEncrypted);
        Assert.AreEqual(MegFileVersion.V3, megVersion);
        
        // Check stream does not get disposed
        Assert.IsTrue(data.CanRead);
    }

    [TestMethod]
    public void Test_GetMegFileVersion_FromFile()
    {
        _fileSystem.AddFile("test.meg", new MockFileData("Some Data..."));
        _serviceProvider.Setup(s => s.GetService(typeof(IMegVersionIdentifier))).Returns(_versionIdentifier.Object);
        var encrypted = true;
        _versionIdentifier.Setup(v => v.GetMegFileVersion(It.IsAny<Stream>(), out encrypted)).Returns(MegFileVersion.V3);
        var megVersion = _megFileService.GetMegFileVersion("test.meg", out var isEncrypted);
        Assert.IsTrue(isEncrypted);
        Assert.AreEqual(MegFileVersion.V3, megVersion);
    }

    [TestMethod]
    public void Test_GetMegFileVersion_Throws_FileNotFound()
    {
        _serviceProvider.Setup(s => s.GetService(typeof(IMegVersionIdentifier))).Returns(_versionIdentifier.Object);
        Assert.ThrowsException<FileNotFoundException>(() => _megFileService.GetMegFileVersion("test.meg", out _));
    }
}