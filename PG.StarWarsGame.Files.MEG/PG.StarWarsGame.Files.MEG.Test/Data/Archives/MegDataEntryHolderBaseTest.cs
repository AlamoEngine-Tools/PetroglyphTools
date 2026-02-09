using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Data.Archives;

public abstract class MegDataEntryHolderBaseTest<TEntry, TArchive> : CommonMegTestBase 
    where TEntry : class, IMegDataEntry
    where TArchive : IMegDataEntryHolder<TEntry>
{
    protected abstract TArchive CreateArchive(IList<TEntry> entries);

    [Fact]
    public void Ctor_Throw_NullArgument()
    {
        Assert.ThrowsAny<ArgumentException>(() => CreateArchive(null!));
    }

    [Fact]
    public void Ctor()
    {
        var entry1 = CreateEntry("path");
        var entry2 = CreateEntry("other");
        var entries = new List<TEntry>
        {
            entry1, entry2
        };

        var archive = CreateArchive(entries);

        Assert.Equal(2, archive.Count);
        
        Assert.Same(entry1, archive[0]);
        Assert.Same(entry2, archive[1]);


        var newEntries = new List<IMegDataEntry>();
        foreach (var entry in (IEnumerable)archive)
            newEntries.Add((IMegDataEntry)entry);

        Assert.Equal(entries.Cast<IMegDataEntry>(), newEntries);
    }

    [Fact]
    public void Ctor_UnsortedEntries_Throws()
    {
        var entry1 = CreateEntry("path", new Crc32(1));
        var entry2 = CreateEntry("other", new Crc32(0));
        
        Assert.ThrowsAny<ArgumentException>(() => CreateArchive([entry1, entry2]));
    }

    [Fact]
    public void IndexOf_Contains()
    {
        var entry1 = CreateEntry("path");
        var entry2 = CreateEntry("other");
        var entries = new List<TEntry>
        {
            entry1, entry2
        };

        var archive = CreateArchive(entries);

        var entry3 = CreateEntry("third");

        Assert.Equal(0, archive.IndexOf(entry1));
        Assert.Equal(1, archive.IndexOf(entry2));
        Assert.Equal(-1, archive.IndexOf(entry3));

        Assert.True(archive.Contains(entry1));
        Assert.True(archive.Contains(entry2));
        Assert.False(archive.Contains(entry3));
    }

    [Fact]
    public void EntriesWithCrc_FirstEntryWithCrc()
    {
        var entry1 = CreateEntry("a", new Crc32(0));
        var entry2 = CreateEntry("b", new Crc32(0));
        var entry3 = CreateEntry("c", new Crc32(1));

        var entries = new List<TEntry>
        {
            entry1,
            entry2,
            entry3
        };

        var archive = CreateArchive(entries);

        var twoFound = archive.EntriesWithCrc(new Crc32(0));
        Assert.Equal(2, twoFound.Count);
        Assert.Equal("a", twoFound[0].Path);
        Assert.Equal("b", twoFound[1].Path);

        var oneFound = archive.EntriesWithCrc(new Crc32(1));
        Assert.Single(oneFound);

        var noneFound = archive.EntriesWithCrc(new Crc32(-1));
        Assert.Empty(noneFound);

        var first = archive.FirstEntryWithCrc(new Crc32(0));
        Assert.Equal("a", first.Path);

        Assert.Throws<KeyNotFoundException>(() => archive.FirstEntryWithCrc(new Crc32(-1)));
    }

    [Theory]
    [InlineData("*", new string[] { }, new string[] { })]
    [InlineData("*", new[] { "a.txt", "new/a.txt" }, new[] { "a.txt" })]
    [InlineData("**/*", new[] { "a.txt", "new/a.txt", "c:/a.txt" }, new[] { "a.txt", "new/a.txt", "c:/a.txt" })]
    [InlineData("**/*.xml", new[] { "a.txt", "a.xml", "new/a.txt", "xml/a.xml" }, new[] { "a.xml", "xml/a.xml" })]
    [InlineData("*_eng.mp3", new[] { "a.mp3", "a_eng.mp3", "a_ger.mp3", "b_eng.mp3", "b_eng.mp2" }, new[] { "a_eng.mp3", "b_eng.mp3" })]
    [InlineData("a*_eng.mp3", new[] { "a_eng.mp3", "ab_eng.mp3", "a_ger.mp3", "b_eng.mp3" }, new[] { "a_eng.mp3", "ab_eng.mp3" })]
    [InlineData("**/a.*", new[] { "a.txt", "a.xml", "new/b.txt", "xml/a.xml" }, new[] { "a.txt", "a.xml", "xml/a.xml" })]
    [InlineData("a.txt", new[] { "a.txt", "a.xml", "new/a.txt", "c:/a.txt" }, new[] { "a.txt" })]
    [InlineData("a.txt", new[] { "a.xml" }, new string[] { })]
    [InlineData("**/a.txt", new[] { "a.txt", "b.txt", "new/a.txt" }, new[] { "a.txt", "new/a.txt" })]
    [InlineData("**/new/a.txt", new[] { "a.txt", "c:/new/a.txt", "new/a.txt" }, new[] { "c:/new/a.txt", "new/a.txt" })]
    // Checks case sensitivity
    [InlineData("NEW/a.txt", new[] { "a.txt", "a.xml", "new/a.txt" }, new[] { "new/a.txt" }, true)]
    // Checks case insensitivity
    [InlineData("NEW/a.txt", new[] { "a.txt", "a.xml", "new/a.txt" }, new string[0])]
    // Below are unusual cases and the behavior is highly dependent on the library used for globbing
    // On a real file system, this is a false positive ("new/../a.txt" should NOT be present)
    // and also this is a false negative ("./a.txt" should be present)
    [InlineData("a.txt", new[] { "./a.txt", "new/../a.txt" }, new string[] { })]
    // On a real file system, this is a false positive ("new/../a.txt" should NOT be present)
    [InlineData("new/**/a.txt", new[] { "new/../a.txt", "new/././a.txt" }, new[] { "new/../a.txt", "new/././a.txt" })]
    public void FindAllEntries(string pattern, string[] files, string[] expectedMatches, bool caseInsensitive = false)
    {

        var megFiles = files.Select(f => CreateEntry(f)).ToList();
        var meg = CreateArchive(megFiles);
        var entries = meg.FindAllEntries(pattern, caseInsensitive).Select(e => e.Path).ToList();
        Assert.Equal(expectedMatches, entries);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void FindAllEntries_Throws(string? pattern)
    {
        var meg = CreateArchive([]);
        Assert.ThrowsAny<ArgumentException>(() => meg.FindAllEntries(pattern!, true));
    }

    protected abstract TEntry CreateEntry(string path, Crc32 crc = default);
}