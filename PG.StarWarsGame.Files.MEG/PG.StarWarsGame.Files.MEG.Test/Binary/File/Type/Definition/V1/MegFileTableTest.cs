// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Binary.Shared.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.File.Type.Definition.V1;

[TestClass]
public class MegFileTableTest
{
    [TestMethod]
    public void Ctor_Test__ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new MegFileTable(null!));
    }

    [TestMethod]
    public void Ctor_Test__ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => new MegFileTable(new List<MegFileContentTableRecord>()));
    }

    [TestMethod]
    public void Test_Size()
    {
        MegFileContentTableRecord entry = new(default, 0, default, default, default);
        var table = new MegFileTable(new List<MegFileContentTableRecord>
        {
            entry,
            entry
        });
        Assert.AreEqual(entry.Size * 2, table.Size);
    }

    [TestMethod]
    public void IFileNameTable_Test_Index()
    {
        MegFileContentTableRecord entry1 = new(default, 0, default, default, default);
        MegFileContentTableRecord entry2 = new(default, 1, default, default, default);
        var table = new MegFileTable(new List<MegFileContentTableRecord>
        {
            entry1,
            entry2
        });

        Assert.AreEqual(2, table.Count);
        Assert.AreEqual(entry1, table[0]);
        Assert.AreEqual(entry2, table[1]);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => table[2]);

        IFileTable ifaceTable = table;
        Assert.AreEqual(2, ifaceTable.Count);
        Assert.AreEqual(entry1, ifaceTable[0]);
        Assert.AreEqual(entry2, ifaceTable[1]);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ifaceTable[2]);
    }

    [TestMethod]
    public void IFileNameTable_Test_Enumerate()
    {
        MegFileContentTableRecord entry1 = new(default, 0, default, default, default);
        MegFileContentTableRecord entry2 = new(default, 1, default, default, default);

        var recordList = new List<MegFileContentTableRecord>
        {
            entry1,
            entry2
        };

        var table = new MegFileTable(recordList);

        var list = new List<MegFileContentTableRecord>();
        foreach (var record in table)
            list.Add(record);
        CollectionAssert.AreEqual(recordList, list);

        var names = new List<IMegFileDescriptor>();
        IFileTable ifaceTable = table;
        foreach (var name in ifaceTable)
            names.Add(name);
        CollectionAssert.AreEqual(recordList.ToList(), names);

    }

    [TestMethod]
    public void Test_Bytes()
    {
        MegFileContentTableRecord entry1 = new(default, 0, default, default, default);
        MegFileContentTableRecord entry2 = new(default, 1, default, default, default);
        var table = new MegFileTable(new List<MegFileContentTableRecord>
        {
            entry1,
            entry2
        });

        var expectedTableBytes = entry1.Bytes.Concat(entry2.Bytes).ToArray();
        CollectionAssert.AreEqual(expectedTableBytes, table.Bytes);
    }
}