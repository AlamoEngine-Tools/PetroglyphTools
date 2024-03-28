using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Data.Archives;

public class ConstructingMegArchiveTest
{
    [Fact]
    public void Test_Ctor_Throw_NullArgument()
    {
        Assert.Throws<ArgumentNullException>(() =>new ConstructingMegArchive(null!, MegFileVersion.V1, false));
    }

    [Fact]
    public void Test_Ctor_Empty()
    {
        var entries = new List<VirtualMegDataEntryReference>();
        var cArchive = new ConstructingMegArchive(entries, MegFileVersion.V3, true);

        Assert.Equal(MegFileVersion.V3, cArchive.MegVersion);
        Assert.Equal(new MegArchive(new List<MegDataEntry>()).ToList(), cArchive.Archive.ToList());
        Assert.True(cArchive.Encrypted);
    }

    [Fact]
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

        var cArchive = new ConstructingMegArchive(entries, MegFileVersion.V3, false);

        Assert.Equal(MegFileVersion.V3, cArchive.MegVersion);
        Assert.False(cArchive.Encrypted);

        var expectedArchiveList = new List<MegDataEntry>
        {
            entry1, entry2
        };

        Assert.Equal(expectedArchiveList, cArchive.Archive.ToList());
    }
}