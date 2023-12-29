using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Test.Files;

[TestClass]
public class MegFileTest
{
    private readonly Mock<IServiceProvider> _serviceProvider = new();
    private readonly MockFileSystem _fileSystem = new();

    [TestInitialize]
    public void SetupTest()
    {
        _serviceProvider.Setup(sp => sp.GetService(typeof(IFileSystem))).Returns(_fileSystem);
    }

    [TestMethod]
    public void Test_Ctor_ThrowsArgumentNullException()
    {
        var param = new MegFileInformation("test.meg", MegFileVersion.V1);
        var model = new Mock<IMegArchive>();

        Assert.ThrowsException<ArgumentNullException>(() => new MegFile(null!, param, _serviceProvider.Object));
        Assert.ThrowsException<ArgumentNullException>(() => new MegFile(model.Object, null!, _serviceProvider.Object));
        Assert.ThrowsException<ArgumentNullException>(() => new MegFile(model.Object, param, null!));
    }

    [TestMethod]
    public void Test_Ctor_SetupProperties()
    {
        const string name = "test.meg";
        var param = new MegFileInformation(name, MegFileVersion.V2);
        var model = new Mock<IMegArchive>().Object;

        _fileSystem.AddEmptyFile(name);

        var megFile = new MegFile(model, param, _serviceProvider.Object);

        Assert.AreSame(model, megFile.Content);
        Assert.AreSame(model, megFile.Archive);
        Assert.AreEqual(MegFileVersion.V2, megFile.FileInformation.FileVersion);
        Assert.IsFalse(megFile.FileInformation.HasEncryption);

        Assert.AreEqual(_fileSystem.Path.GetFullPath(name), megFile.FileInformation.FilePath);
    }

    [TestMethod]
    public void Test_Ctor_SetupProperties_Encrypted()
    {
        var key = new byte[16];
        var iv = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(key);
        RandomNumberGenerator.Create().GetNonZeroBytes(iv);

        var encData = new MegEncryptionData(key, iv);

        var param = new MegFileInformation("test.meg", MegFileVersion.V3, encData);
        var model = new Mock<IMegArchive>().Object;

        _fileSystem.AddEmptyFile("test.meg");

        var megFile = new MegFile(model, param, _serviceProvider.Object);

        Assert.AreSame(model, megFile.Content);
        Assert.AreEqual(MegFileVersion.V3, megFile.FileInformation.FileVersion);
        Assert.IsTrue(megFile.FileInformation.HasEncryption);
        CollectionAssert.AreEqual(iv, megFile.FileInformation.EncryptionData!.IV);
        CollectionAssert.AreEqual(key, megFile.FileInformation.EncryptionData!.Key);
    }

    [TestMethod]
    public void Test_EncryptionKeyHandling()
    {
        var keyIv = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(keyIv);

        var encData = new MegEncryptionData(keyIv, keyIv);
        
        var param = new MegFileInformation("test.meg", MegFileVersion.V3, encData);
        var model = new Mock<IMegArchive>().Object;

        _fileSystem.AddEmptyFile("test.meg");

        var megFile = new MegFile(model, param, _serviceProvider.Object);

        // Ensure that we can safely dispose initialization data;
        param.Dispose();
        encData.Dispose();

        CollectionAssert.AreEqual(keyIv, megFile.FileInformation.EncryptionData!.Key);
        CollectionAssert.AreEqual(keyIv, megFile.FileInformation.EncryptionData.IV);

        // Ensure that FileInformation gives us a copy that's safe to dispose.
        var fileParams = megFile.FileInformation;
        fileParams.Dispose();

        CollectionAssert.AreEqual(keyIv, megFile.FileInformation.EncryptionData!.Key);
        CollectionAssert.AreEqual(keyIv, megFile.FileInformation.EncryptionData.IV);

        // Alter the array reference;
        keyIv[0] = 0;
        keyIv[1] = 0;

        CollectionAssert.AreNotEqual(keyIv, megFile.FileInformation.EncryptionData!.IV);
        CollectionAssert.AreNotEqual(keyIv, megFile.FileInformation.EncryptionData!.Key);

        var iv = megFile.FileInformation.EncryptionData!.IV;
        iv[0] = 0;
        iv[1] = 0;

        var key = megFile.FileInformation.EncryptionData!.Key;
        key[0] = 0;
        key[1] = 0;

        CollectionAssert.AreNotEqual(iv, megFile.FileInformation.EncryptionData!.IV);
        CollectionAssert.AreNotEqual(key, megFile.FileInformation.EncryptionData!.Key);

        megFile.Dispose();

        Assert.ThrowsException<ObjectDisposedException>(() => megFile.FileInformation.EncryptionData.IV);
        Assert.ThrowsException<ObjectDisposedException>(() => megFile.FileInformation.EncryptionData.Key);
    }
}