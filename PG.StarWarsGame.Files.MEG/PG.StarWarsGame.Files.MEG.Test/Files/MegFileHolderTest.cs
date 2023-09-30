using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Test.Files;

[TestClass]
public class MegFileHolderTest
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

        Assert.ThrowsException<ArgumentNullException>(() => new MegFileHolder(null!, param, _serviceProvider.Object));
        Assert.ThrowsException<ArgumentNullException>(() => new MegFileHolder(model.Object, null!, _serviceProvider.Object));
        Assert.ThrowsException<ArgumentNullException>(() => new MegFileHolder(model.Object, param, null!));

        Assert.ThrowsException<ArgumentNullException>(() =>
            new MegFileHolder(null!, param, ReadOnlySpan<byte>.Empty, ReadOnlySpan<byte>.Empty, _serviceProvider.Object));

        Assert.ThrowsException<ArgumentNullException>(() =>
            new MegFileHolder(model.Object, null!, ReadOnlySpan<byte>.Empty, ReadOnlySpan<byte>.Empty, _serviceProvider.Object));

        Assert.ThrowsException<ArgumentNullException>(() =>
            new MegFileHolder(model.Object, param, ReadOnlySpan<byte>.Empty, ReadOnlySpan<byte>.Empty, null!));
    }

    [TestMethod]
    public void Test_Ctor_SetupProperties()
    {
        var param = new MegFileHolderParam { FilePath = "test.meg", FileVersion = MegFileVersion.V2};
        var model = new Mock<IMegArchive>().Object;

        var holder = new MegFileHolder(model, param, _serviceProvider.Object);

        Assert.AreSame(model, holder.Content);
        Assert.AreEqual(MegFileVersion.V2, holder.FileVersion);
        Assert.IsFalse(holder.HasEncryption);
        Assert.IsNull(holder.IV);
        Assert.IsNull(holder.Key);
    }

    [TestMethod]
    public void Test_Ctor_SetupProperties_Encrypted()
    {
        var param = new MegFileHolderParam { FilePath = "test.meg", FileVersion = MegFileVersion.V3 };
        var model = new Mock<IMegArchive>().Object;

        var key = new byte[16];
        var iv = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(key);
        RandomNumberGenerator.Create().GetNonZeroBytes(iv);

        var holder = new MegFileHolder(model, param, key, iv, _serviceProvider.Object);

        Assert.AreSame(model, holder.Content);
        Assert.AreEqual(MegFileVersion.V3, holder.FileVersion);
        Assert.IsTrue(holder.HasEncryption);
        CollectionAssert.AreEqual(iv, holder.IV);
        CollectionAssert.AreEqual(key, holder.Key);
    }

    [TestMethod]
    public void Test_EncryptionKeyHandling()
    {
        var param = new MegFileHolderParam { FilePath = "test.meg", FileVersion = MegFileVersion.V3 };
        var model = new Mock<IMegArchive>().Object;

        var keyIv = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(keyIv);

        var holder = new MegFileHolder(model, param, keyIv, keyIv, _serviceProvider.Object);

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
        var param = new MegFileHolderParam { FilePath = "test.meg", FileVersion = MegFileVersion.V3 };
        var model = new Mock<IMegArchive>().Object;

        var keyIv = new byte[4];
        RandomNumberGenerator.Create().GetNonZeroBytes(keyIv);

        Assert.ThrowsException<ArgumentException>(() => new MegFileHolder(model, param, keyIv, keyIv, _serviceProvider.Object));

        keyIv = new byte[18];
        RandomNumberGenerator.Create().GetNonZeroBytes(keyIv);
        Assert.ThrowsException<ArgumentException>(() => new MegFileHolder(model, param, keyIv, keyIv, _serviceProvider.Object));


        param = new MegFileHolderParam { FilePath = "test.meg", FileVersion = MegFileVersion.V2 };
        keyIv = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(keyIv);
        Assert.ThrowsException<ArgumentException>(() => new MegFileHolder(model, param, keyIv, keyIv, _serviceProvider.Object));
    }
}