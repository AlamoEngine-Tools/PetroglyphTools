// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Binary.Shared.Metadata;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.File.Type.Definition;

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
        MegFileNameTableRecord entry = new("abc", Encoding.ASCII);
        var table = new MegFileNameTable(new List<MegFileNameTableRecord>
        {
            entry,
            entry
        });
        Assert.AreEqual(entry.Size * 2, table.Size);
    }

    [TestMethod]
    public void IFileNameTable_Test_Enumerate()
    {
        MegFileNameTableRecord entry1 = new("123", Encoding.ASCII);
        MegFileNameTableRecord entry2 = new("456", Encoding.ASCII);
        IFileNameTable table = new MegFileNameTable(new List<MegFileNameTableRecord>
        {
            entry1,
            entry2
        });

        Assert.AreEqual("123", table[0]);
        Assert.AreEqual("456", table[1]);
    }

    [TestMethod]
    public void Test_Bytes()
    {
        MegFileNameTableRecord entry1 = new("a", Encoding.ASCII);
        MegFileNameTableRecord entry2 = new("b", Encoding.ASCII);
        var table = new MegFileNameTable(new List<MegFileNameTableRecord>
        {
            entry1,
            entry2
        });

        var expectedTableBytes = entry1.Bytes.Concat(entry2.Bytes).ToArray();
        CollectionAssert.AreEqual(expectedTableBytes, table.Bytes);
    }
}