using System;
using System.IO;
using Moq;
using PG.Commons.Binary;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Files;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

// Note: It's not possible to use Span<T> with Moq. Thus, we cannot test encrypted meg files using it.
// This needs to be tested by integration tests.     
public partial class MegFileServiceTest
{ 
    [Fact]
    public void Test_Load_ThrowFileNotFound()
    {
        Assert.Throws<FileNotFoundException>(() => _megFileService.Load("test.meg"));
    }

    [Fact]
    public void Test_Load_SizeValidationFails()
    {
        var encrypted = false;
        var version = MegFileVersion.V2;

        _versionIdentifier.Setup(v => v.GetMegFileVersion(It.IsAny<Stream>(), out encrypted)).Returns(version);

        _binaryServiceFactory.Setup(f => f.GetReader(version)).Returns(_megBinaryReader.Object);

        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegVersionIdentifier))).Returns(_versionIdentifier.Object);

        _binaryValidator.Setup(v => v.Validate(It.IsAny<IMegBinaryValidationInformation>()))
            .Returns(false);

        _fileSystem.Initialize().WithFile("test.meg");

        Assert.Throws<BinaryCorruptedException>(() => _megFileService.Load("test.meg"));
    }

    [Fact]
    public void Test_Load_BinaryReaderThrows()
    {
        const string fileData = "some random data";
        var encrypted = false;
        const MegFileVersion version = MegFileVersion.V2;

        _versionIdentifier.Setup(v => v.GetMegFileVersion(It.IsAny<Stream>(), out encrypted)).Returns(version);

        _binaryValidator.Setup(v => v.Validate(It.IsAny<IMegBinaryValidationInformation>()))
            .Returns(true);

        _binaryServiceFactory.Setup(f => f.GetReader(version)).Returns(_megBinaryReader.Object);

        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegVersionIdentifier))).Returns(_versionIdentifier.Object);

        _megBinaryReader.Setup(r => r.ReadBinary(It.IsAny<Stream>())).Throws<InvalidOperationException>();

        _fileSystem.Initialize().WithFile("test.meg").Which(m => m.HasStringContent(fileData));

        Assert.Throws<InvalidOperationException>(() => _megFileService.Load("test.meg"));

        _megBinaryReader.Setup(r => r.ReadBinary(It.IsAny<Stream>())).Throws<BinaryCorruptedException>();

        var e2 = Assert.Throws<BinaryCorruptedException>(() => _megFileService.Load("test.meg"));
        Assert.Null(e2.InnerException);
    }

    [Fact]
    public void Test_Load_InvalidBinary()
    {
        const string fileData = "some random data";
        var encrypted = false;
        const MegFileVersion version = MegFileVersion.V2;

        var metadata = new Mock<IMegFileMetadata>();

        _versionIdentifier.Setup(v => v.GetMegFileVersion(It.IsAny<Stream>(), out encrypted)).Returns(version);

        _binaryValidator.Setup(v => v.Validate(It.IsAny<IMegBinaryValidationInformation>()))
            .Returns(false);

        _binaryServiceFactory.Setup(f => f.GetReader(version)).Returns(_megBinaryReader.Object);

        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegVersionIdentifier))).Returns(_versionIdentifier.Object);

        _megBinaryReader.Setup(r => r.ReadBinary(It.IsAny<Stream>())).Returns(metadata.Object);

        _fileSystem.Initialize().WithFile("test.meg").Which(m => m.HasStringContent(fileData));
        
        Assert.Throws<BinaryCorruptedException>(() => _megFileService.Load("test.meg"));

        _binaryValidator.Verify(r => r.Validate(It.IsAny<IMegBinaryValidationInformation>()), Times.Once);
    }

    [Fact]
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
                Assert.Equal(0, s.Position);
            }).Returns(version);

        _binaryValidator.Setup(v => v.Validate(It.IsAny<IMegBinaryValidationInformation>()))
            .Returns(true);

        _binaryServiceFactory.Setup(f => f.GetReader(version)).Returns(_megBinaryReader.Object);
        _binaryServiceFactory.Setup(f => f.GetConverter(version)).Returns(_megBinaryConverter.Object);

        _serviceProvider.Setup(sp => sp.GetService(typeof(IMegVersionIdentifier))).Returns(_versionIdentifier.Object);

        _megBinaryReader.Setup(r => r.ReadBinary(It.IsAny<Stream>())).Callback((Stream s) =>
        {
            Assert.Equal(0, s.Position);
        }).Returns(metadata.Object);


        var megArchive = new Mock<IMegArchive>();
        _megBinaryConverter.Setup(c => c.BinaryToModel(metadata.Object))
            .Returns(megArchive.Object);

        _fileSystem.Initialize().WithFile(megFileName).Which(m => m.HasStringContent(fileData));

        var megFile = _megFileService.Load(megFileName);

        Assert.Same(megArchive.Object, megFile.Content);

        Assert.Equal(version, megFile.FileInformation.FileVersion);
        Assert.False(megFile.FileInformation.HasEncryption);
        Assert.Equal(megFileName, megFile.FileName);
        Assert.Equal(_fileSystem.Path.GetFullPath(megFileName), megFile.FilePath);
        Assert.Equal(_fileSystem.Path.GetDirectoryName(_fileSystem.Path.GetFullPath(megFileName)), megFile.Directory);

        _versionIdentifier.Verify(r => r.GetMegFileVersion(It.IsAny<Stream>(), out encrypted), Times.Once);
        _megBinaryReader.Verify(r => r.ReadBinary(It.IsAny<Stream>()), Times.Once);
        _binaryValidator.Verify(r => r.Validate(It.IsAny<IMegBinaryValidationInformation>()), Times.Once);
        _megBinaryConverter.Verify(r => r.BinaryToModel(It.IsAny<IMegFileMetadata>()), Times.Once);
    }
}