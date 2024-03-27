using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Hashing;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Test.Data;

[TestClass]
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

    [TestMethod]
    public void Test_Ctor_NotSorted_ThrowsInvalidOperationException()
    {
        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            CreateSortedModel(new List<DatStringEntry>
            {
                new("2", new Crc32(2), "value2"),
                new("1", new Crc32(1), "value1"),
            });
        });
    }

    [TestMethod]
    public void Test_ToUnsortedModel()
    {
        var model = CreateSortedModel(CreateDataEntries());
        var unsorted = model.ToUnsortedModel();

        CollectionAssert.AreEqual(model.ToList(), unsorted.ToList());
        Assert.AreEqual(DatFileType.NotOrdered, unsorted.KeySortOder);
    }
}

[TestClass]
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

    [TestMethod]
    public void Test_ToSortedModel()
    {
        var model = CreateUnsortedModel(CreateDataEntries());
        var sorted = model.ToSortedModel();
        
        Assert.IsTrue(Crc32Utilities.IsSortedByCrc32(sorted));
        Assert.AreEqual(DatFileType.OrderedByCrc32, sorted.KeySortOder);
    }
}


[TestClass]
public abstract class DatModelTest
{ 
    protected abstract DatFileType ExpectedFileType { get; }

    protected abstract IDatModel CreateModel(IList<DatStringEntry> entries);

    protected abstract IList<DatStringEntry> CreateDataEntries();

    [TestMethod]
    public void Test_Ctor_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() => CreateModel(null!));
    }

    [TestMethod]
    public void Test_Ctor()
    {
        var model = CreateModel(CreateDataEntries());

        Assert.AreEqual(ExpectedFileType, model.KeySortOder);
        Assert.AreEqual(4, model.Count);
        CollectionAssert.AreEquivalent(new HashSet<string>{ "1", "3", "4" }.ToList(), model.Keys.ToList());
        CollectionAssert.AreEquivalent(new HashSet<Crc32> { new(1), new(3), new(4) }.ToList(), model.CrcKeys.ToList());
    }

    [TestMethod]
    public void Test_Enumerate()
    {
        var entries = CreateDataEntries();

        var model = CreateModel(entries);
        var modelEntries = model.ToList();
        CollectionAssert.AreEqual(entries.ToList(), modelEntries);

        modelEntries.Clear();
        var enumerable = (IEnumerable)model;
        foreach (var obj in enumerable) 
            modelEntries.Add((DatStringEntry)obj);
        CollectionAssert.AreEqual(entries.ToList(), modelEntries);
    }

    [TestMethod]
    public void Test_ContainsKey()
    {
        var entry1 = new DatStringEntry("1", new Crc32(1), "value1");

        var entries = new List<DatStringEntry>
        {
            entry1,
        };

        var model = CreateModel(entries);

        Assert.IsTrue(model.ContainsKey("1"));
        Assert.IsTrue(model.ContainsKey(new Crc32(1)));

        Assert.IsFalse(model.ContainsKey("11"));
        Assert.IsFalse(model.ContainsKey(new Crc32(11)));
    }

    [TestMethod]
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

        Assert.AreEqual("value1", model.GetValue("1"));
        model.TryGetValue("1", out var value);
        Assert.AreEqual("value1", value);

        Assert.AreEqual("value1", model.GetValue(new Crc32(1)));
        model.TryGetValue(new Crc32(1), out value);
        Assert.AreEqual("value1", value);


        Assert.ThrowsException<KeyNotFoundException>(() => model.GetValue("11"));
        model.TryGetValue("11", out value);
        Assert.IsNull(value);

        Assert.ThrowsException<KeyNotFoundException>(() => model.GetValue(new Crc32(11)));
        model.TryGetValue(new Crc32(11), out value);
        Assert.IsNull(value);
    }

    [TestMethod]
    public void Test_EntriesWithCrc()
    {
        var model = CreateModel(CreateDataEntries());
        var entriesWithCrc1 = model.EntriesWithCrc(new Crc32(1));
        CollectionAssert.AreEqual(new List<string>
        {
            "value1", "value2"
        }, entriesWithCrc1.Select(e => e.Value).ToList());

        var noEntries = model.EntriesWithCrc(new Crc32(11));
        CollectionAssert.AreEqual(new List<DatStringEntry>(), noEntries.ToList());
    }

    [TestMethod]
    public void Test_EntriesWithKey()
    {
        var model = CreateModel(CreateDataEntries());
        var entriesWithCrc1 = model.EntriesWithKey("1");
        CollectionAssert.AreEqual(new List<string>
        {
            "value1", "value2"
        }, entriesWithCrc1.Select(e => e.Value).ToList());

        var noEntries = model.EntriesWithKey("11");
        CollectionAssert.AreEqual(new List<DatStringEntry>(), noEntries.ToList());
    }
}