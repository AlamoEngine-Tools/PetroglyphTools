using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PG.Commons;
using PG.StarWarsGame.Files.Binary;
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

    public DatBinaryConverterTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        PetroglyphCommons.ContributeServices(sc);
        _converter = new DatBinaryConverter(sc.BuildServiceProvider());
    }

    private static KeyTableRecord CreateKeyRecord(string key, string? alternateKey = null)
    {
        return new KeyTableRecord(key, alternateKey ?? key);
    }

    private static IndexTableRecord CreateIndexRecord(bool small, uint valueLength = 1)
    {
        return small
            ? new IndexTableRecord(DatTestData.SmallCrc, (uint)DatTestData.SmallCrcValue.Length, valueLength)
            : new IndexTableRecord(DatTestData.BigCrc, (uint)DatTestData.BigCrcValue.Length, valueLength);
    }

    private static DatStringEntry CreateEntry(bool small, string value)
    {
        return small
            ? new DatStringEntry(DatTestData.SmallCrcValue, DatTestData.SmallCrc, value)
            : new DatStringEntry(DatTestData.BigCrcValue, DatTestData.BigCrc, value);
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
            CreateIndexRecord(true),
            CreateIndexRecord(true),
            CreateIndexRecord(false)
        });

        var keyTable = new BinaryTable<KeyTableRecord>(new List<KeyTableRecord>
        {
           CreateKeyRecord(DatTestData.SmallCrcValue),
           CreateKeyRecord(DatTestData.SmallCrcValue),
           CreateKeyRecord(DatTestData.BigCrcValue)
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
        Assert.Equal(DatFileType.OrderedByCrc32, model.KeySortOrder);

        Assert.Equal([
            CreateEntry(true, "a"),
            CreateEntry(true, "b"),
            CreateEntry(false, "c"),
        ], model.ToList());
    }

    [Fact]
    public void Test_BinaryToModel_Unsorted()
    {
        var header = new DatHeader(3);
        var indexTable = new BinaryTable<IndexTableRecord>(new List<IndexTableRecord>
        {
            CreateIndexRecord(false),
            CreateIndexRecord(false),
            CreateIndexRecord(true),
        });

        var keyTable = new BinaryTable<KeyTableRecord>(new List<KeyTableRecord>
        {
            CreateKeyRecord(DatTestData.BigCrcValue),
            CreateKeyRecord(DatTestData.BigCrcValue),
            CreateKeyRecord(DatTestData.SmallCrcValue),
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
        Assert.Equal(DatFileType.NotOrdered, model.KeySortOrder);

        Assert.Equal([
            CreateEntry(false, "a"),
            CreateEntry(false, "b"),
            CreateEntry(true, "c"),
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
            CreateEntry(true, "abc"),
            CreateEntry(true, "d"),
            CreateEntry(false, new string('a', 260)),
        };

        var model = new Mock<IDatModel>();
        model.Setup(m => m.Count).Returns(3);
        model.Setup(m => m.GetEnumerator()).Returns(modelEntries.GetEnumerator());
        model.Setup(m => m.KeySortOrder).Returns(DatFileType.OrderedByCrc32);

        var binary = _converter.ModelToBinary(model.Object);

        Assert.Equal(model.Object.Count, binary.RecordNumber);
        Assert.Equal(model.Object.Count, (int)binary.Header.RecordCount);

        Assert.Equal([
            CreateIndexRecord(true, 3),
            CreateIndexRecord(true),
            CreateIndexRecord(false, 260),
        ],binary.IndexTable.ToList());

        Assert.Equal([
            CreateKeyRecord(DatTestData.SmallCrcValue),
            CreateKeyRecord(DatTestData.SmallCrcValue),
            CreateKeyRecord(DatTestData.BigCrcValue),
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
            CreateEntry(false, "abc"),
            CreateEntry(true, "d"),
        };

        var model = new Mock<IDatModel>();
        model.Setup(m => m.Count).Returns(2);
        model.Setup(m => m.KeySortOrder).Returns(DatFileType.NotOrdered);
        model.Setup(m => m.GetEnumerator()).Returns(modelEntries.GetEnumerator());

        var binary = _converter.ModelToBinary(model.Object);

        Assert.Equal(model.Object.Count, binary.RecordNumber);
        Assert.Equal(model.Object.Count, (int)binary.Header.RecordCount);

        Assert.Equal([
            CreateIndexRecord(false, 3),
            CreateIndexRecord(true),
        ], binary.IndexTable.ToList());

        Assert.Equal([
            CreateKeyRecord(DatTestData.BigCrcValue),
            CreateKeyRecord(DatTestData.SmallCrcValue),
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
            CreateEntry(false, "abc"),
            CreateEntry(true, "d"),
        };

        var model = new Mock<IDatModel>();
        model.Setup(m => m.Count).Returns(2);
        model.Setup(m => m.KeySortOrder).Returns(DatFileType.OrderedByCrc32);
        model.Setup(m => m.GetEnumerator()).Returns(modelEntries.GetEnumerator());

        Assert.Throws<ArgumentException>(() => _converter.ModelToBinary(model.Object));
    }
}