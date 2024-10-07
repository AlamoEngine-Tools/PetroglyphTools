using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MTD.Data;
using Xunit;

namespace PG.StarWarsGame.Files.MTD.Test.Data;

public class MegaTextureDirectoryTest
{
    [Fact]
    public void Ctor_Duplicates_Throws()
    {
        Assert.Throws<DuplicateMtdEntryException>(() => new MegaTextureDirectory(new List<MegaTextureFileIndex>
        {
            new("abc", new Crc32(123), Rectangle.Empty, true),
            new("def", new Crc32(123), new Rectangle(1, 2, 3, 4), false),
        }));
    }

    [Fact]
    public void Enumerable()
    {
        var entry1 = new MegaTextureFileIndex("entry1", new Crc32(123), Rectangle.Empty, true);
        var entry2 = new MegaTextureFileIndex("entry2", new Crc32(456), Rectangle.Empty, true);

        var entryList = new List<MegaTextureFileIndex>{entry1, entry2};

        var mtd = new MegaTextureDirectory(entryList);

        entryList.Add(new MegaTextureFileIndex("distract", new Crc32(123), Rectangle.Empty, true));

        Assert.Equal(2, mtd.Count);

        using var enumerator = mtd.GetEnumerator();
        var objEnumerator = ((IEnumerable)mtd).GetEnumerator();

        var count = 0;
        while (enumerator.MoveNext() && objEnumerator.MoveNext())
        {
            var expected = entryList[count++];
            Assert.Equal(expected, enumerator.Current);
            Assert.Equal(expected, (MegaTextureFileIndex)objEnumerator.Current!);
        }

        Assert.Equal(2, count);
    }

    [Fact]
    public void Contains()
    {
        var entry1 = new MegaTextureFileIndex("entry1", new Crc32(123), Rectangle.Empty, true);
        var entry2 = new MegaTextureFileIndex("entry2", new Crc32(456), Rectangle.Empty, true);

        var entryList = new List<MegaTextureFileIndex> { entry1, entry2 };

        var mtd = new MegaTextureDirectory(entryList);

        Assert.True(mtd.Contains(new Crc32(123)));
        Assert.True(mtd.Contains(new Crc32(456)));
        Assert.False(mtd.Contains(new Crc32(0)));
        Assert.False(mtd.Contains(new Crc32(789)));
    }

    [Fact]
    public void TryGet()
    {
        var entry1 = new MegaTextureFileIndex("entry1", new Crc32(123), Rectangle.Empty, true);
        var entry2 = new MegaTextureFileIndex("entry2", new Crc32(456), Rectangle.Empty, true);

        var entryList = new List<MegaTextureFileIndex> { entry1, entry2 };

        var mtd = new MegaTextureDirectory(entryList);

        MegaTextureFileIndex actual;

        Assert.True(mtd.TryGetEntry(new Crc32(123), out actual));
        Assert.Equal(entry1, actual);

        Assert.True(mtd.TryGetEntry(new Crc32(456), out actual));
        Assert.Equal(entry2, actual);

        Assert.False(mtd.TryGetEntry(new Crc32(789), out actual));
        Assert.Null(actual);
    }
}