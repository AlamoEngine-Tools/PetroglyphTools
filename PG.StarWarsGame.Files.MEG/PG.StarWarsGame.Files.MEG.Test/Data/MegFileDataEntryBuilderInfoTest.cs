using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Test.Data;

[TestClass]
public class MegFileDataEntryBuilderInfoTest
{
    [TestMethod]
    public void Test_Ctor_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new MegFileDataEntryBuilderInfo(null!));
    }

    #region Ctor_LocalFile

    [TestMethod]
    public void Test_Ctor_OriginIsLocalFile()
    {
        var origin = new MegDataEntryOriginInfo("path");
        var info = new MegFileDataEntryBuilderInfo(origin);

        Assert.AreSame(origin, info.OriginInfo);
        Assert.AreEqual("path", info.FilePath);
        Assert.AreEqual(null, info.Size);
        Assert.IsFalse(info.Encrypted);
    }

    [TestMethod]
    public void Test_Ctor_OriginIsLocalFile_OverridesProperties()
    {
        var origin = new MegDataEntryOriginInfo("path");
        var info = new MegFileDataEntryBuilderInfo(origin, "PATH", 123, true);

        Assert.AreSame(origin, info.OriginInfo);
        Assert.AreEqual("PATH", info.FilePath);
        Assert.AreEqual(123u, info.Size);
        Assert.IsTrue(info.Encrypted);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("    ")]
    public void Test_Ctor_OriginIsLocalFile_OverridesProperties_PathEmpty_Throws(string path)
    {
        var origin = new MegDataEntryOriginInfo("path");
        Assert.ThrowsException<ArgumentException>(() => new MegFileDataEntryBuilderInfo(origin, path, 123, true));
    }

    #endregion

    #region Ctor_Entry

    [TestMethod]
    public void Test_Ctor_OriginIsEntryReference()
    {
        var meg = new Mock<IMegFile>();
        var origin = new MegDataEntryOriginInfo(new MegDataEntryLocationReference(meg.Object,
            MegDataEntryTest.CreateEntry("path", default, 123, 321, true)));

        var info = new MegFileDataEntryBuilderInfo(origin);

        Assert.AreSame(origin, info.OriginInfo);
        Assert.AreEqual("path", info.FilePath);
        Assert.AreEqual(321u, info.Size);
        Assert.AreEqual(123u, info.OriginInfo.MegFileLocation!.DataEntry.Location.Offset);
        Assert.IsTrue(info.Encrypted);
    }

    [TestMethod]
    public void Test_Ctor_OriginIsEntryReference_OverridesProperties()
    {
        var meg = new Mock<IMegFile>();
        var origin = new MegDataEntryOriginInfo(new MegDataEntryLocationReference(meg.Object,
            MegDataEntryTest.CreateEntry("path", default, 123, 321, true)));

        var info = new MegFileDataEntryBuilderInfo(origin, "PATH", 999, false);

        Assert.AreSame(origin, info.OriginInfo);
        Assert.AreEqual("PATH", info.FilePath);
        Assert.AreEqual(321u, info.Size); // Value 999 must be ignored
        Assert.IsFalse(info.Encrypted);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("    ")]
    public void Test_Ctor_OriginIsEntryReference_OverridesProperties_PathEmpty_Throws(string path)
    {
        var meg = new Mock<IMegFile>();
        var origin = new MegDataEntryOriginInfo(new MegDataEntryLocationReference(meg.Object,
            MegDataEntryTest.CreateEntry("path", default, 123, 321, true)));
        Assert.ThrowsException<ArgumentException>(() => new MegFileDataEntryBuilderInfo(origin, path, 123, true));
    }

    #endregion

    #region Factory_LocalFile

    [TestMethod]
    public void Test_Factory_FromFile()
    {
        var info = MegFileDataEntryBuilderInfo.FromFile("path", null);
        Assert.IsTrue(info.OriginInfo.IsLocalFile);
        Assert.AreEqual("path", info.FilePath);
        Assert.AreEqual(null, info.Size);
        Assert.IsFalse(info.Encrypted);
    }

    [TestMethod]
    public void Test_Factory_FromFile_OverridesProperties()
    {
        var info = MegFileDataEntryBuilderInfo.FromFile("path", "123", 123, true);
        Assert.IsTrue(info.OriginInfo.IsLocalFile);
        Assert.AreEqual("123", info.FilePath);
        Assert.AreEqual(123u, info.Size);
        Assert.IsTrue(info.Encrypted);
    }

    [TestMethod]
    [DataRow("", null)]
    [DataRow("   ", null)]
    [DataRow("   ", null)]
    [DataRow("test", "")]
    [DataRow("test", "   ")]
    public void Test_Factory_FromFile_Throws(string path, string overridePath)
    {
        Assert.ThrowsException<ArgumentException>(() => MegFileDataEntryBuilderInfo.FromFile(path, overridePath));
    }

    [TestMethod]
    public void Test_Factory_FromFile_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() => MegFileDataEntryBuilderInfo.FromFile(null!, "random"));
    }

    #endregion

    #region Factory_Entry

    [TestMethod]
    public void Test_Factory_OriginIsLocalFile()
    {
        var meg = new Mock<IMegFile>();
        var entry = MegDataEntryTest.CreateEntry("path", default, 123, 321, true);

        var info = MegFileDataEntryBuilderInfo.FromEntry(meg.Object, entry);

        Assert.IsTrue(info.OriginInfo.IsEntryReference);
        Assert.AreEqual("path", info.FilePath);
        Assert.AreEqual(321u, info.Size);
        Assert.AreEqual(123u, info.OriginInfo.MegFileLocation!.DataEntry.Location.Offset);
        Assert.IsTrue(info.Encrypted);
    }

    [TestMethod]
    public void Test_Factory_OriginIsEntryReference_OverridesProperties()
    {
        var meg = new Mock<IMegFile>();
        var entry = MegDataEntryTest.CreateEntry("path", default, 123, 321, true);

        var info = MegFileDataEntryBuilderInfo.FromEntry(meg.Object, entry, "PATH", false);

        Assert.IsTrue(info.OriginInfo.IsEntryReference);
        Assert.AreEqual("PATH", info.FilePath);
        Assert.AreEqual(321u, info.Size); // Value 999 must be ignored
        Assert.AreEqual(123u, info.OriginInfo.MegFileLocation!.DataEntry.Location.Offset);
        Assert.IsFalse(info.Encrypted);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("    ")]
    public void Test_Factory_OriginIsEntryReference_OverridesProperties_PathEmpty_Throws(string path)
    {
        var meg = new Mock<IMegFile>();
        var entry = MegDataEntryTest.CreateEntry("path", default, 123, 321, true);
        Assert.ThrowsException<ArgumentException>(() => MegFileDataEntryBuilderInfo.FromEntry(meg.Object, entry, path, false));
    }

    #endregion
}