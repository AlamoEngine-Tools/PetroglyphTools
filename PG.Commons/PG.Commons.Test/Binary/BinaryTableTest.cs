using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.Binary;
using Xunit;

namespace PG.Commons.Test.Binary;

public class BinaryTableClassTest : BinaryTableTest<TestClassBinary>
{
    protected override TestClassBinary CreateFile(uint index, uint seed)
    {
        var random = new Random();
        var size = (seed + 1) * 2;
        var bytes = new byte[size];
        random.NextBytes(bytes);
        return new TestClassBinary(bytes);
    }
}

public class BinaryTableStructTest : BinaryTableTest<TestStructBinary>
{
    protected override TestStructBinary CreateFile(uint index, uint seed)
    {
        var random = new Random();
        var size = (seed + 1) * 2;
        var bytes = new byte[size];
        random.NextBytes(bytes);
        return new TestStructBinary(bytes);
    }
}

public abstract class BinaryTableTest<T> where T : IBinary
{
    private static BinaryTable<T> CreateFileTable(IList<T> items)
    {
        return new BinaryTable<T>(items);
    }

    protected abstract T CreateFile(uint index, uint seed);

    [Fact]
    public void Test_EmptyTable()
    {
        var table = CreateFileTable(new List<T>(0));
        Assert.Empty(table);
        Assert.Equal(0, table.Size);
        Assert.Equal([], table.Bytes);
    }

    [Fact]
    public void Test_Size_1_Entry()
    {
        var entry = CreateFile(0, 1);
        var table = CreateFileTable(new List<T>
        {
            entry
        });
        Assert.Equal(entry.Size, table.Size);
    }

    [Fact]
    public void Test_Size_2_Entries()
    {
        var entry1 = CreateFile(0, 1);
        var entry2 = CreateFile(1, 2);
        var table = CreateFileTable(new List<T>
        {
            entry1,
            entry2
        });
        Assert.Equal(entry1.Size + entry2.Size, table.Size);
    }

    [Fact]
    public void IFileNameTable_Test_Index()
    {
        var entry1 = CreateFile(0, 1);
        var entry2 = CreateFile(1, 2);
        var table = CreateFileTable(new List<T>
        {
            entry1,
            entry2
        });

        Assert.Equal(2, table.Count);
        Assert.Equal(entry1, table[0]);
        Assert.Equal(entry2, table[1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => table[2]);
    }

    [Fact]
    public void Test_Enumerate()
    {
        var entry1 = CreateFile(0, 1);
        var entry2 = CreateFile(1, 2);

        var recordList = new List<T>
        {
            entry1,
            entry2
        };

        var table = CreateFileTable(recordList);

        var list = new List<T>();
        foreach (var record in table)
            list.Add(record);
        Assert.Equal(recordList, list);
    }

    [Fact]
    public void Test_Enumerate_AsIEnumerable()
    {
        var entry1 = CreateFile(0, 1);
        var entry2 = CreateFile(1, 2);

        var recordList = new List<T>
        {
            entry1,
            entry2
        };

        IEnumerable table = CreateFileTable(recordList);

        var list = new List<T>();
        foreach (T record in table)
            list.Add(record);
        Assert.Equal(recordList, list);
    }

    [Fact]
    public void Test_Enumerate_ResetEnumerator()
    {
        var entry1 = CreateFile(0, 1);
        var entry2 = CreateFile(1, 2);

        var recordList = new List<T>
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

    [Fact]
    public void Test_Bytes()
    {
        var entry1 = CreateFile(0, 1);
        var entry2 = CreateFile(1, 2);
        var table = CreateFileTable(new List<T>
        {
            entry1,
            entry2
        });

        var expectedTableBytes = entry1.Bytes.Concat(entry2.Bytes).ToArray();
        Assert.Equal(expectedTableBytes, table.Bytes);
    }
}

public class TestClassBinary(byte[] bytes) : IBinary
{
    public byte[] Bytes { get; } = bytes;
    public int Size => Bytes.Length;
}

public readonly struct TestStructBinary(byte[] bytes) : IBinary
{
    public byte[] Bytes { get; } = bytes;
    public int Size => Bytes.Length;
}