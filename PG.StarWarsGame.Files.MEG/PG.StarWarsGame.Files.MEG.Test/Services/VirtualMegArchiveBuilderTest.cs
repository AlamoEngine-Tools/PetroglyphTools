using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

[TestClass]
public class VirtualMegArchiveBuilderTest
{
    [TestMethod]
    public void Test_BuildFrom_NullArgs_Throws()
    {
        var service = new VirtualMegArchiveBuilder();

        Assert.ThrowsException<ArgumentNullException>(() => service.BuildFrom((IMegFile) null!, false));
        Assert.ThrowsException<ArgumentNullException>(() => service.BuildFrom((List<IMegFile>) null!, false));
        Assert.ThrowsException<ArgumentNullException>(() => service.BuildFrom((IEnumerable<MegDataEntryReference>) null!, false));

        Assert.ThrowsException<ArgumentNullException>(() => service.BuildFrom((IMegFile)null!, true));
        Assert.ThrowsException<ArgumentNullException>(() => service.BuildFrom((List<IMegFile>)null!, true));
        Assert.ThrowsException<ArgumentNullException>(() => service.BuildFrom((IEnumerable<MegDataEntryReference>)null!, true));
    }

    [TestMethod]
    public void Test_BuildFrom_ListOfReferences_DoesNotExists_Throws()
    {
        var service = new VirtualMegArchiveBuilder();

        var entry1 = MegDataEntryTest.CreateEntry("A", new Crc32(1));
        
        var meg1 = new Mock<IMegFile>();
        meg1.SetupGet(m => m.Archive).Returns(new MegArchive(new List<MegDataEntry>()));

        var entries = new List<MegDataEntryReference>
        {
            new(new(meg1.Object, entry1)),
        };

        Assert.ThrowsException<FileNotInMegException>(() => service.BuildFrom(entries, false));
        Assert.ThrowsException<FileNotInMegException>(() => service.BuildFrom(entries, true));
    }

    [TestMethod]
    public void Test_BuildFrom_ListOfReferences_DoNotReplace()
    {
        var service = new VirtualMegArchiveBuilder();

        var entry1 = MegDataEntryTest.CreateEntry("A", new Crc32(1));
        var entry2 = MegDataEntryTest.CreateEntry("B", new Crc32(0));
        var entry3 = MegDataEntryTest.CreateEntry("C", new Crc32(1));

        var meg1 = new Mock<IMegFile>();
        meg1.SetupGet(m => m.Archive).Returns(new MegArchive(new List<MegDataEntry> { entry1 }));

        var meg2 = new Mock<IMegFile>();
        meg2.SetupGet(m => m.Archive).Returns(new MegArchive(new List<MegDataEntry> { entry2, entry3 }));

        var entries = new List<MegDataEntryReference>
        {
            new(new(meg1.Object, entry1)),
            new(new(meg2.Object, entry2)),
            new(new(meg2.Object, entry3))
        };

        var archive = service.BuildFrom(entries, false);

        Assert.IsNotNull(archive);
        Assert.AreEqual(3, archive.Count);

        Assert.AreEqual(entry2, archive[0].Location.DataEntry);
        Assert.AreEqual(meg2.Object, archive[0].Location.MegFile);

        Assert.AreEqual(entry1, archive[1].Location.DataEntry);
        Assert.AreEqual(meg1.Object, archive[1].Location.MegFile);

        Assert.AreEqual(entry3, archive[2].Location.DataEntry);
        Assert.AreEqual(meg2.Object, archive[2].Location.MegFile);
    }

    [TestMethod]
    public void Test_BuildFrom_ListOfReferences_Replace()
    {
        var service = new VirtualMegArchiveBuilder();

        var entry1 = MegDataEntryTest.CreateEntry("A", new Crc32(1));
        var entry2 = MegDataEntryTest.CreateEntry("B", new Crc32(0));
        var entry3 = MegDataEntryTest.CreateEntry("C", new Crc32(1));

        var meg1 = new Mock<IMegFile>();
        meg1.SetupGet(m => m.Archive).Returns(new MegArchive(new List<MegDataEntry> { entry1 }));

        var meg2 = new Mock<IMegFile>();
        meg2.SetupGet(m => m.Archive).Returns(new MegArchive(new List<MegDataEntry> { entry2, entry3 }));

        var entries = new List<MegDataEntryReference>
        {
            new(new(meg1.Object, entry1)),
            new(new(meg2.Object, entry2)),
            new(new(meg2.Object, entry3))
        };

        var archive = service.BuildFrom(entries, true);

        Assert.IsNotNull(archive);
        Assert.AreEqual(2, archive.Count);

        Assert.AreEqual(entry2, archive[0].Location.DataEntry);
        Assert.AreEqual(meg2.Object, archive[0].Location.MegFile);

        Assert.AreEqual(entry3, archive[1].Location.DataEntry);
        Assert.AreEqual(meg2.Object, archive[1].Location.MegFile);
    }

    [TestMethod]
    public void Test_BuildFrom_Meg_DoNotReplace()
    {
        var service = new VirtualMegArchiveBuilder();

        var entry1 = MegDataEntryTest.CreateEntry("A", new Crc32(0));
        var entry2 = MegDataEntryTest.CreateEntry("B", new Crc32(0));
        var entry3 = MegDataEntryTest.CreateEntry("C", new Crc32(1));

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(new MegArchive(new List<MegDataEntry> { entry1, entry2, entry3 }));

        var archive = service.BuildFrom(meg.Object, false);

        Assert.IsNotNull(archive);
        Assert.AreEqual(3, archive.Count);

        Assert.AreEqual(entry1, archive[0].Location.DataEntry);
        Assert.AreEqual(meg.Object, archive[0].Location.MegFile);

        Assert.AreEqual(entry2, archive[1].Location.DataEntry);
        Assert.AreEqual(meg.Object, archive[1].Location.MegFile);

        Assert.AreEqual(entry3, archive[2].Location.DataEntry);
        Assert.AreEqual(meg.Object, archive[2].Location.MegFile);
    }

    [TestMethod]
    public void Test_BuildFrom_Meg_Replace()
    {
        var service = new VirtualMegArchiveBuilder();

        var entry1 = MegDataEntryTest.CreateEntry("A", new Crc32(0));
        var entry2 = MegDataEntryTest.CreateEntry("B", new Crc32(0));
        var entry3 = MegDataEntryTest.CreateEntry("C", new Crc32(1));

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(new MegArchive(new List<MegDataEntry> { entry1, entry2, entry3 }));

        var archive = service.BuildFrom(meg.Object, true);

        Assert.IsNotNull(archive);
        Assert.AreEqual(2, archive.Count);

        Assert.AreEqual(entry2, archive[0].Location.DataEntry);
        Assert.AreEqual(meg.Object, archive[0].Location.MegFile);

        Assert.AreEqual(entry3, archive[1].Location.DataEntry);
        Assert.AreEqual(meg.Object, archive[1].Location.MegFile);
    }

    [TestMethod]
    public void Test_BuildFrom_ListOfMegs_DoNotReplace()
    {
        var service = new VirtualMegArchiveBuilder();

        var entry1 = MegDataEntryTest.CreateEntry("A", new Crc32(1));
        var entry2 = MegDataEntryTest.CreateEntry("B", new Crc32(0));
        var entry3 = MegDataEntryTest.CreateEntry("C", new Crc32(1));

        var meg1 = new Mock<IMegFile>();
        meg1.SetupGet(m => m.Archive).Returns(new MegArchive(new List<MegDataEntry> { entry1 }));

        var meg2 = new Mock<IMegFile>();
        meg2.SetupGet(m => m.Archive).Returns(new MegArchive(new List<MegDataEntry> { entry2, entry3 }));

        var archive = service.BuildFrom(new List<IMegFile>{meg1.Object, meg2.Object}, false);

        Assert.IsNotNull(archive);
        Assert.AreEqual(3, archive.Count);

        Assert.AreEqual(entry2, archive[0].Location.DataEntry);
        Assert.AreEqual(meg2.Object, archive[0].Location.MegFile);

        Assert.AreEqual(entry1, archive[1].Location.DataEntry);
        Assert.AreEqual(meg1.Object, archive[1].Location.MegFile);

        Assert.AreEqual(entry3, archive[2].Location.DataEntry);
        Assert.AreEqual(meg2.Object, archive[2].Location.MegFile);
    }

    [TestMethod]
    public void Test_BuildFrom_ListOfMegs_Replace()
    {
        var service = new VirtualMegArchiveBuilder();

        var entry1 = MegDataEntryTest.CreateEntry("A", new Crc32(1));
        var entry2 = MegDataEntryTest.CreateEntry("B", new Crc32(0));
        var entry3 = MegDataEntryTest.CreateEntry("C", new Crc32(1));

        var meg1 = new Mock<IMegFile>();
        meg1.SetupGet(m => m.Archive).Returns(new MegArchive(new List<MegDataEntry> { entry1 }));

        var meg2 = new Mock<IMegFile>();
        meg2.SetupGet(m => m.Archive).Returns(new MegArchive(new List<MegDataEntry> { entry2, entry3 }));

        var archive = service.BuildFrom(new List<IMegFile> { meg1.Object, meg2.Object }, true);

        Assert.IsNotNull(archive);
        Assert.AreEqual(2, archive.Count);

        Assert.AreEqual(entry2, archive[0].Location.DataEntry);
        Assert.AreEqual(meg2.Object, archive[0].Location.MegFile);
        
        Assert.AreEqual(entry3, archive[1].Location.DataEntry);
        Assert.AreEqual(meg2.Object, archive[1].Location.MegFile);
    }
}