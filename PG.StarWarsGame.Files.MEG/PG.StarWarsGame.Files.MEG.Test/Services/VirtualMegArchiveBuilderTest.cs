using System;
using System.Collections.Generic;

using Moq;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services;


public class VirtualMegArchiveBuilderTest
{
    [Fact]
    public void Test_BuildFrom_NullArgs_Throws()
    {
        var service = new VirtualMegArchiveBuilder();

        Assert.Throws<ArgumentNullException>(() => service.BuildFrom((IMegFile) null!, false));
        Assert.Throws<ArgumentNullException>(() => service.BuildFrom((List<IMegFile>) null!, false));
        Assert.Throws<ArgumentNullException>(() => service.BuildFrom((IEnumerable<MegDataEntryReference>) null!, false));

        Assert.Throws<ArgumentNullException>(() => service.BuildFrom((IMegFile)null!, true));
        Assert.Throws<ArgumentNullException>(() => service.BuildFrom((List<IMegFile>)null!, true));
        Assert.Throws<ArgumentNullException>(() => service.BuildFrom((IEnumerable<MegDataEntryReference>)null!, true));
    }

    [Fact]
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

        Assert.Throws<FileNotInMegException>(() => service.BuildFrom(entries, false));
        Assert.Throws<FileNotInMegException>(() => service.BuildFrom(entries, true));
    }

    [Fact]
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

        Assert.NotNull(archive);
        Assert.Equal(3, archive.Count);

        Assert.Equal(entry2, archive[0].Location.DataEntry);
        Assert.Equal(meg2.Object, archive[0].Location.MegFile);

        Assert.Equal(entry1, archive[1].Location.DataEntry);
        Assert.Equal(meg1.Object, archive[1].Location.MegFile);

        Assert.Equal(entry3, archive[2].Location.DataEntry);
        Assert.Equal(meg2.Object, archive[2].Location.MegFile);
    }

    [Fact]
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

        Assert.NotNull(archive);
        Assert.Equal(2, archive.Count);

        Assert.Equal(entry2, archive[0].Location.DataEntry);
        Assert.Equal(meg2.Object, archive[0].Location.MegFile);

        Assert.Equal(entry3, archive[1].Location.DataEntry);
        Assert.Equal(meg2.Object, archive[1].Location.MegFile);
    }

    [Fact]
    public void Test_BuildFrom_Meg_DoNotReplace()
    {
        var service = new VirtualMegArchiveBuilder();

        var entry1 = MegDataEntryTest.CreateEntry("A", new Crc32(0));
        var entry2 = MegDataEntryTest.CreateEntry("B", new Crc32(0));
        var entry3 = MegDataEntryTest.CreateEntry("C", new Crc32(1));

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(new MegArchive(new List<MegDataEntry> { entry1, entry2, entry3 }));

        var archive = service.BuildFrom(meg.Object, false);

        Assert.NotNull(archive);
        Assert.Equal(3, archive.Count);

        Assert.Equal(entry1, archive[0].Location.DataEntry);
        Assert.Equal(meg.Object, archive[0].Location.MegFile);

        Assert.Equal(entry2, archive[1].Location.DataEntry);
        Assert.Equal(meg.Object, archive[1].Location.MegFile);

        Assert.Equal(entry3, archive[2].Location.DataEntry);
        Assert.Equal(meg.Object, archive[2].Location.MegFile);
    }

    [Fact]
    public void Test_BuildFrom_Meg_Replace()
    {
        var service = new VirtualMegArchiveBuilder();

        var entry1 = MegDataEntryTest.CreateEntry("A", new Crc32(0));
        var entry2 = MegDataEntryTest.CreateEntry("B", new Crc32(0));
        var entry3 = MegDataEntryTest.CreateEntry("C", new Crc32(1));

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.Archive).Returns(new MegArchive(new List<MegDataEntry> { entry1, entry2, entry3 }));

        var archive = service.BuildFrom(meg.Object, true);

        Assert.NotNull(archive);
        Assert.Equal(2, archive.Count);

        Assert.Equal(entry2, archive[0].Location.DataEntry);
        Assert.Equal(meg.Object, archive[0].Location.MegFile);

        Assert.Equal(entry3, archive[1].Location.DataEntry);
        Assert.Equal(meg.Object, archive[1].Location.MegFile);
    }

    [Fact]
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

        Assert.NotNull(archive);
        Assert.Equal(3, archive.Count);

        Assert.Equal(entry2, archive[0].Location.DataEntry);
        Assert.Equal(meg2.Object, archive[0].Location.MegFile);

        Assert.Equal(entry1, archive[1].Location.DataEntry);
        Assert.Equal(meg1.Object, archive[1].Location.MegFile);

        Assert.Equal(entry3, archive[2].Location.DataEntry);
        Assert.Equal(meg2.Object, archive[2].Location.MegFile);
    }

    [Fact]
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

        Assert.NotNull(archive);
        Assert.Equal(2, archive.Count);

        Assert.Equal(entry2, archive[0].Location.DataEntry);
        Assert.Equal(meg2.Object, archive[0].Location.MegFile);
        
        Assert.Equal(entry3, archive[1].Location.DataEntry);
        Assert.Equal(meg2.Object, archive[1].Location.MegFile);
    }
}