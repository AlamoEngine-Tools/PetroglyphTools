using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;

namespace PG.StarWarsGame.Files.MEG.Test.Data;

[TestClass]
public class MegFileDataEntryBuilderInfoTest
{
    [TestMethod]
    public void Test_Ctor()
    {
        var origin = new MegDataEntryOriginInfo("path");
        var info = new MegFileDataEntryBuilderInfo(origin)
        {
            OverrideEncrypted = true,
            OverrideFileName = "PATH"
        };

        Assert.AreSame(origin, info.OriginInfo);
        Assert.AreEqual("PATH", info.OverrideFileName);
        Assert.IsTrue(info.OverrideEncrypted);
    }

    [TestMethod]
    public void Test_Ctor_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new MegFileDataEntryBuilderInfo(null!));
    }
}