using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
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
    public void Test_Ctor_NotSorted_ThrowsInvalidOperationException()
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
    public void Test_ToUnsortedModel()
    {
        var model = CreateSortedModel(CreateDataEntries());
        var unsorted = model.ToUnsortedModel();

        Assert.Equal(model.ToList(), unsorted.ToList());
        Assert.Equal(DatFileType.NotOrdered, unsorted.KeySortOder);
    }
}

public class UnsortedDatModelTest : DatModelTest
{
    protected override DatFileType ExpectedFileType => DatFileType.NotOrdered;

    private UnsortedDatModel CreateUnsortedModel(IList<DatStringEntry> entries)
    {
        return new UnsortedDatModel(entries);
    }

    protected override IDatModel CreateModel(IList<DatStringEntry> entries)
    {
        return CreateUnsortedModel(entries);
    }

    protected override IList<DatStringEntry> CreateDataEntries()
    {
        var entry1 = new DatStringEntry("1", new Crc32(1), "value1");
        var entry2 = new DatStringEntry("1", new Crc32(1), "value2");
        var entry3 = new DatStringEntry("3", new Crc32(3), "value3");
        var entry4 = new DatStringEntry("4", new Crc32(4), "value4");

        var entries = new List<DatStringEntry>
        {
            entry3,
            entry4,
            entry1,
            entry2,
        };

        return entries;
    }

    [Fact]
    public void Test_ToSortedModel()
    {
        var model = CreateUnsortedModel(CreateDataEntries());
        var sorted = model.ToSortedModel();
        
        Assert.True(Crc32Utilities.IsSortedByCrc32(sorted));
        Assert.Equal(DatFileType.OrderedByCrc32, sorted.KeySortOder);
    }
}

public abstract class DatModelTest
{ 
    protected abstract DatFileType ExpectedFileType { get; }

    protected abstract IDatModel CreateModel(IList<DatStringEntry> entries);

    protected abstract IList<DatStringEntry> CreateDataEntries();

    [Fact]
    public void Test_Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => CreateModel(null!));
    }

    [Fact]
    public void Test_Ctor()
    {
        var model = CreateModel(CreateDataEntries());

        Assert.Equal(ExpectedFileType, model.KeySortOder);
        Assert.Equal(4, model.Count);
        Assert.Equivalent(new HashSet<string>{ "1", "3", "4" }.ToList(), model.Keys.ToList());
        Assert.Equivalent(new HashSet<Crc32> { new(1), new(3), new(4) }.ToList(), model.CrcKeys.ToList());
    }

    [Fact]
    public void Test_Enumerate()
    {
        var entries = CreateDataEntries();

        var model = CreateModel(entries);
        var modelEntries = model.ToList();
        Assert.Equal(entries.ToList(), modelEntries);

        modelEntries.Clear();
        var enumerable = (IEnumerable)model;
        foreach (var obj in enumerable) 
            modelEntries.Add((DatStringEntry)obj);
        Assert.Equal(entries.ToList(), modelEntries);
    }

    [Fact]
    public void Test_ContainsKey()
    {
        var entry1 = new DatStringEntry("1", new Crc32(1), "value1");

        var entries = new List<DatStringEntry>
        {
            entry1,
        };

        var model = CreateModel(entries);

        Assert.True(model.ContainsKey("1"));
        Assert.True(model.ContainsKey(new Crc32(1)));

        Assert.False(model.ContainsKey("11"));
        Assert.False(model.ContainsKey(new Crc32(11)));
    }

    [Fact]
    public void Test_GetValueTryGetValue()
    {
        var entry1 = new DatStringEntry("1", new Crc32(1), "value1");
        var entry2 = new DatStringEntry("1", new Crc32(1), "value2");

        var entries = new List<DatStringEntry>
        {
            entry1,
            entry2
        };

        var model = CreateModel(entries);

        Assert.Equal("value1", model.GetValue("1"));
        model.TryGetValue("1", out var value);
        Assert.Equal("value1", value);

        Assert.Equal("value1", model.GetValue(new Crc32(1)));
        model.TryGetValue(new Crc32(1), out value);
        Assert.Equal("value1", value);


        Assert.Throws<KeyNotFoundException>(() => model.GetValue("11"));
        model.TryGetValue("11", out value);
        Assert.Null(value);

        Assert.Throws<KeyNotFoundException>(() => model.GetValue(new Crc32(11)));
        model.TryGetValue(new Crc32(11), out value);
        Assert.Null(value);
    }

    [Fact]
    public void Test_EntriesWithCrc()
    {
        var model = CreateModel(CreateDataEntries());
        var entriesWithCrc1 = model.EntriesWithCrc(new Crc32(1));
        Assert.Equal(new List<string>
        {
            "value1", "value2"
        }, entriesWithCrc1.Select(e => e.Value).ToList());

        var noEntries = model.EntriesWithCrc(new Crc32(11));
        Assert.Equal(new List<DatStringEntry>(), noEntries.ToList());
    }

    [Fact]
    public void Test_EntriesWithKey()
    {
        var model = CreateModel(CreateDataEntries());
        var entriesWithCrc1 = model.EntriesWithKey("1");
        Assert.Equal(new List<string>
        {
            "value1", "value2"
        }, entriesWithCrc1.Select(e => e.Value).ToList());

        var noEntries = model.EntriesWithKey("11");
        Assert.Equal(new List<DatStringEntry>(), noEntries.ToList());
    }
}