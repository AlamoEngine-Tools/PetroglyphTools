using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Test.Data.Archives;

[TestClass]
public class ConstructingMegArchiveTest
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_Ctor_Throw_NullArgument()
    {
        _ = new ConstructingMegArchive(null!, MegFileVersion.V1);
    }

    [TestMethod]
    public void Test_Ctor_Empty()
    {
        var entries = new List<VirtualMegDataEntryReference>();
        var cArchive = new ConstructingMegArchive(entries, MegFileVersion.V3);

        Assert.AreEqual(MegFileVersion.V3, cArchive.MegVersion);
        CollectionAssert.AreEqual(new MegArchive(new List<MegDataEntry>()).ToList(), cArchive.Archive.ToList());
    }

    [TestMethod]
    public void Test_Ctor_CreateArchive()
    {
        var entry1 = MegDataEntryTest.CreateEntry("pathA", new Crc32(1), 1, 1, true);
        var entry2 = MegDataEntryTest.CreateEntry("pathB", new Crc32(2), 2, 2);

        var meg = new Mock<IMegFile>();
        var locEntry = MegDataEntryTest.CreateEntry("pathC", new Crc32(3), 3, 3);

        var reference1 = new VirtualMegDataEntryReference(
            entry1, new MegDataEntryOriginInfo("path"));

        var reference2 = new VirtualMegDataEntryReference(
            entry2, new MegDataEntryOriginInfo(new MegDataEntryLocationReference(meg.Object, locEntry)));

        var entries = new List<VirtualMegDataEntryReference>
        {
            reference1, reference2
        };

        var cArchive = new ConstructingMegArchive(entries, MegFileVersion.V3);

        Assert.AreEqual(MegFileVersion.V3, cArchive.MegVersion);

        var expectedArchiveList = new List<MegDataEntry>
        {
            entry1, entry2
        };

        CollectionAssert.AreEqual(expectedArchiveList, cArchive.Archive.ToList());
    }
}