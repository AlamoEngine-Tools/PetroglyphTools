using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Test.Files;

[TestClass]
public class MegFileInformationTest
{
    [TestMethod]
    public void Test_Ctor_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new MegFileInformation(null!, MegFileVersion.V2));
        Assert.ThrowsException<ArgumentException>(() => new MegFileInformation("", MegFileVersion.V2));
        Assert.ThrowsException<ArgumentException>(() => new MegFileInformation("path", MegFileVersion.V1, MegEncryptionDataTest.CreateRandomData()));
        Assert.ThrowsException<ArgumentException>(() => new MegFileInformation("path", MegFileVersion.V2, MegEncryptionDataTest.CreateRandomData()));
    }

    [TestMethod]
    [DataRow(MegFileVersion.V1)]
    [DataRow(MegFileVersion.V2)]
    [DataRow(MegFileVersion.V3)]
    public void Test_Ctor(MegFileVersion version)
    {
        var fileInfo = new MegFileInformation("path", version);
        Assert.AreEqual("path", fileInfo.FilePath);
        Assert.AreEqual(version, fileInfo.FileVersion);
        Assert.IsNull(fileInfo.EncryptionData);
        Assert.IsFalse(fileInfo.HasEncryption);
    }

    [TestMethod]
    public void Test_Ctor_Encrypted()
    {
        var encData = MegEncryptionDataTest.CreateRandomData();
        var fileInfo = new MegFileInformation("path", MegFileVersion.V3, encData);
        Assert.AreEqual("path", fileInfo.FilePath);
        Assert.AreEqual(MegFileVersion.V3, fileInfo.FileVersion);
        Assert.AreSame(encData, fileInfo.EncryptionData);
        Assert.IsTrue(fileInfo.HasEncryption);
    }

    [TestMethod]
    public void Test_Dispose()
    {
        var encData = MegEncryptionDataTest.CreateRandomData();
        var fileInfo = new MegFileInformation("path", MegFileVersion.V3, encData);
        fileInfo.Dispose();
        Assert.IsTrue(encData.IsDisposed);
    }

    [TestMethod]
    public void Test_CopyRecord()
    {
        var encData = MegEncryptionDataTest.CreateRandomData();
        var orgKey = encData.Key;
        var orgIv = encData.IV;
        var fileInfo = new MegFileInformation("path", MegFileVersion.V3, encData);

        var other = fileInfo with { FilePath = "otherPath"};
        Assert.AreEqual("otherPath", other.FilePath);
        Assert.AreEqual(MegFileVersion.V3, other.FileVersion);
        Assert.AreNotSame(encData, other.EncryptionData);
        Assert.IsTrue(other.HasEncryption);
        CollectionAssert.AreEqual(orgKey, other.EncryptionData.Key);
        CollectionAssert.AreEqual(orgIv, other.EncryptionData.IV);

        fileInfo.Dispose();

        CollectionAssert.AreEqual(orgKey, other.EncryptionData.Key);
        CollectionAssert.AreEqual(orgIv, other.EncryptionData.IV);

        Assert.ThrowsException<ObjectDisposedException>(() => fileInfo with { });
    }
}