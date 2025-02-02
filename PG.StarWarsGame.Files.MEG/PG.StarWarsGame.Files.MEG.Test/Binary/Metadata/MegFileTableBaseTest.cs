using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Metadata;

public abstract class MegFileTableBaseTest
{
    private protected abstract IMegFileTable CreateFileTable(IList<IMegFileDescriptor> files);

    private protected abstract IMegFileDescriptor CreateFile(uint index, uint seed);

    [Fact]
    public void Enumerate()
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
        Assert.Equal(recordList, list);
    }

    [Fact]
    public void Enumerate_ResetEnumerator()
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
        Assert.Equal(table[0], enumerator.Current);

        enumerator.Reset();

        enumerator.MoveNext();
        Assert.Equal(table[0], enumerator.Current);
    }
}