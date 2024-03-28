using System;
using System.IO;
using Moq;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Files;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

public partial class MegFileServiceTest
{
    [Fact]
    public void Test_GetMegFileVersion_Throws()
    {
        _serviceProvider.Setup(s => s.GetService(typeof(IMegVersionIdentifier))).Returns(_versionIdentifier.Object);

       Assert.Throws<ArgumentNullException>(() => _megFileService.GetMegFileVersion((string)null!, out _));
       Assert.Throws<ArgumentException>(() => _megFileService.GetMegFileVersion("", out _));
       Assert.Throws<ArgumentException>(() => _megFileService.GetMegFileVersion("   ", out _));
    }

    [Fact]
    public void Test_GetMegFileVersion()
    {
        _fileSystem.Initialize().WithFile("test.meg").Which(manipulator => manipulator.HasStringContent("Some Data..."));
        _serviceProvider.Setup(s => s.GetService(typeof(IMegVersionIdentifier))).Returns(_versionIdentifier.Object);
        var encrypted = true;
        _versionIdentifier.Setup(v => v.GetMegFileVersion(It.IsAny<Stream>(), out encrypted)).Returns(MegFileVersion.V3);
        var megVersion = _megFileService.GetMegFileVersion("test.meg", out var isEncrypted);
        Assert.True(isEncrypted);
        Assert.Equal(MegFileVersion.V3, megVersion);
    }

    [Fact]
    public void Test_GetMegFileVersion_Throws_FileNotFound()
    {
        _serviceProvider.Setup(s => s.GetService(typeof(IMegVersionIdentifier))).Returns(_versionIdentifier.Object);
        Assert.Throws<FileNotFoundException>(() => _megFileService.GetMegFileVersion("test.meg", out _));
    }
}