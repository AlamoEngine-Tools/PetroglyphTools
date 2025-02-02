using System;
using System.Security.Cryptography;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Files;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Files;

public class MegFileTest : CommonMegTestBase
{
    [Fact]
    public void Ctor_ThrowsArgumentNullException()
    {
        FileSystem.Initialize().WithFile("test.meg");
        var param = new MegFileInformation("test.meg", MegFileVersion.V1);
        var model = new MegArchive([]);

        Assert.Throws<ArgumentNullException>(() => new MegFile(null!, param, ServiceProvider));
        Assert.Throws<ArgumentNullException>(() => new MegFile(model, null!, ServiceProvider));
        Assert.Throws<ArgumentNullException>(() => new MegFile(model, param, null!));
    }

    [Fact]
    public void Ctor_SetupProperties()
    {
        const string name = "test.meg";
        var param = new MegFileInformation(name, MegFileVersion.V2);
        var model = new MegArchive([]);

        FileSystem.Initialize().WithFile("test.meg");

        var megFile = new MegFile(model, param, ServiceProvider);

        Assert.Same(model, megFile.Content);
        Assert.Same(model, megFile.Archive);
        Assert.Equal(MegFileVersion.V2, megFile.FileInformation.FileVersion);
        Assert.False(megFile.FileInformation.HasEncryption);

        Assert.Equal(FileSystem.Path.GetFullPath(name), megFile.FileInformation.FilePath);
    }

    [Fact]
    public void Ctor_SetupProperties_DisposeFileInfoParam()
    {
        const string name = "test.meg";

        var encData = MegEncryptionDataTest.CreateRandomData();

        var copyKey = encData.Key;
        
        var param = new MegFileInformation(name, MegFileVersion.V3, encData);

        FileSystem.Initialize().WithFile("test.meg");

        var megFile = new MegFile(new MegArchive([]), param, ServiceProvider);

        param.Dispose();

        Assert.True(param.EncryptionData!.IsDisposed);

        Assert.NotNull(megFile.FileInformation.EncryptionData);
        Assert.Equal(megFile.FileInformation.EncryptionData.Key, copyKey);
    }

    [Fact]
    public void Ctor_SetupProperties_Encrypted()
    {
        var key = new byte[16];
        var iv = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(key);
        RandomNumberGenerator.Create().GetNonZeroBytes(iv);

        var encData = new MegEncryptionData(key, iv);

        var param = new MegFileInformation("test.meg", MegFileVersion.V3, encData);

        FileSystem.Initialize().WithFile("test.meg");

        var model = new MegArchive([]);
        var megFile = new MegFile(model, param, ServiceProvider);

        Assert.Same(model, megFile.Content);
        Assert.Equal(MegFileVersion.V3, megFile.FileInformation.FileVersion);
        Assert.True(megFile.FileInformation.HasEncryption);
        Assert.Equal(iv, megFile.FileInformation.EncryptionData!.IV);
        Assert.Equal(key, megFile.FileInformation.EncryptionData!.Key);
    }

    [Fact]
    public void EncryptionKeyHandling()
    {
        var keyIv = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(keyIv);

        var encData = new MegEncryptionData(keyIv, keyIv);
        
        var param = new MegFileInformation("test.meg", MegFileVersion.V3, encData);
        var model = new MegArchive([]);

        FileSystem.Initialize().WithFile("test.meg");

        var megFile = new MegFile(model, param, ServiceProvider);

        // Ensure that we can safely dispose initialization data;
        param.Dispose();
        encData.Dispose();

        Assert.Equal(keyIv, megFile.FileInformation.EncryptionData!.Key);
        Assert.Equal(keyIv, megFile.FileInformation.EncryptionData.IV);

        // Ensure that FileInformation gives us a copy that's safe to dispose.
        var fileParams = megFile.FileInformation;
        fileParams.Dispose();

        Assert.Equal(keyIv, megFile.FileInformation.EncryptionData!.Key);
        Assert.Equal(keyIv, megFile.FileInformation.EncryptionData.IV);

        // Alter the array reference;
        keyIv[0] = 0;
        keyIv[1] = 0;

        Assert.NotEqual(keyIv, megFile.FileInformation.EncryptionData!.IV);
        Assert.NotEqual(keyIv, megFile.FileInformation.EncryptionData!.Key);

        var iv = megFile.FileInformation.EncryptionData!.IV;
        iv[0] = 0;
        iv[1] = 0;

        var key = megFile.FileInformation.EncryptionData!.Key;
        key[0] = 0;
        key[1] = 0;

        Assert.NotEqual(iv, megFile.FileInformation.EncryptionData!.IV);
        Assert.NotEqual(key, megFile.FileInformation.EncryptionData!.Key);

        megFile.Dispose();

        Assert.Throws<ObjectDisposedException>(() => megFile.FileInformation.EncryptionData.IV);
        Assert.Throws<ObjectDisposedException>(() => megFile.FileInformation.EncryptionData.Key);
    }
}