using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Test.Data.Archives;

[TestClass]
public class MegDataEntryHolderBaseTest
{

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_Ctor_Throw_NullArgument()
    {
        _ = new TestArchive(null!);
    }

    [TestMethod]
    public void Test_Ctor()
    {
        var entry1 = new Mock<IMegDataEntry>().Object;
        var entry2 = new Mock<IMegDataEntry>().Object;
        var entries = new List<IMegDataEntry>
        {
            entry1, entry2
        };

        var archive = new TestArchive(entries);

        Assert.AreEqual(2, archive.Count);
        Assert.AreSame(entry1, archive[0]);
        Assert.AreSame(entry2, archive[1]);


        var newEntries = new List<IMegDataEntry>();
        foreach (var entry in (IEnumerable)archive)
            newEntries.Add((IMegDataEntry)entry);

        CollectionAssert.AreEqual(entries, newEntries);
    }

    [TestMethod]
    public void Test_IndexOf_Contains()
    {
        var entry1 = new Mock<IMegDataEntry>().Object;
        var entry2 = new Mock<IMegDataEntry>().Object;
        var entries = new List<IMegDataEntry>
        {
            entry1, entry2
        };

        var archive = new TestArchive(entries);

        var entry3 = new Mock<IMegDataEntry>().Object;

        Assert.AreEqual(0, archive.IndexOf(entry1));
        Assert.AreEqual(1, archive.IndexOf(entry2));
        Assert.AreEqual(-1, archive.IndexOf(entry3));

        Assert.IsTrue(archive.Contains(entry1));
        Assert.IsTrue(archive.Contains(entry2));
        Assert.IsFalse(archive.Contains(entry3));
    }

    [TestMethod]
    public void Test_EntriesWithCrc_LastEntryWithCrc()
    {
        var entry1 = new Mock<IMegDataEntry>();
        entry1.SetupGet(e => e.FilePath).Returns("a");

        var entry2 = new Mock<IMegDataEntry>();
        entry2.SetupGet(e => e.FilePath).Returns("b");

        var entry3 = new Mock<IMegDataEntry>();
        entry3.SetupGet(e => e.Crc32).Returns(new Crc32(1));

        var entries = new List<IMegDataEntry>
        {
            entry1.Object,
            entry2.Object,
            entry3.Object
        };

        var archive = new TestArchive(entries);

        var twoFound = archive.EntriesWithCrc(new Crc32(0));
        Assert.AreEqual(2, twoFound.Count);
        Assert.AreEqual("a", twoFound[0].FilePath);
        Assert.AreEqual("b", twoFound[1].FilePath);

        var oneFound = archive.EntriesWithCrc(new Crc32(1));
        Assert.AreEqual(1, oneFound.Count);

        var noneFound = archive.EntriesWithCrc(new Crc32(-1));
        Assert.AreEqual(0, noneFound.Count);

        var last = archive.LastEntryWithCrc(new Crc32(0));
        Assert.AreEqual("b", last!.FilePath);

        var lastButNone = archive.LastEntryWithCrc(new Crc32(-1));
        Assert.IsNull(lastButNone);
    }

    [TestMethod]
    [DataRow("*", new string[] { }, new string[] { })]
    [DataRow("*", new[] { "a.txt", "new/a.txt" }, new[] { "a.txt" })]
    [DataRow("**/*", new[] { "a.txt", "new/a.txt", "c:/a.txt" }, new[] { "a.txt", "new/a.txt", "c:/a.txt" })]
    [DataRow("**/*.xml", new[] { "a.txt", "a.xml", "new/a.txt", "xml/a.xml" }, new[] { "a.xml", "xml/a.xml" })]
    [DataRow("*_eng.mp3", new[] { "a.mp3", "a_eng.mp3", "a_ger.mp3", "b_eng.mp3", "b_eng.mp2" }, new[] { "a_eng.mp3", "b_eng.mp3" })]
    [DataRow("a*_eng.mp3", new[] { "a_eng.mp3", "ab_eng.mp3", "a_ger.mp3", "b_eng.mp3" }, new[] { "a_eng.mp3", "ab_eng.mp3" })]
    [DataRow("**/a.*", new[] { "a.txt", "a.xml", "new/b.txt", "xml/a.xml" }, new[] { "a.txt", "a.xml", "xml/a.xml" })]
    [DataRow("a.txt", new[] { "a.txt", "a.xml", "new/a.txt", "c:/a.txt" }, new[] { "a.txt" })]
    [DataRow("a.txt", new[] { "a.xml" }, new string[] { })]
    [DataRow("**/a.txt", new[] { "a.txt", "b.txt", "new/a.txt" }, new[] { "a.txt", "new/a.txt" })]
    [DataRow("**/new/a.txt", new[] { "a.txt", "c:/new/a.txt", "new/a.txt" }, new[] { "c:/new/a.txt", "new/a.txt" })]
    // Checks case insensitivity
    [DataRow("NEW/a.txt", new[] { "a.txt", "a.xml", "new/a.txt" }, new[] { "new/a.txt" })]
    // Below are unusual cases and the behavior is highly dependent on the library used for globbing
    // On a real file system, this is a false positive ("new/../a.txt" should NOT be present)
    // and also this is a false negative ("./a.txt" should be present)
    [DataRow("a.txt", new[] { "./a.txt", "new/../a.txt" }, new string[] { })]
    // On a real file system, this is a false positive ("new/../a.txt" should NOT be present)
    [DataRow("new/**/a.txt", new[] { "new/../a.txt", "new/././a.txt" }, new[] { "new/../a.txt", "new/././a.txt" })]
    public void Test_FindAllEntries(string pattern, string[] files, string[] expectedMatches)
    {

        var megFiles = files.Select(f =>
        {
            var entry = new Mock<IMegDataEntry>();
            entry.SetupGet(e => e.FilePath).Returns(f);
            return entry.Object;
        }).ToList();
        var meg = new TestArchive(megFiles);
        var entries = meg.FindAllEntries(pattern).Select(e => e.FilePath).ToList();
        CollectionAssert.AreEqual(expectedMatches, entries);
    }

    private class TestArchive : MegDataEntryHolderBase<IMegDataEntry>
    {
        public TestArchive(IList<IMegDataEntry> entries) : base(entries)
        {
        }
    }
}