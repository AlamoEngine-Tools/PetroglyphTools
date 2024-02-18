using System;
using System.Collections.Generic;
using System.IO;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Binary;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Files;
using Testably.Abstractions.Testing;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

// Note: It's not possible to use Span<T> with Moq. Thus, we cannot test encrypted meg files using it.
// This needs to be tested by integration tests.     
public partial class MegFileServiceTest
{ 
    [TestMethod]
    [ExpectedException(typeof(FileNotFoundException))]
    public void Test_Load__ThrowFileNotFound()
    {
        _megFileService.Load("test.meg");
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

        _binaryValidator.Setup(v => v.Validate(It.IsAny<IMegBinaryValidationInformation>()))
            .Returns(new ValidationResult(new List<ValidationFailure> { new("Fail", "Fail") }));

        _fileSystem.Initialize().WithFile("test.meg");

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

        _fileSystem.Initialize().WithFile("test.meg").Which(m => m.HasStringContent(fileData));

        Assert.ThrowsException<InvalidOperationException>(() => _megFileService.Load("test.meg"));

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

        _fileSystem.Initialize().WithFile("test.meg").Which(m => m.HasStringContent(fileData));
        
        Assert.ThrowsException<BinaryCorruptedException>(() => _megFileService.Load("test.meg"));

        _binaryValidator.Verify(r => r.Validate(It.IsAny<IMegBinaryValidationInformation>()), Times.Once);
    }

    [TestMethod]
    public void Test_Load()
    {
        const string megFileName = "test.meg";
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

        _fileSystem.Initialize().WithFile(megFileName).Which(m => m.HasStringContent(fileData));

        var megFile = _megFileService.Load(megFileName);

        Assert.AreSame(megArchive.Object, megFile.Content);

        Assert.AreEqual(megFile.FileInformation.FileVersion, version);
        Assert.IsFalse(megFile.FileInformation.HasEncryption);
        Assert.AreEqual(megFileName, megFile.FileName);
        Assert.AreEqual(_fileSystem.Path.GetFullPath(megFileName), megFile.FilePath);
        Assert.AreEqual(_fileSystem.Path.GetDirectoryName(_fileSystem.Path.GetFullPath(megFileName)), megFile.Directory);

        _versionIdentifier.Verify(r => r.GetMegFileVersion(It.IsAny<Stream>(), out encrypted), Times.Once);
        _megBinaryReader.Verify(r => r.ReadBinary(It.IsAny<Stream>()), Times.Once);
        _binaryValidator.Verify(r => r.Validate(It.IsAny<IMegBinaryValidationInformation>()), Times.Once);
        _megBinaryConverter.Verify(r => r.BinaryToModel(It.IsAny<IMegFileMetadata>()), Times.Once);
    }
}