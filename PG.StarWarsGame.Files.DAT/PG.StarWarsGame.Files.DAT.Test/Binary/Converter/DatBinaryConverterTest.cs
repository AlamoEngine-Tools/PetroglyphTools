using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Binary;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using Testably.Abstractions.Testing;

namespace PG.StarWarsGame.Files.DAT.Test.Binary.Converter;

[TestClass]
public class DatBinaryConverterTest
{
    private DatBinaryConverter _converter = null!;
    private Mock<ICrc32HashingService> _hashingService = null!;

    [TestInitialize]
    public void Setup()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        _hashingService = new Mock<ICrc32HashingService>();
        sc.AddSingleton(_hashingService.Object);
        _converter = new DatBinaryConverter(sc.BuildServiceProvider());
    }

    [TestMethod]
    public void Test_BinaryToModel_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _converter.BinaryToModel(null!));
    }

    [TestMethod]
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

        Assert.AreEqual(binary.RecordNumber, model.Count);
        Assert.AreEqual(DatFileType.OrderedByCrc32, model.KeySortOder);

        CollectionAssert.AreEqual(new List<DatStringEntry>
        {
            new("1", new Crc32(1), "a"),
            new("1", new Crc32(1), "b"),
            new("2", new Crc32(2), "c"),
        }, model.ToList());
    }

    [TestMethod]
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

        Assert.AreEqual(binary.RecordNumber, model.Count);
        Assert.AreEqual(DatFileType.NotOrdered, model.KeySortOder);

        CollectionAssert.AreEqual(new List<DatStringEntry>
        {
            new("2", new Crc32(2), "a"),
            new("2", new Crc32(2), "b"),
            new("1", new Crc32(1), "c"),
        }, model.ToList());
    }

    [TestMethod]
    public void Test_ModelToBinary_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _converter.ModelToBinary(null!));
    }

    [TestMethod]
    public void Test_ModelToBinary()
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

        _hashingService.Setup(h => h.GetCrc32("1", Encoding.ASCII)).Returns(new Crc32(1));
        _hashingService.Setup(h => h.GetCrc32("2", Encoding.ASCII)).Returns(new Crc32(2));

        var binary = _converter.ModelToBinary(model.Object);

        Assert.AreEqual(model.Object.Count, binary.RecordNumber);
        Assert.AreEqual(model.Object.Count, (int)binary.Header.RecordCount);

        CollectionAssert.AreEqual(new List<IndexTableRecord>
        {
            new(new Crc32(1), 1, 3),
            new(new Crc32(1), 1, 1),
            new(new Crc32(2), 1, 260),
        },binary.IndexTable.ToList());

        CollectionAssert.AreEqual(new List<KeyTableRecord>
        {
            new ("1", "1"),
            new ("1", "1"),
            new ("2", "2"),
        }, binary.KeyTable.ToList());

        CollectionAssert.AreEqual(new List<ValueTableRecord>
        {
            new ("abc"),
            new ("d"),
            new (new string('a', 260)),
        }, binary.ValueTable.ToList());
    }
}