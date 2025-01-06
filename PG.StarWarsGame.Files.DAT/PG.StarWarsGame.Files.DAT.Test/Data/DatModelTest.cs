using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Data;

public abstract class DatModelTest
{ 
    protected abstract DatFileType ExpectedFileType { get; }

    protected abstract IDatModel CreateModel(IList<DatStringEntry> entries);

    protected abstract IList<DatStringEntry> CreateDataEntries();

    [Fact]
    public void Ctor_NullArgs_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => CreateModel(null!));
    }

    [Fact]
    public void Ctor()
    {
        var model = CreateModel(CreateDataEntries());

        Assert.Equal(ExpectedFileType, model.KeySortOrder);
        Assert.Equal(4, model.Count);
        Assert.Equivalent(new HashSet<string>{ "1", "3", "4" }.ToList(), model.Keys.ToList());
        Assert.Equivalent(new HashSet<Crc32> { new(1), new(3), new(4) }.ToList(), model.CrcKeys.ToList());
    }

    [Fact]
    public void Enumerate()
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
    public void ContainsKey()
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
    public void GetValueTryGetValue()
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
    public void EntriesWithCrc()
    {
        var model = CreateModel(CreateDataEntries());
        var entriesWithCrc1 = model.EntriesWithCrc(new Crc32(1));
        Assert.Equal(["value1", "value2"], entriesWithCrc1.Select(e => e.Value).ToList());

        var noEntries = model.EntriesWithCrc(new Crc32(11));
        Assert.Equal([], noEntries.ToList());
    }

    [Fact]
    public void EntriesWithKey()
    {
        var model = CreateModel(CreateDataEntries());
        var entriesWithCrc1 = model.EntriesWithKey("1");
        Assert.Equal(["value1", "value2"], entriesWithCrc1.Select(e => e.Value).ToList());

        var noEntries = model.EntriesWithKey("11");
        Assert.Equal([], noEntries.ToList());
    }

    [Fact]
    public void FirstEntryWithCrc()
    {
        var model = CreateModel(CreateDataEntries());

        var value = model.FirstEntryWithCrc(new Crc32(1));
        Assert.Equal(new DatStringEntry("1", new Crc32(1), "value1"), value);

        Assert.Throws<KeyNotFoundException>(() => model.FirstEntryWithCrc(new Crc32(99)));
    }

    [Fact]
    public void FirstEntryWithKey()
    {
        var model = CreateModel(CreateDataEntries());

        var value = model.FirstEntryWithKey("1");
        Assert.Equal(new DatStringEntry("1", new Crc32(1), "value1"), value);

        Assert.Throws<KeyNotFoundException>(() => model.FirstEntryWithKey("99"));
    }
}