using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Binary;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;

namespace PG.StarWarsGame.Files.DAT.Test.Binary.Metadata;

[TestClass]
public class DatBinaryFileTest
{
    [TestMethod]
    public void Test_Ctor_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new DatBinaryFile(default, null!, new BinaryTable<ValueTableRecord>([]), new BinaryTable<KeyTableRecord>([])));
        Assert.ThrowsException<ArgumentNullException>(() => new DatBinaryFile(default, new BinaryTable<IndexTableRecord>([]), null!, new BinaryTable<KeyTableRecord>([])));
        Assert.ThrowsException<ArgumentNullException>(() => new DatBinaryFile(default, new BinaryTable<IndexTableRecord>([]), new BinaryTable<ValueTableRecord>([]), null!));
    }

    [TestMethod]
    public void Test_Ctor_Empty()
    {
        DatHeader header = default;
        var index = new BinaryTable<IndexTableRecord>([]);
        var values = new BinaryTable<ValueTableRecord>([]);
        var keys = new BinaryTable<KeyTableRecord>([]);

        var dat = new DatBinaryFile(header, index, values, keys);

        Assert.AreEqual(0, dat.RecordNumber);
        Assert.AreSame(index, dat.IndexTable);
        Assert.AreSame(keys, dat.KeyTable);
        Assert.AreSame(values, dat.ValueTable);
        Assert.AreEqual(4, dat.Size);
        CollectionAssert.AreEqual(BitConverter.GetBytes(dat.RecordNumber), dat.Bytes);
    }

    [TestMethod]
    public void Test_Ctor()
    {
        var header = new DatHeader(2);
        var index = new BinaryTable<IndexTableRecord>(new List<IndexTableRecord>
        {
            new(new Crc32(0), 1, 2),
            new(new Crc32(1), 1, 2),
        });
        var values = new BinaryTable<ValueTableRecord>(new List<ValueTableRecord>
        {
            new("Value1"),
            new("Value2")
        });
        var keys = new BinaryTable<KeyTableRecord>(new List<KeyTableRecord>
        {
            new("Key1"),
            new("Key2")
        });

        var dat = new DatBinaryFile(header, index, values, keys);

        Assert.AreEqual(2, dat.RecordNumber);
        Assert.AreSame(index, dat.IndexTable);
        Assert.AreSame(keys, dat.KeyTable);
        Assert.AreSame(values, dat.ValueTable);
        Assert.AreEqual(header.Size + index.Size + values.Size + keys.Size, dat.Size);

        var bytes = new List<byte>();
        bytes.AddRange(header.Bytes);
        bytes.AddRange(index.Bytes);
        bytes.AddRange(values.Bytes);
        bytes.AddRange(keys.Bytes);

        CollectionAssert.AreEqual(bytes, dat.Bytes);
    }
}