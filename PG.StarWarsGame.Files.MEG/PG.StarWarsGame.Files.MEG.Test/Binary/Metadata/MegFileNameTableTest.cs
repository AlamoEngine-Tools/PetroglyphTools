// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Metadata;


[TestClass]
public class MegFileNameTableTest
{
    [TestMethod]
    public void Ctor_Test__ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new MegFileNameTable(null!));
    }

    [TestMethod]
    public void Test__EmptyTable()
    {
        var table = new MegFileNameTable(new List<MegFileNameTableRecord>(0));
        Assert.AreEqual(0, table.Count);
        Assert.AreEqual(0, table.Size);
        CollectionAssert.AreEqual(new byte[]{}, table.Bytes);
    }

    [TestMethod]
    public void Test_Size_One_Entry()
    {
        MegFileNameTableRecord entry = new("abc");
        var table = new MegFileNameTable(new List<MegFileNameTableRecord>
        {
            entry
        });
        Assert.AreEqual(entry.Size, table.Size);
    }

    [TestMethod]
    public void Test_Size_2_Entries()
    {
        MegFileNameTableRecord entry = new("abc");
        var table = new MegFileNameTable(new List<MegFileNameTableRecord>
        {
            entry,
            entry
        });
        Assert.AreEqual(entry.Size * 2, table.Size);
    }

    [TestMethod]
    public void IFileNameTable_Test_Index()
    {
        MegFileNameTableRecord entry1 = new("123");
        MegFileNameTableRecord entry2 = new("456");
        var table = new MegFileNameTable(new List<MegFileNameTableRecord>
        {
            entry1,
            entry2
        });

        Assert.AreEqual(2, table.Count);
        Assert.AreEqual(entry1, table[0]);
        Assert.AreEqual(entry2, table[1]);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => table[2]);

        IMegFileNameTable ifaceTable = table;
        Assert.AreEqual(2, ifaceTable.Count);
        Assert.AreEqual("123", ifaceTable[0].FileName);
        Assert.AreEqual("456", ifaceTable[1].FileName);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ifaceTable[2]);
    }

    [TestMethod]
    public void IFileNameTable_Test_Enumerate()
    {
        MegFileNameTableRecord entry1 = new("123");
        MegFileNameTableRecord entry2 = new("456");

        var recordList = new List<MegFileNameTableRecord>
        {
            entry1,
            entry2
        };

        var table = new MegFileNameTable(recordList);

        var list = new List<MegFileNameTableRecord>();
        foreach (var record in table)
            list.Add(record);
        CollectionAssert.AreEqual(recordList, list);
    }

    [TestMethod]
    public void IFileNameTable_Test_EnumerateAsIEnumerable()
    {
        MegFileNameTableRecord entry1 = new("123");
        MegFileNameTableRecord entry2 = new("456");

        var recordList = new List<MegFileNameTableRecord>
        {
            entry1,
            entry2
        };

        IEnumerable table = new MegFileNameTable(recordList);

        var list = new List<MegFileNameTableRecord>();
        foreach (MegFileNameTableRecord record in table)
            list.Add(record);
        CollectionAssert.AreEqual(recordList, list);
    }

    [TestMethod]
    public void Test_Enumerate_AsIMegFileNameTable()
    {
        MegFileNameTableRecord entry1 = new("123");
        MegFileNameTableRecord entry2 = new("456");

        var recordList = new List<MegFileNameTableRecord>
        {
            entry1,
            entry2
        };

        var expectedNameInfoList = recordList.Select(r => new MegFileNameInformation(r.FileName, r.OriginalFilePath)).ToList();

        IMegFileNameTable table = new MegFileNameTable(recordList);
        var names = new List<MegFileNameInformation>();

        using var enumerator = table.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var currentTyped = enumerator.Current;
            var currentObj = ((IEnumerator)enumerator).Current;
            Assert.AreEqual(currentObj, currentTyped);

            names.Add(currentTyped);
        }

        CollectionAssert.AreEqual(expectedNameInfoList, names);

    }

    [TestMethod]
    public void Test_Enumerate_AsIMegFileNameTable_ResetEnumerator()
    {
        MegFileNameTableRecord entry1 = new("123");
        MegFileNameTableRecord entry2 = new("456");

        var recordList = new List<MegFileNameTableRecord>
        {
            entry1,
            entry2
        };


        IMegFileNameTable table = new MegFileNameTable(recordList);

        using var enumerator = table.GetEnumerator();
        enumerator.MoveNext();
        Assert.AreEqual(table[0].FileName, enumerator.Current.FileName);
        enumerator.Reset();
        Assert.AreEqual(default, enumerator.Current);
        enumerator.MoveNext();
        Assert.AreEqual(table[0].FileName, enumerator.Current.FileName);
    }

    [TestMethod]
    public void Test_Bytes()
    {
        MegFileNameTableRecord entry1 = new("a");
        MegFileNameTableRecord entry2 = new("b");
        var table = new MegFileNameTable(new List<MegFileNameTableRecord>
        {
            entry1,
            entry2
        });

        var expectedTableBytes = entry1.Bytes.Concat(entry2.Bytes).ToArray();
        CollectionAssert.AreEqual(expectedTableBytes, table.Bytes);
    }
}