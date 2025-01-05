using System;
using System.Security.Cryptography;
using PG.StarWarsGame.Files.MEG.Files;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Files;

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

    [Fact]
    public void Test_KeyHandling()
    {
        var keyIv = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(keyIv);

        var encData = new MegEncryptionData(keyIv, keyIv);

        // Alter the array reference;
        keyIv[0] = 0;
        keyIv[1] = 0;

        Assert.NotEqual(keyIv, encData.IV);
        Assert.NotEqual(keyIv, encData.Key);

        var iv = encData.IV;
        iv[0] = 0;
        iv[1] = 0;

        var key = encData.Key;
        key[0] = 0;
        key[1] = 0;

        Assert.NotEqual(iv, encData.IV);
        Assert.NotEqual(key, encData.Key);

        encData.Dispose();

        Assert.True(encData.IsDisposed);

        Assert.Throws<ObjectDisposedException>(() => encData.Key);
        Assert.Throws<ObjectDisposedException>(() => encData.IV);
    }

    [Fact]
    public void Test_Ctor_InvalidKeySizes()
    {
        var invalidSize = new byte[4];
        var validSize = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(invalidSize);
        RandomNumberGenerator.Create().GetNonZeroBytes(validSize);

        Assert.Throws<ArgumentException>(() => new MegEncryptionData(invalidSize, validSize));
        Assert.Throws<ArgumentException>(() => new MegEncryptionData(validSize, invalidSize));
    }
}