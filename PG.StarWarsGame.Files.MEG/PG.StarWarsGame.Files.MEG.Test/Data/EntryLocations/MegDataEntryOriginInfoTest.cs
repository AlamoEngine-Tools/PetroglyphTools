using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Test.Data.EntryLocations;

[TestClass]
public class MegDataEntryOriginInfoTest
{
    [TestMethod]
    public void Test_Ctor_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new MegDataEntryOriginInfo((string)null!));
        Assert.ThrowsException<ArgumentNullException>(() => new MegDataEntryOriginInfo((MegDataEntryReferenceLocation)null!));

        Assert.ThrowsException<ArgumentException>(() => new MegDataEntryOriginInfo(string.Empty));
        Assert.ThrowsException<ArgumentException>(() => new MegDataEntryOriginInfo("    "));
    }

    [TestMethod]
    public void Test_Ctor_Path()
    {
        var originInfo = new MegDataEntryOriginInfo("path");

        Assert.AreEqual("path", originInfo.FilePath);
        Assert.IsNull(originInfo.MegFileLocation);
    }

    [TestMethod]
    public void Test_Ctor_ReferenceLocation()
    {
        var meg = new Mock<IMegFile>().Object;
        var location = new MegDataEntryReferenceLocation(meg, MegDataEntryTest.CreateEntry("path"));

        var originInfo = new MegDataEntryOriginInfo(location);

        Assert.AreEqual(location, originInfo.MegFileLocation);
        Assert.IsNull(originInfo.FilePath);
    }

    [TestMethod]
    public void Test_HashCode()
    {
        var meg = new Mock<IMegFile>().Object;
        var location = new MegDataEntryReferenceLocation(meg, MegDataEntryTest.CreateEntry("path"));
        var otherLocation = new MegDataEntryReferenceLocation(meg, MegDataEntryTest.CreateEntry("path"));

        var originLoc = new MegDataEntryOriginInfo(location);
        var otherOriginLoc = new MegDataEntryOriginInfo(otherLocation);
        var originPath = new MegDataEntryOriginInfo("path");
        var otherOriginPath = new MegDataEntryOriginInfo("path");

        Assert.AreNotEqual(originLoc.GetHashCode(), originPath.GetHashCode());

        Assert.AreEqual(originLoc.GetHashCode(), otherOriginLoc.GetHashCode());
        Assert.AreEqual(originPath.GetHashCode(), otherOriginPath.GetHashCode());
    }

    [TestMethod]
    public void Test_Equals()
    {
        var meg = new Mock<IMegFile>().Object;
        var location = new MegDataEntryReferenceLocation(meg, MegDataEntryTest.CreateEntry("path"));
        var otherLocation = new MegDataEntryReferenceLocation(meg, MegDataEntryTest.CreateEntry("path"));

        var originLoc = new MegDataEntryOriginInfo(location);
        var otherOriginLoc = new MegDataEntryOriginInfo(otherLocation);
        var originPath = new MegDataEntryOriginInfo("path");
        var otherOriginPath = new MegDataEntryOriginInfo("path");


        Assert.AreEqual(originLoc, originLoc);
        Assert.AreEqual(originLoc, (object)originLoc);
        Assert.AreEqual(originLoc, otherOriginLoc);

        Assert.AreEqual(originPath, originPath);
        Assert.AreEqual(originPath, (object)originPath);
        Assert.AreEqual(originPath, otherOriginPath);

        Assert.IsFalse(originLoc.Equals(null));
        Assert.AreNotEqual(originLoc, (object?)null);

        Assert.IsFalse(originPath.Equals(null));
        Assert.AreNotEqual(originPath, (object?)null);

        Assert.AreNotEqual(originPath, originLoc);
        Assert.AreNotEqual(originPath, (object)originLoc);

        Assert.AreNotEqual(originPath, new MegDataEntryOriginInfo("PATH"));

        Assert.AreNotEqual(originLoc,
            new MegDataEntryOriginInfo(new MegDataEntryReferenceLocation(meg, MegDataEntryTest.CreateEntry("PATH"))));
    }
}