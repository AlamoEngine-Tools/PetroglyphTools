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
        var param = new MegFileHolderParam { FilePath = "test.meg" };
        var model = new Mock<IMegArchive>();

        Assert.ThrowsException<ArgumentNullException>(() => new MegFile(null!, param, _serviceProvider.Object));
        Assert.ThrowsException<ArgumentNullException>(() => new MegFile(model.Object, null!, _serviceProvider.Object));
        Assert.ThrowsException<ArgumentNullException>(() => new MegFile(model.Object, param, null!));
    }

    [TestMethod]
    public void Test_Ctor_SetupProperties()
    {
        var param = new MegFileHolderParam { FilePath = "test.meg", FileVersion = MegFileVersion.V2};
        var model = new Mock<IMegArchive>().Object;

        var holder = new MegFile(model, param, _serviceProvider.Object);

        Assert.AreSame(model, holder.Content);
        Assert.AreSame(model, holder.Archive);
        Assert.AreEqual(MegFileVersion.V2, holder.FileVersion);
        Assert.IsFalse(holder.HasEncryption);
        Assert.IsNull(holder.IV);
        Assert.IsNull(holder.Key);
    }

    [TestMethod]
    public void Test_Ctor_SetupProperties_Encrypted()
    {
        var key = new byte[16];
        var iv = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(key);
        RandomNumberGenerator.Create().GetNonZeroBytes(iv);

        var param = new MegFileHolderParam { FilePath = "test.meg", FileVersion = MegFileVersion.V3, Key = key, IV = iv};
        var model = new Mock<IMegArchive>().Object;

        var holder = new MegFile(model, param, _serviceProvider.Object);

        Assert.AreSame(model, holder.Content);
        Assert.AreEqual(MegFileVersion.V3, holder.FileVersion);
        Assert.IsTrue(holder.HasEncryption);
        CollectionAssert.AreEqual(iv, holder.IV);
        CollectionAssert.AreEqual(key, holder.Key);
    }

    [TestMethod]
    public void Test_EncryptionKeyHandling()
    {
        var keyIv = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(keyIv);

        var param = new MegFileHolderParam { FilePath = "test.meg", FileVersion = MegFileVersion.V3, IV = keyIv, Key = keyIv};
        var model = new Mock<IMegArchive>().Object;

        var holder = new MegFile(model, param, _serviceProvider.Object);

        // Alter the array reference;
        keyIv[0] = 0;
        keyIv[1] = 0;

        CollectionAssert.AreNotEqual(keyIv, holder.IV);
        CollectionAssert.AreNotEqual(keyIv, holder.Key);

        var iv = holder.IV;
        iv[0] = 0;
        iv[1] = 0;

        var key = holder.Key;
        key[0] = 0;
        key[1] = 0;

        CollectionAssert.AreNotEqual(iv, holder.IV);
        CollectionAssert.AreNotEqual(key, holder.Key);

        holder.Dispose();

        Assert.IsNull(holder.Key);
        Assert.IsNull(holder.IV);
    }

    [TestMethod]
    public void Test_Ctor_Ctor_InvalidKeySizes()
    {
        var baseParam = new MegFileHolderParam { FilePath = "test.meg", FileVersion = MegFileVersion.V3 };
        var model = new Mock<IMegArchive>().Object;

        var invalidSize = new byte[4];
        var validSize = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(invalidSize);
        RandomNumberGenerator.Create().GetNonZeroBytes(validSize);


        var param1 = baseParam with { Key = invalidSize, IV = validSize };
        var param2 = baseParam with { Key = validSize, IV = invalidSize };
        var param3 = baseParam with { Key = validSize, IV = validSize, FileVersion = MegFileVersion.V2 };

        Assert.ThrowsException<ArgumentException>(() => new MegFile(model, param1, _serviceProvider.Object));
        Assert.ThrowsException<ArgumentException>(() => new MegFile(model, param2, _serviceProvider.Object));
        Assert.ThrowsException<ArgumentException>(() => new MegFile(model, param3, _serviceProvider.Object));
    }
}

[TestClass]
public class MegFileHolderParamTest
{
    [TestMethod]
    public void Test_KeyHandling()
    {
        var keyIv = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(keyIv);

        var param = new MegFileHolderParam { FilePath = "test.meg", FileVersion = MegFileVersion.V3, IV = keyIv, Key = keyIv };
       
        // Alter the array reference;
        keyIv[0] = 0;
        keyIv[1] = 0;

        CollectionAssert.AreNotEqual(keyIv, param.IV);
        CollectionAssert.AreNotEqual(keyIv, param.Key);

        var iv = param.IV;
        iv[0] = 0;
        iv[1] = 0;

        var key = param.Key;
        key[0] = 0;
        key[1] = 0;

        CollectionAssert.AreNotEqual(iv, param.IV);
        CollectionAssert.AreNotEqual(key, param.Key);

        param.Dispose();

        Assert.IsNull(param.Key);
        Assert.IsNull(param.IV);
    }
}