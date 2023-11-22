using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Test.Data.EntryLocations;

[TestClass]
public class MegDataEntryLocationReferenceTest
{
    [TestMethod]
    public void Test_Ctor_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new MegDataEntryLocationReference(null!, MegDataEntryTest.CreateEntry("path")));

        Assert.ThrowsException<ArgumentNullException>(() =>
            new MegDataEntryLocationReference(new Mock<IMegFile>().Object, null!));
    }

    [TestMethod]
    public void Test_Ctor()
    {
        var meg = new Mock<IMegFile>().Object;
        var entry = MegDataEntryTest.CreateEntry("path");

        var reference = new MegDataEntryLocationReference(meg, entry);

        Assert.AreSame(meg, reference.MegFile);
        Assert.AreSame(entry, reference.DataEntry);
    }

    [TestMethod]
    public void Test_Equals_Hashcode()
    {
        var meg = new Mock<IMegFile>().Object;
        var entry = MegDataEntryTest.CreateEntry("path");

        var reference = new MegDataEntryLocationReference(meg, entry);

        var otherEqual = new MegDataEntryLocationReference(meg, entry);

        var otherNotEqualMeg = new MegDataEntryLocationReference(new Mock<IMegFile>().Object, entry);
        var otherNotEqualEntry = new MegDataEntryLocationReference(meg, MegDataEntryTest.CreateEntry("other"));

        Assert.AreEqual(reference, reference);
        Assert.AreEqual(reference, otherEqual);
        Assert.AreEqual(reference.GetHashCode(), otherEqual.GetHashCode());

        Assert.IsFalse(reference.Equals(null));
        Assert.AreNotEqual(reference, (object?)null);
        Assert.AreNotEqual(reference, new object());
        Assert.AreNotEqual(reference, otherNotEqualMeg);
        Assert.AreNotEqual(reference, otherNotEqualEntry);

        Assert.AreNotEqual(reference.GetHashCode(), otherNotEqualMeg.GetHashCode());
        Assert.AreNotEqual(reference.GetHashCode(), otherNotEqualEntry.GetHashCode());
    }
}