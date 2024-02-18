using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Metadata;

public abstract class MegFileTableBaseTest
{
    private protected abstract IMegFileTable CreateFileTable(IList<IMegFileDescriptor> files);

    private protected abstract IMegFileDescriptor CreateFile(uint index, uint seed);

    [TestMethod]
    public void Test_EmptyTable()
    {
        var table = CreateFileTable(new List<IMegFileDescriptor>(0));
        Assert.AreEqual(0, table.Count);
        Assert.AreEqual(0, table.Size);
        CollectionAssert.AreEqual(new byte[] { }, table.Bytes);
    }

    [TestMethod]
    public void Test_Size_1_Entry()
    {
        var entry = CreateFile(0, 1);
        var table = CreateFileTable(new List<IMegFileDescriptor>
        {
            entry
        });
        Assert.AreEqual(entry.Size, table.Size);
    }

    [TestMethod]
    public void Test_Size_2_Entries()
    {
        var entry1 = CreateFile(0, 1);
        var entry2 = CreateFile(1, 2);
        var table = CreateFileTable(new List<IMegFileDescriptor>
        {
            entry1,
            entry2
        });
        Assert.AreEqual(entry1.Size + entry2.Size, table.Size);
    }

    [TestMethod]
    public void IFileNameTable_Test_Index()
    {
        var entry1 = CreateFile(0, 1);
        var entry2 = CreateFile(1, 2);
        var table = CreateFileTable(new List<IMegFileDescriptor>
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
        var entry1 = CreateFile(0, 1);
        var entry2 = CreateFile(1, 2);

        var recordList = new List<IMegFileDescriptor>
        {
            entry1,
            entry2
        };

        var table = CreateFileTable(recordList);

        var list = new List<IMegFileDescriptor>();
        foreach (var record in table)
            list.Add(record);
        CollectionAssert.AreEqual(recordList, list);
    }

    [TestMethod]
    public void Test_Enumerate_AsIEnumerable()
    {
        var entry1 = CreateFile(0, 1);
        var entry2 = CreateFile(1, 2);

        var recordList = new List<IMegFileDescriptor>
        {
            entry1,
            entry2
        };

        IEnumerable table = CreateFileTable(recordList);

        var list = new List<IMegFileDescriptor>();
        foreach (IMegFileDescriptor record in table)
            list.Add(record);
        CollectionAssert.AreEqual(recordList, list);
    }

    [TestMethod]
    public void Test_Enumerate_ResetEnumerator()
    {
        var entry1 = CreateFile(0, 1);
        var entry2 = CreateFile(1, 2);

        var recordList = new List<IMegFileDescriptor>
        {
            entry1,
            entry2
        };

        var table = CreateFileTable(recordList);

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
        var entry1 = CreateFile(0, 1);
        var entry2 = CreateFile(1, 2);
        var table = CreateFileTable(new List<IMegFileDescriptor>
        {
            entry1,
            entry2
        });

        var expectedTableBytes = entry1.Bytes.Concat(entry2.Bytes).ToArray();
        CollectionAssert.AreEqual(expectedTableBytes, table.Bytes);
    }
}