using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;

namespace PG.StarWarsGame.Files.DAT.Test.Binary.Metadata;

[TestClass]
public class IndexTableTest
{
    private static IndexTable CreateTable(IList<IndexTableRecord> indexTableRecords)
    {
        return new IndexTable(indexTableRecords);
    }

    private static IndexTableRecord CreateRecord(int index, int seed)
    {
        return new IndexTableRecord(new Crc32(seed), (uint)seed + 1, (uint)seed + 2);
    }

    [TestMethod]
    public void Ctor_Test__ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new IndexTable(null!));
    }

    [TestMethod]
    public void Test_Index()
    {
        var entry1 = CreateRecord(0, 1);
        var entry2 = CreateRecord(1, 2);
        var table = CreateTable(new List<IndexTableRecord>
        {
            entry1,
            entry2
        });

        Assert.AreEqual(2, table.Count);
        Assert.AreEqual(entry1, table[0]);
        Assert.AreEqual(entry2, table[1]);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => table[2]);
    }

    [TestMethod]
    public void Test_EmptyTable()
    {
        var table = CreateTable(new List<IndexTableRecord>(0));
        Assert.AreEqual(0, table.Count);
        Assert.AreEqual(0, table.Size);
        CollectionAssert.AreEqual(new byte[] { }, table.Bytes);
    }

    [TestMethod]
    public void Test_Size_1_Entry()
    {
        var entry = CreateRecord(0, 1);
        var table = CreateTable(new List<IndexTableRecord>
        {
            entry
        });
        Assert.AreEqual(entry.Size, table.Size);
    }

    [TestMethod]
    public void Test_Size_2_Entries()
    {
        var entry1 = CreateRecord(0, 1);
        var entry2 = CreateRecord(1, 2);
        var table = CreateTable(new List<IndexTableRecord>
        {
            entry1,
            entry2
        });
        Assert.AreEqual(entry1.Size + entry2.Size, table.Size);
    }

    [TestMethod]
    public void IFileNameTable_Test_Index()
    {
        var entry1 = CreateRecord(0, 1);
        var entry2 = CreateRecord(1, 2);
        var table = CreateTable(new List<IndexTableRecord>
        {
            entry1,
            entry2
        });

        Assert.AreEqual(2, table.Count);
        Assert.AreEqual(entry1, table[0]);
        Assert.AreEqual(entry2, table[1]);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => table[2]);
    }

    [TestMethod]
    public void Test_Enumerate()
    {
        var entry1 = CreateRecord(0, 1);
        var entry2 = CreateRecord(1, 2);

        var recordList = new List<IndexTableRecord>
        {
            entry1,
            entry2
        };

        var table = CreateTable(recordList);

        var list = new List<IndexTableRecord>();
        foreach (var record in table)
            list.Add(record);
        CollectionAssert.AreEqual(recordList, list);
    }

    [TestMethod]
    public void Test_Enumerate_AsIEnumerable()
    {
        var entry1 = CreateRecord(0, 1);
        var entry2 = CreateRecord(1, 2);

        var recordList = new List<IndexTableRecord>
        {
            entry1,
            entry2
        };

        IEnumerable table = CreateTable(recordList);

        var list = new List<IndexTableRecord>();
        foreach (IndexTableRecord record in table)
            list.Add(record);
        CollectionAssert.AreEqual(recordList, list);
    }

    [TestMethod]
    public void Test_Enumerate_ResetEnumerator()
    {
        var entry1 = CreateRecord(0, 1);
        var entry2 = CreateRecord(1, 2);

        var recordList = new List<IndexTableRecord>
        {
            entry1,
            entry2
        };

        var table = CreateTable(recordList);

        using var enumerator = table.GetEnumerator();
        enumerator.MoveNext();
        Assert.AreEqual(table[0], enumerator.Current);

        enumerator.Reset();

        enumerator.MoveNext();
        Assert.AreEqual(table[0], enumerator.Current);
    }

    [TestMethod]
    public void Test_Bytes()
    {
        var entry1 = CreateRecord(0, 1);
        var entry2 = CreateRecord(1, 2);
        var table = CreateTable(new List<IndexTableRecord>
        {
            entry1,
            entry2
        });

        var expectedTableBytes = entry1.Bytes.Concat(entry2.Bytes).ToArray();
        CollectionAssert.AreEqual(expectedTableBytes, table.Bytes);
    }
}