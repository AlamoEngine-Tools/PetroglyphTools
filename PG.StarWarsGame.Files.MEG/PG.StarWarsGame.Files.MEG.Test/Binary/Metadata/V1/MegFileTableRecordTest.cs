using System;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Metadata.V1;

public class MegFileTableRecordTest
{
    [Fact]
    public void Ctor_InvalidArgs_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MegFileTableRecord(
                new Crc32(0),
                int.MaxValue + 1u,
                0,
                0,
                0));

        Assert.Throws<ArgumentOutOfRangeException>(() => new MegFileTableRecord(
            new Crc32(0),
            0,
            0,
            0,
            int.MaxValue + 1u));
    }

    [Fact]
    public void Ctor()
    {
        var record = new MegFileTableRecord(new Crc32(0), 1, 2, 3, 4);
        Assert.Equal(0, (int)record.Crc32);
        Assert.Equal(1, (int)record.FileTableRecordIndex);
        Assert.Equal(2, (int)record.FileSize);
        Assert.Equal(3, (int)record.FileOffset);
        Assert.Equal(4, (int)record.FileNameIndex);
    }

    [Fact]
    public void Comparison()
    {
        var r0 = new MegFileTableRecord(new Crc32(0), 1, 1, 1, 1);
        var r1 = new MegFileTableRecord(new Crc32(1), 0, 0, 0, 0);

        Assert.True(r0 < r1);
        Assert.False(r0 > r1);
        Assert.True(r1 > r0);
        Assert.False(r1 < r0);

#pragma warning disable CS1718
        Assert.True(r1 <= r1);
        Assert.True(r1 >= r1);
#pragma warning restore CS1718

        Assert.Equal(0, r0.CompareTo(r0));
        Assert.Equal(-1, r0.CompareTo(r1));
        Assert.Equal(1, r1.CompareTo(r0));

        Assert.Equal(1, ((IComparable<IMegFileDescriptor>)r1).CompareTo(null!));
        Assert.Equal(0, ((IComparable<IMegFileDescriptor>)r1).CompareTo(r1));
        Assert.Equal(1, ((IComparable<IMegFileDescriptor>)r1).CompareTo(r0));
        Assert.Equal(-1, ((IComparable<IMegFileDescriptor>)r0).CompareTo(r1));
    }

    [Fact]
    public void Bytes()
    {
        var record = new MegFileTableRecord(new Crc32(1), 2, 3, 4, 5);

        var bytes = new byte[]
        {
            0x1, 0x0, 0x0, 0x0,
            0x2, 0x0, 0x0, 0x0,
            0x3, 0x0, 0x0, 0x0,
            0x4, 0x0, 0x0, 0x0,
            0x5, 0x0, 0x0, 0x0,
        };
        Assert.Equal(bytes, record.Bytes);
    }
}