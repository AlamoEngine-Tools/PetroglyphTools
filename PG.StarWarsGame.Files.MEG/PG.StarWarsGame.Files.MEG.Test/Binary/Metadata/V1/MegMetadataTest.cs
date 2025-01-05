using System;
using System.Collections.Generic;
using System.Linq;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Metadata.V1;

public class MegMetadataTest
{
    [Fact]
    public void Ctor_Test__ThrowsArgumentNullException()
    {
        var fileTable = new MegFileTable(new List<MegFileTableRecord>
            { new(default, 0, 0, 0, 0) });
        Assert.Throws<ArgumentNullException>(() => new MegMetadata(default, null!, fileTable));


        var fileNameTable = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord>
            { MegFileNameTableRecordTest.CreateNameRecord("123") });
        Assert.Throws<ArgumentNullException>(() =>
            new MegMetadata(default, fileNameTable, null!));
    }

    [Fact]
    public void Ctor_Test__ThrowsArgumentException()
    {
        var header1 = new MegHeader(1, 1);
        var header2 = new MegHeader(2, 2);
        var fileTable1 = new MegFileTable(new List<MegFileTableRecord>
            { new(default, 0, 0, 0, 0) });
        var fileNameTable1 = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord>
            { MegFileNameTableRecordTest.CreateNameRecord("123") });
        var fileNameTable2 = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord>
        {
            MegFileNameTableRecordTest.CreateNameRecord("123"),
            MegFileNameTableRecordTest.CreateNameRecord("456")
        });

        Assert.Throws<ArgumentException>(() => new MegMetadata(header2, fileNameTable1, fileTable1));
        Assert.Throws<ArgumentException>(() => new MegMetadata(header1, fileNameTable2, fileTable1));
    }


    [Fact]
    public void Ctor_Test__Correct()
    {
        new MegMetadata(
            new MegHeader(1, 1),
            new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord> { MegFileNameTableRecordTest.CreateNameRecord("123") }),
            new MegFileTable(new List<MegFileTableRecord> { default }));

        Assert.True(true);
    }

    [Fact]
    public void Test_SizeBytes_WithContent()
    {
        var header = new MegHeader(1, 1);
        var fileNameTable = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord> { MegFileNameTableRecordTest.CreateNameRecord("123") });
        var fileTable = new MegFileTable(new List<MegFileTableRecord> { default });

        var metadata = new MegMetadata(header, fileNameTable, fileTable);

        var expectedBytes = header.Bytes
            .Concat(fileNameTable.Bytes)
            .Concat(fileTable.Bytes)
            .ToArray();


        Assert.Equal(header.Size + fileNameTable.Size + fileTable.Size, metadata.Size);
        Assert.Equal(expectedBytes, metadata.Bytes);
    }

    [Fact]
    public void Test_SizeBytes_Empty()
    {
        var header = new MegHeader(0, 0);
        var fileNameTable = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord>());
        var fileTable = new MegFileTable(new List<MegFileTableRecord>());

        var metadata = new MegMetadata(header, fileNameTable, fileTable);

        var expectedBytes = header.Bytes.ToArray();

        Assert.Equal(header.Size, metadata.Size);
        Assert.Equal(expectedBytes, metadata.Bytes);
    }
}