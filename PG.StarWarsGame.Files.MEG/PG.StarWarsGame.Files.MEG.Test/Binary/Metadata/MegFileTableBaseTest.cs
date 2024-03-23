using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Metadata;

public abstract class MegFileTableBaseTest
{
    private protected abstract IMegFileTable CreateFileTable(IList<IMegFileDescriptor> files);

    private protected abstract IMegFileDescriptor CreateFile(uint index, uint seed);

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
}