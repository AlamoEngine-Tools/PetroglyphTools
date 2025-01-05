using System;
using System.Collections.Generic;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

public class VirtualMegArchiveBuilderTest : CommonMegTestBase
{
    [Fact]
    public void Test_BuildFrom_NullArgs_Throws()
    {
        var service = new VirtualMegArchiveBuilder();

        Assert.Throws<ArgumentNullException>(() => service.BuildFrom( null!));
        Assert.Throws<ArgumentNullException>(() => service.BuildFrom((List<IMegFile>) null!, false));
        Assert.Throws<ArgumentNullException>(() => service.BuildFrom((IEnumerable<MegDataEntryReference>) null!, false));

        Assert.Throws<ArgumentNullException>(() => service.BuildFrom((List<IMegFile>)null!, true));
        Assert.Throws<ArgumentNullException>(() => service.BuildFrom((IEnumerable<MegDataEntryReference>)null!, true));
    }

    [Fact]
    public void Test_BuildFrom_ListOfReferences_DoesNotExists_Throws()
    {
        var service = new VirtualMegArchiveBuilder();

        var entry1 = MegDataEntryTest.CreateEntry("A", new Crc32(1));

        FileSystem.File.Create("test.meg");
        var megFile = new MegFile(new MegArchive([]), new MegFileInformation("test.meg", MegFileVersion.V1), ServiceProvider);

        var entries = new List<MegDataEntryReference>
        {
            new(new(megFile, entry1)),
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

        FileSystem.File.Create("1.meg");
        var meg1 = new MegFile(new MegArchive([entry1]), new MegFileInformation("1.meg", MegFileVersion.V1), ServiceProvider);

        FileSystem.File.Create("2.meg");
        var meg2 = new MegFile(new MegArchive([entry2, entry3]), new MegFileInformation("2.meg", MegFileVersion.V1), ServiceProvider);

        var entries = new List<MegDataEntryReference>
        {
            new(new(meg1, entry1)),
            new(new(meg2, entry2)),
            new(new(meg2, entry3))
        };

        var archive = service.BuildFrom(entries, false);

        Assert.NotNull(archive);
        Assert.Equal(2, archive.Count);

        Assert.Equal(entry2, archive[0].Location.DataEntry);
        Assert.Equal(meg2, archive[0].Location.MegFile);

        Assert.Equal(entry1, archive[1].Location.DataEntry);
        Assert.Equal(meg1, archive[1].Location.MegFile);
    }

    [Fact]
    public void Test_BuildFrom_ListOfReferences_Replace()
    {
        var service = new VirtualMegArchiveBuilder();

        var entry1 = MegDataEntryTest.CreateEntry("A", new Crc32(1));
        var entry2 = MegDataEntryTest.CreateEntry("B", new Crc32(0));
        var entry3 = MegDataEntryTest.CreateEntry("C", new Crc32(1));

        FileSystem.File.Create("1.meg");
        var meg1 = new MegFile(new MegArchive([entry1]), new MegFileInformation("1.meg", MegFileVersion.V1), ServiceProvider);

        FileSystem.File.Create("2.meg");
        var meg2 = new MegFile(new MegArchive([entry2, entry3]), new MegFileInformation("2.meg", MegFileVersion.V1), ServiceProvider);

        var entries = new List<MegDataEntryReference>
        {
            new(new(meg1, entry1)),
            new(new(meg2, entry2)),
            new(new(meg2, entry3))
        };

        var archive = service.BuildFrom(entries, true);

        Assert.NotNull(archive);
        Assert.Equal(2, archive.Count);

        Assert.Equal(entry2, archive[0].Location.DataEntry);
        Assert.Equal(meg2, archive[0].Location.MegFile);

        Assert.Equal(entry3, archive[1].Location.DataEntry);
        Assert.Equal(meg2, archive[1].Location.MegFile);
    }

    [Fact]
    public void Test_BuildFrom_SingleMeg()
    {
        var service = new VirtualMegArchiveBuilder();

        var entry1 = MegDataEntryTest.CreateEntry("A", new Crc32(0));
        var entry2 = MegDataEntryTest.CreateEntry("B", new Crc32(0));
        var entry3 = MegDataEntryTest.CreateEntry("C", new Crc32(1));

        FileSystem.File.Create("test.meg");
        var megFile = new MegFile(new MegArchive([entry1, entry2, entry3]), new MegFileInformation("test.meg", MegFileVersion.V1), ServiceProvider);

        var archive = service.BuildFrom(megFile);

        Assert.NotNull(archive);
        Assert.Equal(2, archive.Count);

        Assert.Equal(entry1, archive[0].Location.DataEntry);
        Assert.Equal(megFile, archive[0].Location.MegFile);

        Assert.Equal(entry3, archive[1].Location.DataEntry);
        Assert.Equal(megFile, archive[1].Location.MegFile);
    }

    [Fact]
    public void Test_BuildFrom_ListOfMegs_DoNotReplace()
    {
        var service = new VirtualMegArchiveBuilder();

        var entry1_1 = MegDataEntryTest.CreateEntry("A", new Crc32(1));
        var entry1_2 = MegDataEntryTest.CreateEntry("B", new Crc32(1));

        var entry2_1 = MegDataEntryTest.CreateEntry("0", new Crc32(0));
        var entry2_2 = MegDataEntryTest.CreateEntry("C", new Crc32(1));
        var entry2_3 = MegDataEntryTest.CreateEntry("D", new Crc32(1));

        FileSystem.File.Create("1.meg");
        var meg1 = new MegFile(new MegArchive([entry1_1, entry1_2]), new MegFileInformation("1.meg", MegFileVersion.V1), ServiceProvider);

        FileSystem.File.Create("2.meg");
        var meg2 = new MegFile(new MegArchive([entry2_1, entry2_2, entry2_3]), new MegFileInformation("2.meg", MegFileVersion.V1), ServiceProvider);

        var archive = service.BuildFrom(new List<IMegFile>{meg1, meg2}, false);

        Assert.NotNull(archive);
        Assert.Equal(2, archive.Count);

        Assert.Equal(entry2_1, archive[0].Location.DataEntry);
        Assert.Equal(meg2, archive[0].Location.MegFile);

        Assert.Equal(entry1_1, archive[1].Location.DataEntry);
        Assert.Equal(meg1, archive[1].Location.MegFile);
    }

    [Fact]
    public void Test_BuildFrom_ListOfMegs_Replace()
    {
        var service = new VirtualMegArchiveBuilder();

        var entry1_1 = MegDataEntryTest.CreateEntry("A", new Crc32(1));
        var entry1_2 = MegDataEntryTest.CreateEntry("B", new Crc32(1));

        var entry2_1 = MegDataEntryTest.CreateEntry("0", new Crc32(0));
        var entry2_2 = MegDataEntryTest.CreateEntry("C", new Crc32(1));
        var entry2_3 = MegDataEntryTest.CreateEntry("D", new Crc32(1));

        FileSystem.File.Create("1.meg");
        var meg1 = new MegFile(new MegArchive([entry1_1, entry1_2]), new MegFileInformation("1.meg", MegFileVersion.V1), ServiceProvider);

        FileSystem.File.Create("2.meg");
        var meg2 = new MegFile(new MegArchive([entry2_1, entry2_2, entry2_3]), new MegFileInformation("2.meg", MegFileVersion.V1), ServiceProvider);

        var archive = service.BuildFrom(new List<IMegFile> { meg1, meg2 }, true);

        Assert.NotNull(archive);
        Assert.Equal(2, archive.Count);

        Assert.Equal(entry2_1, archive[0].Location.DataEntry);
        Assert.Equal(meg2, archive[0].Location.MegFile);
        
        Assert.Equal(entry2_2, archive[1].Location.DataEntry);
        Assert.Equal(meg2, archive[1].Location.MegFile);
    }
}