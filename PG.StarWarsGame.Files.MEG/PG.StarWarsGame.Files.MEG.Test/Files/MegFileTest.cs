using System;
using System.IO.Abstractions;
using System.Security.Cryptography;

using Moq;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Files;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Files;


public class MegFileTest
{
    private readonly Mock<IServiceProvider> _serviceProvider = new();
    private readonly MockFileSystem _fileSystem = new();

    public MegFileTest()
    {
        _serviceProvider.Setup(sp => sp.GetService(typeof(IFileSystem))).Returns(_fileSystem);
    }

    [Fact]
    public void Test_Ctor_ThrowsArgumentNullException()
    {
        var param = new MegFileInformation("test.meg", MegFileVersion.V1);
        var model = new Mock<IMegArchive>();

        Assert.Throws<ArgumentNullException>(() => new MegFile(null!, param, _serviceProvider.Object));
        Assert.Throws<ArgumentNullException>(() => new MegFile(model.Object, null!, _serviceProvider.Object));
        Assert.Throws<ArgumentNullException>(() => new MegFile(model.Object, param, null!));
    }

    [Fact]
    public void Test_Ctor_SetupProperties()
    {
        const string name = "test.meg";
        var param = new MegFileInformation(name, MegFileVersion.V2);
        var model = new Mock<IMegArchive>().Object;

        _fileSystem.Initialize().WithFile("test.meg");

        var megFile = new MegFile(model, param, _serviceProvider.Object);

        Assert.Same(model, megFile.Content);
        Assert.Same(model, megFile.Archive);
        Assert.Equal(MegFileVersion.V2, megFile.FileInformation.FileVersion);
        Assert.False(megFile.FileInformation.HasEncryption);

        Assert.Equal(_fileSystem.Path.GetFullPath(name), megFile.FileInformation.FilePath);
    }

    [Fact]
    public void Test_Ctor_SetupProperties_DiposeFileInfoParam()
    {
        const string name = "test.meg";

        var encData = MegEncryptionDataTest.CreateRandomData();

        var copyKey = encData.Key;
        
        var param = new MegFileInformation(name, MegFileVersion.V3, encData);
        var model = new Mock<IMegArchive>().Object;

        _fileSystem.Initialize().WithFile("test.meg");

        var megFile = new MegFile(model, param, _serviceProvider.Object);

        param.Dispose();

        Assert.True(param.EncryptionData!.IsDisposed);

        Assert.NotNull(megFile.FileInformation.EncryptionData);
        Assert.Equal(megFile.FileInformation.EncryptionData.Key, copyKey);
    }

    [Fact]
    public void Test_Ctor_SetupProperties_Encrypted()
    {
        var key = new byte[16];
        var iv = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(key);
        RandomNumberGenerator.Create().GetNonZeroBytes(iv);

        var encData = new MegEncryptionData(key, iv);

        var param = new MegFileInformation("test.meg", MegFileVersion.V3, encData);
        var model = new Mock<IMegArchive>().Object;

        _fileSystem.Initialize().WithFile("test.meg");

        var megFile = new MegFile(model, param, _serviceProvider.Object);

        Assert.Same(model, megFile.Content);
        Assert.Equal(MegFileVersion.V3, megFile.FileInformation.FileVersion);
        Assert.True(megFile.FileInformation.HasEncryption);
        Assert.Equal(iv, megFile.FileInformation.EncryptionData!.IV);
        Assert.Equal(key, megFile.FileInformation.EncryptionData!.Key);
    }

    [Fact]
    public void Test_EncryptionKeyHandling()
    {
        var keyIv = new byte[16];
        RandomNumberGenerator.Create().GetNonZeroBytes(keyIv);

        var encData = new MegEncryptionData(keyIv, keyIv);
        
        var param = new MegFileInformation("test.meg", MegFileVersion.V3, encData);
        var model = new Mock<IMegArchive>().Object;

        _fileSystem.Initialize().WithFile("test.meg");

        var megFile = new MegFile(model, param, _serviceProvider.Object);

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