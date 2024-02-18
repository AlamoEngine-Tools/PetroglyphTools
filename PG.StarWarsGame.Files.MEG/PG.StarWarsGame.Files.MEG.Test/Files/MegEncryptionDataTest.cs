using System;
using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Test.Files;

[TestClass]
public class MegEncryptionDataTest
{
    internal static MegEncryptionData CreateRandomData()
    {
        var key = new byte[16];
        var iv = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(key);
        RandomNumberGenerator.Create().GetNonZeroBytes(iv);
        return new MegEncryptionData(key, iv);
    }

    [TestMethod]
    public void Test_KeyHandling()
    {
        var keyIv = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(keyIv);

        var encData = new MegEncryptionData(keyIv, keyIv);

        // Alter the array reference;
        keyIv[0] = 0;
        keyIv[1] = 0;

        CollectionAssert.AreNotEqual(keyIv, encData.IV);
        CollectionAssert.AreNotEqual(keyIv, encData.Key);

        var iv = encData.IV;
        iv[0] = 0;
        iv[1] = 0;

        var key = encData.Key;
        key[0] = 0;
        key[1] = 0;

        CollectionAssert.AreNotEqual(iv, encData.IV);
        CollectionAssert.AreNotEqual(key, encData.Key);

        encData.Dispose();

        Assert.IsTrue(encData.IsDisposed);

        Assert.ThrowsException<ObjectDisposedException>(() => encData.Key);
        Assert.ThrowsException<ObjectDisposedException>(() => encData.IV);
    }

    [TestMethod]
    public void Test_Ctor_InvalidKeySizes()
    {
        var invalidSize = new byte[4];
        var validSize = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(invalidSize);
        RandomNumberGenerator.Create().GetNonZeroBytes(validSize);

        Assert.ThrowsException<ArgumentException>(() => new MegEncryptionData(invalidSize, validSize));
        Assert.ThrowsException<ArgumentException>(() => new MegEncryptionData(validSize, invalidSize));
    }
}