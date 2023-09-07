// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
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
    public void Ctor_Test__ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => new MegFileNameTable(new List<MegFileNameTableRecord>()));
    }

    [TestMethod]
    public void Test_Size()
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
        Assert.AreEqual("123", ifaceTable[0]);
        Assert.AreEqual("456", ifaceTable[1]);
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

        var names = new List<string>();
        IMegFileNameTable ifaceTable = table;
        foreach (var name in ifaceTable)
            names.Add(name);
        CollectionAssert.AreEqual(recordList.Select(r => r.FileName).ToList(), names);

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