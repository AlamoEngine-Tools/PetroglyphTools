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
    public void Test_Ctor_OriginIsEntryReference()
    {
        var meg = new Mock<IMegFile>();
        var origin = new MegDataEntryOriginInfo(new MegDataEntryLocationReference(meg.Object, 
            MegDataEntryTest.CreateEntry("path", default, 123, 321, true)));

        var info = new MegFileDataEntryBuilderInfo(origin);

        Assert.AreSame(origin, info.OriginInfo);
        Assert.AreEqual("path", info.FilePath);
        Assert.AreEqual(321u, info.Size);
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

    [TestMethod]
    public void Test_Ctor_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new MegFileDataEntryBuilderInfo(null!));
    }
}