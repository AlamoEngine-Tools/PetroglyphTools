using System;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Data;

public class SortedDatModelTest : DatModelTest
{
    protected override DatFileType ExpectedFileType => DatFileType.OrderedByCrc32;

    private SortedDatModel CreateSortedModel(IList<DatStringEntry> entries)
    {
        return new SortedDatModel(entries);
    }

    protected override IDatModel CreateModel(IList<DatStringEntry> entries)
    {
        return CreateSortedModel(entries);
    }

    protected override IList<DatStringEntry> CreateDataEntries()
    {
        var entry1 = new DatStringEntry("1", new Crc32(1), "value1");
        var entry2 = new DatStringEntry("1", new Crc32(1), "value2");
        var entry3 = new DatStringEntry("3", new Crc32(3), "value3");
        var entry4 = new DatStringEntry("4", new Crc32(4), "value4");

        var entries = new List<DatStringEntry>
        {
            entry1,
            entry2,
            entry3,
            entry4,
        };

        return entries;
    }

    [Fact]
    public void Ctor_NotSorted_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            CreateSortedModel(new List<DatStringEntry>
            {
                new("2", new Crc32(2), "value2"),
                new("1", new Crc32(1), "value1"),
            });
        });
    }

    [Fact]
    public void ToUnsortedModel()
    {
        var model = CreateSortedModel(CreateDataEntries());
        var unsorted = model.ToUnsortedModel();

        Assert.Equal(model.ToList(), unsorted.ToList());
        Assert.Equal(DatFileType.NotOrdered, unsorted.KeySortOrder);
    }
}