using System;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

public partial class MegFileServiceTest
{
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