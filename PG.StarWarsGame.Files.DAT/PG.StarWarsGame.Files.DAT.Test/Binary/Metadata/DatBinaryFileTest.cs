using System;
using System.Collections.Generic;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Binary.Metadata;

public class DatBinaryFileTest
{
    [Fact]
    public void Ctor_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new DatBinaryFile(default, null!, new BinaryTable<ValueTableRecord>([]), new BinaryTable<KeyTableRecord>([])));
        Assert.Throws<ArgumentNullException>(() => new DatBinaryFile(default, new BinaryTable<IndexTableRecord>([]), null!, new BinaryTable<KeyTableRecord>([])));
        Assert.Throws<ArgumentNullException>(() => new DatBinaryFile(default, new BinaryTable<IndexTableRecord>([]), new BinaryTable<ValueTableRecord>([]), null!));
    }

    [Fact]
    public void Ctor_Empty()
    {
        DatHeader header = default;
        var index = new BinaryTable<IndexTableRecord>([]);
        var values = new BinaryTable<ValueTableRecord>([]);
        var keys = new BinaryTable<KeyTableRecord>([]);

        var dat = new DatBinaryFile(header, index, values, keys);

        Assert.Equal(0, dat.RecordNumber);
        Assert.Same(index, dat.IndexTable);
        Assert.Same(keys, dat.KeyTable);
        Assert.Same(values, dat.ValueTable);
        Assert.Equal(4, dat.Size);
        Assert.Equal(BitConverter.GetBytes(dat.RecordNumber), dat.Bytes);
    }

    [Fact]
    public void Ctor()
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
            new("Key1", "Key1"),
            new("Key2", "Key2")
        });

        var dat = new DatBinaryFile(header, index, values, keys);

        Assert.Equal(2, dat.RecordNumber);
        Assert.Same(index, dat.IndexTable);
        Assert.Same(keys, dat.KeyTable);
        Assert.Same(values, dat.ValueTable);
        Assert.Equal(header.Size + index.Size + values.Size + keys.Size, dat.Size);

        var bytes = new List<byte>();
        bytes.AddRange(header.Bytes);
        bytes.AddRange(index.Bytes);
        bytes.AddRange(values.Bytes);
        bytes.AddRange(keys.Bytes);

        Assert.Equal(bytes, dat.Bytes);
    }
}