using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Test.Data;

[TestClass]
public class MegArchiveTest
{

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_Ctor_Throw_NullArgument()
    {
        _ = new MegArchive(null!);
    }

    [TestMethod]
    public void Test_Ctor()
    {
        var entry1 = new MegDataEntryIdentity(new Crc32(123), "path", 456, 789);
        var entry2 = new MegDataEntryIdentity(new Crc32(987), "test", 123, 456);
        var entries = new List<MegDataEntryIdentity>
        {
            entry1, entry2
        };

        var archive = new MegArchive(entries);

        Assert.AreEqual(2, archive.Count);
        Assert.AreEqual(entry1, archive[0]);
        Assert.AreEqual(entry2, archive[1]);


        var newEntries = new List<MegDataEntryIdentity>();
        foreach (var entry in (IEnumerable)archive) 
            newEntries.Add((MegDataEntryIdentity)entry);

        CollectionAssert.AreEqual(entries, newEntries);
    }

    [TestMethod]
    public void Test_IndexOf_Contains()
    {
        var entry1 = new MegDataEntryIdentity(new Crc32(123), "path", 456, 789);
        var entry2 = new MegDataEntryIdentity(new Crc32(987), "test", 123, 456);
        var entries = new List<MegDataEntryIdentity>
        {
            entry1, entry2
        };

        var archive = new MegArchive(entries);

        var entry3 = new MegDataEntryIdentity(new Crc32(456), "new", 789, 123);

        Assert.AreEqual(0, archive.IndexOf(entry1));
        Assert.AreEqual(1, archive.IndexOf(entry2));
        Assert.AreEqual(-1, archive.IndexOf(entry3));

        Assert.IsTrue(archive.Contains(entry1));
        Assert.IsTrue(archive.Contains(entry2));
        Assert.IsFalse(archive.Contains(entry3));
    }


    [TestMethod]
    [DataRow("*",             new string[] {  },                                                       new string[] { })]
    [DataRow("*",             new[] { "a.txt", "new/a.txt" },                                          new[] { "a.txt" })]
    [DataRow("**/*",          new[] { "a.txt", "new/a.txt", "c:/a.txt" },                              new[] { "a.txt", "new/a.txt", "c:/a.txt" })]
    [DataRow("**/*.xml",      new[] { "a.txt", "a.xml", "new/a.txt", "xml/a.xml" },                    new[] { "a.xml", "xml/a.xml" })]
    [DataRow("*_eng.mp3",     new[] { "a.mp3", "a_eng.mp3", "a_ger.mp3", "b_eng.mp3", "b_eng.mp2" },   new[] { "a_eng.mp3", "b_eng.mp3" })]
    [DataRow("a*_eng.mp3",    new[] { "a_eng.mp3", "ab_eng.mp3", "a_ger.mp3", "b_eng.mp3" },           new[] { "a_eng.mp3", "ab_eng.mp3" })]
    [DataRow("**/a.*",        new[] { "a.txt", "a.xml", "new/b.txt", "xml/a.xml" },                    new[] { "a.txt", "a.xml", "xml/a.xml" })]
    [DataRow("a.txt",         new[] { "a.txt", "a.xml", "new/a.txt", "c:/a.txt" },                     new[] { "a.txt" })]
    [DataRow("a.txt",         new[] { "a.xml" },                                                       new string[] { })]
    [DataRow("**/a.txt",      new[] { "a.txt", "b.txt", "new/a.txt" },                                 new[] { "a.txt", "new/a.txt" })]
    [DataRow("**/new/a.txt",  new[] { "a.txt", "c:/new/a.txt", "new/a.txt" },                          new[] { "c:/new/a.txt", "new/a.txt" })]
    // Checks case insensitivity
    [DataRow("NEW/a.txt", new[] { "a.txt", "a.xml", "new/a.txt" }, new[] { "new/a.txt" })]
    // Below are unusual cases and the behavior is highly dependent on the library used for globbing
    // On a real file system, this is a false positive ("new/../a.txt" should NOT be present)
    // and also this is a false negative ("./a.txt" should be present)
    [DataRow("a.txt",         new[] { "./a.txt", "new/../a.txt" },                                     new string[] { })]
    // On a real file system, this is a false positive ("new/../a.txt" should NOT be present)
    [DataRow("new/**/a.txt",  new[] { "new/../a.txt", "new/././a.txt" },                               new[] { "new/../a.txt", "new/././a.txt" })] 
    public void Test_FindAllEntries(string pattern, string[] files, string[] expectedMatches)
    {
        var megFiles = files.Select(CreateFromFile).ToList();
        var meg = new MegArchive(megFiles);
        var entries = meg.FindAllEntries(pattern).Select(e => e.FilePath).ToList();
        CollectionAssert.AreEqual(expectedMatches, entries);
    }

    private static MegDataEntryIdentity CreateFromFile(string path)
    {
        return new(new Crc32(0), path, 0, 0);
    }
}