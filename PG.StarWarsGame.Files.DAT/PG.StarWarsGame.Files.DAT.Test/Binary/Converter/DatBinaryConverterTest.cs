using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PG.Commons.Binary;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Binary.Converter;

public class DatBinaryConverterTest
{
    private readonly DatBinaryConverter _converter;
    private readonly Mock<ICrc32HashingService> _hashingService;

    public DatBinaryConverterTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        _hashingService = new Mock<ICrc32HashingService>();
        sc.AddSingleton(_hashingService.Object);
        _converter = new DatBinaryConverter(sc.BuildServiceProvider());
    }

    [Fact]
    public void Test_BinaryToModel_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _converter.BinaryToModel(null!));
    }

    [Fact]
    public void Test_BinaryToModel_Sorted()
    {
        var header = new DatHeader(3);
        var indexTable = new BinaryTable<IndexTableRecord>(new List<IndexTableRecord>
        {
            new(new Crc32(1), 1, 1),
            new(new Crc32(1), 1, 1),
            new(new Crc32(2), 1, 1),
        });

        var keyTable = new BinaryTable<KeyTableRecord>(new List<KeyTableRecord>
        {
            new("1", "1"),
            new("1", "1"),
            new("2", "2"),
        });
        var valueTable = new BinaryTable<ValueTableRecord>(new List<ValueTableRecord>
        {
            new("a"),
            new("b"),
            new("c"),
        });
        var binary = new DatBinaryFile(header, indexTable, valueTable, keyTable);

        var model = _converter.BinaryToModel(binary);

        Assert.Equal(binary.RecordNumber, model.Count);
        Assert.Equal(DatFileType.OrderedByCrc32, model.KeySortOder);

        Assert.Equal([
            new("1", new Crc32(1), "a"),
            new("1", new Crc32(1), "b"),
            new("2", new Crc32(2), "c")
        ], model.ToList());
    }

    [Fact]
    public void Test_BinaryToModel_Unsorted()
    {
        var header = new DatHeader(3);
        var indexTable = new BinaryTable<IndexTableRecord>(new List<IndexTableRecord>
        {
            new(new Crc32(2), 1, 1),
            new(new Crc32(2), 1, 1),
            new(new Crc32(1), 1, 1),
        });

        var keyTable = new BinaryTable<KeyTableRecord>(new List<KeyTableRecord>
        {
            new("2", "2"),
            new("2", "2"),
            new("1", "1"),
        });
        var valueTable = new BinaryTable<ValueTableRecord>(new List<ValueTableRecord>
        {
            new("a"),
            new("b"),
            new("c"),
        });
        var binary = new DatBinaryFile(header, indexTable, valueTable, keyTable);

        var model = _converter.BinaryToModel(binary);

        Assert.Equal(binary.RecordNumber, model.Count);
        Assert.Equal(DatFileType.NotOrdered, model.KeySortOder);

        Assert.Equal([
            new("2", new Crc32(2), "a"),
            new("2", new Crc32(2), "b"),
            new("1", new Crc32(1), "c")
        ], model.ToList());
    }

    [Fact]
    public void Test_ModelToBinary_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _converter.ModelToBinary(null!));
    }

    [Fact]
    public void Test_ModelToBinary_Ordered()
    {
        var modelEntries = new List<DatStringEntry>
        {
            new("1", new Crc32(0), "abc"),
            new("1", new Crc32(0), "d"),
            new("2", new Crc32(0), new string('a', 260)),
        };

        var model = new Mock<IDatModel>();
        model.Setup(m => m.Count).Returns(3);
        model.Setup(m => m.GetEnumerator()).Returns(modelEntries.GetEnumerator());
        model.Setup(m => m.KeySortOder).Returns(DatFileType.OrderedByCrc32);

        _hashingService.Setup(h => h.GetCrc32("1", Encoding.ASCII)).Returns(new Crc32(1));
        _hashingService.Setup(h => h.GetCrc32("2", Encoding.ASCII)).Returns(new Crc32(2));

        var binary = _converter.ModelToBinary(model.Object);

        Assert.Equal(model.Object.Count, binary.RecordNumber);
        Assert.Equal(model.Object.Count, (int)binary.Header.RecordCount);

        Assert.Equal([
            new(new Crc32(1), 1, 3),
            new(new Crc32(1), 1, 1),
            new(new Crc32(2), 1, 260)
        ],binary.IndexTable.ToList());

        Assert.Equal([
            new("1", "1"),
            new("1", "1"),
            new("2", "2")
        ], binary.KeyTable.ToList());

        Assert.Equal([
            new("abc"),
            new("d"),
            new(new string('a', 260))
        ], binary.ValueTable.ToList());
    }

    [Fact]
    public void Test_ModelToBinary_Unsorted()
    {
        var modelEntries = new List<DatStringEntry>
        {
            new("2", new Crc32(0), "abc"),
            new("1", new Crc32(0), "d"),
        };

        var model = new Mock<IDatModel>();
        model.Setup(m => m.Count).Returns(2);
        model.Setup(m => m.KeySortOder).Returns(DatFileType.NotOrdered);
        model.Setup(m => m.GetEnumerator()).Returns(modelEntries.GetEnumerator());

        _hashingService.Setup(h => h.GetCrc32("1", Encoding.ASCII)).Returns(new Crc32(1));
        _hashingService.Setup(h => h.GetCrc32("2", Encoding.ASCII)).Returns(new Crc32(2));

        var binary = _converter.ModelToBinary(model.Object);

        Assert.Equal(model.Object.Count, binary.RecordNumber);
        Assert.Equal(model.Object.Count, (int)binary.Header.RecordCount);

        Assert.Equal([
            new(new Crc32(2), 1, 3),
            new(new Crc32(1), 1, 1)
        ], binary.IndexTable.ToList());

        Assert.Equal([
            new("2", "2"),
            new("1", "1")
        ], binary.KeyTable.ToList());

        Assert.Equal([
            new("abc"),
            new("d")
        ], binary.ValueTable.ToList());
    }

    [Fact]
    public void Test_ModelToBinary_WantsSortedButIsNot_ThrowsArgumentException()
    {
        var modelEntries = new List<DatStringEntry>
        {
            new("2", new Crc32(0), "abc"),
            new("1", new Crc32(0), "d"),
        };

        var model = new Mock<IDatModel>();
        model.Setup(m => m.Count).Returns(2);
        model.Setup(m => m.KeySortOder).Returns(DatFileType.OrderedByCrc32);
        model.Setup(m => m.GetEnumerator()).Returns(modelEntries.GetEnumerator());

        _hashingService.Setup(h => h.GetCrc32("1", Encoding.ASCII)).Returns(new Crc32(1));
        _hashingService.Setup(h => h.GetCrc32("2", Encoding.ASCII)).Returns(new Crc32(2));

        Assert.Throws<ArgumentException>(() => _converter.ModelToBinary(model.Object));
    }
}