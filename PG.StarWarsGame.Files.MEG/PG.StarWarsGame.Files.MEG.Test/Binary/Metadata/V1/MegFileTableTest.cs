using System;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Metadata.V1;

public class MegFileTableTest : MegFileTableBaseTest
{
    private static MegFileTable CreateFileTableV1(IList<MegFileTableRecord> files)
    {
        return new MegFileTable(files);
    }

    private protected override IMegFileTable CreateFileTable(IList<IMegFileDescriptor> files)
    {
        return CreateFileTableV1(files.Select(CreateFileRecordV1).ToList());
    }

    private static MegFileTableRecord CreateFileRecordV1(IMegFileDescriptor file)
    {
        return new MegFileTableRecord(file.Crc32, (uint)file.Index, file.FileSize, file.FileOffset, (uint)file.FileNameIndex);
    }

    private protected override IMegFileDescriptor CreateFile(uint index, uint seed)
    {
        return new MegFileTableRecord(new Crc32(seed), index, seed, seed, index);
    }

    [Fact]
    public void Ctor_NullArgs_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new MegFileTable(null!));
    }

    [Fact]
    public void Index()
    {
        var entry1 = CreateFileRecordV1(CreateFile(0, 1));
        var entry2 = CreateFileRecordV1(CreateFile(1, 2));
        var table = CreateFileTableV1(new List<MegFileTableRecord>
        {
            entry1,
            entry2
        });

        Assert.Equal(2, table.Count);
        Assert.Equal(entry1, table[0]);
        Assert.Equal(entry2, table[1]);
        Assert.Throws<ArgumentOutOfRangeException>(() => table[2]);
    }
}