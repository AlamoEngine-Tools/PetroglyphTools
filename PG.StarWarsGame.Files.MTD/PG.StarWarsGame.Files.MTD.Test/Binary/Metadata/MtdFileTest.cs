using System;
using System.Collections.Generic;
using System.Linq;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MTD.Binary.Metadata;
using Xunit;

namespace PG.StarWarsGame.Files.MTD.Test.Binary.Metadata;

public class MtdFileTest
{
    [Fact]
    public void Ctor_NullArgs_ThrowsArgumentNullException()
    { 
        Assert.Throws<ArgumentNullException>(() => new MtdBinaryFile(default, null!));
    }

    [Fact]
    public void Ctor_InvalidArgs_ThrowsArgumentException()
    {
        var header = new MtdHeader(1);

        Assert.Throws<ArgumentException>(() => new MtdBinaryFile(header, new BinaryTable<MtdBinaryFileInfo>(
            new List<MtdBinaryFileInfo>
            {
                new("a", 1, 1, 1, 1, true),
                new("b", 1, 1, 1, 1, true)
            })));

        Assert.Throws<ArgumentException>(() => new MtdBinaryFile(header, new BinaryTable<MtdBinaryFileInfo>(new List<MtdBinaryFileInfo>())));
    }


    [Fact]
    public void Ctor()
    {
        var header = new MtdHeader(2);

        _ = new MtdBinaryFile(header, new BinaryTable<MtdBinaryFileInfo>(new List<MtdBinaryFileInfo>
        {
            new("a", 1, 1, 1, 1, true),
            new("b", 1, 1, 1, 1, true)
        }));
        Assert.True(true);
    }

    [Fact]
    public void SizeBytes_WithContent()
    {
        var header = new MtdHeader(2);
        var fileTable = new BinaryTable<MtdBinaryFileInfo>(new List<MtdBinaryFileInfo>
        {
            new("a", 1, 1, 1, 1, true),
            new("b", 1, 1, 1, 1, true)
        });

        var file = new MtdBinaryFile(header, fileTable);

        var expectedBytes = header.Bytes
            .Concat(fileTable.Bytes)
            .ToArray();


        Assert.Equal(header.Size + fileTable.Size, file.Size);
        Assert.Equal(expectedBytes, file.Bytes);
    }

    [Fact]
    public void SizeBytes_Empty()
    {
        var header = new MtdHeader(0);
        var fileTable = new BinaryTable<MtdBinaryFileInfo>(new List<MtdBinaryFileInfo>());

        var file = new MtdBinaryFile(header, fileTable);

        var expectedBytes = header.Bytes.ToArray();

        Assert.Equal(header.Size, file.Size);
        Assert.Equal(expectedBytes, file.Bytes);
    }
}