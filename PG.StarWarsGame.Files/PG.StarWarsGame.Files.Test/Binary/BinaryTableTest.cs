using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PG.StarWarsGame.Files.Binary;
using Xunit;

namespace PG.StarWarsGame.Files.Test.Binary;

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
    public void EmptyTable()
    {
        var table = CreateFileTable([]);
        Assert.Empty(table);
        Assert.Equal(0, table.Size);
        Assert.Equal([], table.Bytes);
    }

    [Fact]
    public void Size_1_Entry()
    {
        var entry = CreateFile(0, 1);
        var table = CreateFileTable(new List<T>
        {
            entry
        });
        Assert.Equal(entry.Size, table.Size);
    }

    [Fact]
    public void Size_2_Entries()
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
    public void Index()
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
    public void Enumerate()
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
    public void Enumerate_AsIEnumerable()
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
    public void Enumerate_ResetEnumerator()
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
    public void Bytes()
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

    [Fact]
    public void GetBytes_Empty()
    {
        var table = CreateFileTable([]);

        Span<byte> buffer = [1, 2, 3, 4];
        table.GetBytes(buffer);
        Assert.Equal([1, 2, 3, 4], buffer.ToArray());
    }

    [Fact]
    public void GetBytes_OneEntry()
    {
        var entry1 = CreateFile(0, 1);
        var expectedBytes = entry1.Bytes;

        var table = CreateFileTable([entry1]);

        Span<byte> buffer = new byte[entry1.Size + 10];
        buffer.Fill(1);

        table.GetBytes(buffer);

        Assert.Equal(expectedBytes, buffer.Slice(0, entry1.Size).ToArray());

        Span<byte> ones = new byte[10];
        ones.Fill(1);
        Assert.Equal(ones.ToArray(), buffer.Slice(entry1.Size).ToArray());
    }

    [Fact]
    public void GetBytes()
    {
        var numEntries = new Random().Next(2, 20);

        var entries = new List<T>(numEntries);
        var expectedBytes = new List<byte>();
        for (var i = 0; i < numEntries; i++)
        {
            var entry = CreateFile((uint)i, 1);
            entries.Add(entry);
            expectedBytes.AddRange(entry.Bytes);
        }

        var table = CreateFileTable(entries);

        Assert.Equal(entries.Sum(x => x.Size), table.Size);

        Span<byte> buffer = new byte[table.Size + 10];
        buffer.Fill(1);

        table.GetBytes(buffer);

        Assert.Equal(expectedBytes.ToArray(), buffer.Slice(0, table.Size).ToArray());

        Span<byte> ones = new byte[10];
        ones.Fill(1);
        Assert.Equal(ones.ToArray(), buffer.Slice(table.Size).ToArray());
    }

    [Fact]
    public void GetBytes_TooShortSpan_Throws()
    {
        var numEntries = new Random().Next(1, 20);

        var entries = new List<T>(numEntries);
        for (var i = 0; i < numEntries; i++)
        {
            var entry = CreateFile((uint)i, 1);
            entries.Add(entry);
        }

        var table = CreateFileTable(entries);

        Assert.Equal(entries.Sum(x => x.Size), table.Size);

        var buffer = new byte[table.Size - 1];
        buffer.AsSpan().Fill(1);

        var expected = (byte[])buffer.Clone();
        
        Assert.Throws<ArgumentException>(() => table.GetBytes(buffer));

        Assert.Equal(expected, buffer);
    }
}

public class TestClassBinary(byte[] bytes) : IBinary
{
    public byte[] Bytes { get; } = bytes;
    public int Size => Bytes.Length;
    public void GetBytes(Span<byte> bytes) => Bytes.CopyTo(bytes);
}

public readonly struct TestStructBinary(byte[] bytes) : IBinary
{
    public byte[] Bytes { get; } = bytes;
    public int Size => Bytes.Length;
    public void GetBytes(Span<byte> bytes) => Bytes.CopyTo(bytes);
}