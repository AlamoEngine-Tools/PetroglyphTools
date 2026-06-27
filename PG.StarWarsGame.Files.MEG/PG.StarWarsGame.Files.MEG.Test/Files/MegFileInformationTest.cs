using System;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Files;

public class MegFileInformationTest
{
    [Fact]
    public void Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new MegFileInformation(null!, MegVersion.V2));
        Assert.Throws<ArgumentException>(() => new MegFileInformation("", MegVersion.V2));
        Assert.Throws<ArgumentException>(() => new MegFileInformation("path", MegVersion.V1, MegEncryptionDataTest.CreateRandomData()));
        Assert.Throws<ArgumentException>(() => new MegFileInformation("path", MegVersion.V2, MegEncryptionDataTest.CreateRandomData()));
    }

    [Theory]
    [InlineData(MegVersion.V1)]
    [InlineData(MegVersion.V2)]
    [InlineData(MegVersion.V3)]
    public void Ctor(MegVersion version)
    {
        var fileInfo = new MegFileInformation("path", version);
        Assert.Equal("path", fileInfo.FilePath);
        Assert.Equal(version, fileInfo.Version);
        Assert.Null(fileInfo.EncryptionData);
        Assert.False(fileInfo.HasEncryption);
    }

    [Fact]
    public void Ctor_Encrypted()
    {
        var encData = MegEncryptionDataTest.CreateRandomData();
        var fileInfo = new MegFileInformation("path", MegVersion.V3, encData);
        Assert.Equal("path", fileInfo.FilePath);
        Assert.Equal(MegVersion.V3, fileInfo.Version);
        Assert.Same(encData, fileInfo.EncryptionData);
        Assert.True(fileInfo.HasEncryption);
    }

    [Fact]
    public void Dispose()
    {
        var encData = MegEncryptionDataTest.CreateRandomData();
        var fileInfo = new MegFileInformation("path", MegVersion.V3, encData);
        fileInfo.Dispose();
        Assert.True(encData.IsDisposed);
    }

    [Fact]
    public void CopyRecord()
    {
        var encData = MegEncryptionDataTest.CreateRandomData();
        var orgKey = encData.Key;
        var orgIv = encData.IV;
        var fileInfo = new MegFileInformation("path", MegVersion.V3, encData);

        var other = fileInfo with { FilePath = "otherPath"};
        Assert.Equal("otherPath", other.FilePath);
        Assert.Equal(MegVersion.V3, other.Version);
        Assert.NotSame(encData, other.EncryptionData);
        Assert.True(other.HasEncryption);
        Assert.Equal(orgKey, other.EncryptionData.Key);
        Assert.Equal(orgIv, other.EncryptionData.IV);

        fileInfo.Dispose();

        Assert.Equal(orgKey, other.EncryptionData.Key);
        Assert.Equal(orgIv, other.EncryptionData.IV);

        Assert.Throws<ObjectDisposedException>(() => fileInfo with { });
    }
}