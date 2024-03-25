using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Binary;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services;
using Testably.Abstractions.Testing;

namespace PG.StarWarsGame.Files.DAT.Test.Services;

[TestClass]
public class DatFileServiceTest
{
    private DatFileService _service;
    private readonly MockFileSystem _fileSystem = new();
    private Mock<IDatBinaryConverter> _converter;
    private Mock<IDatFileReader> _reader;

    [TestInitialize]
    public void Setup()
    {
        _converter = new();
        _reader = new();
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        sc.AddSingleton(_converter.Object);
        sc.AddSingleton(_reader.Object);
        _service = new DatFileService(sc.BuildServiceProvider());
    }

    [TestMethod]
    public void Test_CreateDatFile_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            _service.CreateDatFile(null!, new List<DatStringEntry>(), DatFileType.NotOrdered));

        Assert.ThrowsException<ArgumentNullException>(() =>
            _service.CreateDatFile(_fileSystem.FileStream.New("test.dat", FileMode.Create), null!, DatFileType.NotOrdered));
    }

    [TestMethod]
    public void Test_CreateDatFile_PreserveOrder()
    {
        var binary = CreateUnsortedBinary();

        var inputEntries = new List<DatStringEntry>
        {
            new("2", new Crc32(2), "value2"),
            new("2", new Crc32(2), "value22"),
            new("1", new Crc32(1), "value1"),
        };

        _fileSystem.Initialize();

        _converter.Setup(c => c.ModelToBinary(It.IsAny<IDatModel>()))
            .Callback((IDatModel m) =>
            {
                var entries = m.ToList();
                Assert.AreEqual(3, m.Count);
                CollectionAssert.AreEqual(inputEntries, entries);
            })
            .Returns(binary);

        var fs = _fileSystem.FileStream.New("test.dat", FileMode.Create);
        _service.CreateDatFile(fs, inputEntries, DatFileType.NotOrdered);
        fs.Dispose();

        CollectionAssert.AreEqual(binary.Bytes, _fileSystem.File.ReadAllBytes("test.dat"));

        _converter.Verify(c => c.ModelToBinary(It.IsAny<IDatModel>()), Times.Once);
    }

    [TestMethod]
    public void Test_CreateDatFile_SortEntries()
    {
        var binary = CreateUnsortedBinary();

        var inputEntries = new List<DatStringEntry>
        {
            new("2", new Crc32(2), "value2"),
            new("2", new Crc32(2), "value22"),
            new("1", new Crc32(1), "value1"),
        };

        _fileSystem.Initialize();

        _converter.Setup(c => c.ModelToBinary(It.IsAny<IDatModel>()))
            .Callback((IDatModel m) =>
            {
                var entries = m.ToList();
                Assert.AreEqual(3, m.Count);
                CollectionAssert.AreEqual(new List<DatStringEntry>
                {
                    new("1", new Crc32(1), "value1"),
                    new("2", new Crc32(2), "value2"),
                    new("2", new Crc32(2), "value22"),
                }, entries);
            })
            .Returns(binary);

        var fs = _fileSystem.FileStream.New("test.dat", FileMode.Create);
        _service.CreateDatFile(fs, inputEntries, DatFileType.OrderedByCrc32);
        fs.Dispose();

        CollectionAssert.AreEqual(binary.Bytes, _fileSystem.File.ReadAllBytes("test.dat"));

        _converter.Verify(c => c.ModelToBinary(It.IsAny<IDatModel>()), Times.Once);
    }

    [DataTestMethod]
    [DataRow(DatFileType.NotOrdered)]
    [DataRow(DatFileType.OrderedByCrc32)]
    public void Test_GetDatFileType(DatFileType expected)
    {
        _fileSystem.Initialize().WithFile("test.dat").Which(a => a.HasBytesContent([0x1]));

        _reader.Setup(r => r.PeekFileType(It.IsAny<Stream>()))
            .Callback((Stream s) =>
            {
                Assert.AreEqual(1, s.Length);
            })
            .Returns(expected);
        
        var sorted = _service.GetDatFileType("test.dat");

        Assert.AreEqual(expected, sorted);
        _reader.Verify(r => r.PeekFileType(It.IsAny<Stream>()), Times.Once);
    }

    [TestMethod]
    public void Test_Load()
    {
        var sortedModel = new Mock<IDatModel>();
        sortedModel.SetupGet(m => m.KeySortOder).Returns(DatFileType.OrderedByCrc32);

        var unsortedModel = new Mock<IDatModel>();
        sortedModel.SetupGet(m => m.KeySortOder).Returns(DatFileType.NotOrdered);

        var sortedBinary = CreateSortedBinary();
        var unsortedBinary = CreateUnsortedBinary();

        _fileSystem.Initialize()
            .WithFile("sorted.dat").Which(a => a.HasBytesContent(sortedBinary.Bytes))
            .WithFile("unsorted.dat").Which(a => a.HasBytesContent(unsortedBinary.Bytes));

        _reader.SetupSequence(r => r.ReadBinary(It.IsAny<Stream>()))
            .Returns(sortedBinary)
            .Returns(unsortedBinary);

        _reader.SetupSequence(r => r.PeekFileType(It.IsAny<Stream>()))
            .Returns(DatFileType.OrderedByCrc32)
            .Returns(DatFileType.NotOrdered);

        _converter.Setup(c => c.BinaryToModel(sortedBinary))
            .Returns(sortedModel.Object);
        _converter.Setup(c => c.BinaryToModel(unsortedBinary))
            .Returns(unsortedModel.Object);

        var sortedFileHolder =_service.Load("sorted.dat");
        Assert.AreEqual(_fileSystem.Path.GetFullPath("sorted.dat") , sortedFileHolder.FilePath);
        Assert.AreEqual(_fileSystem.Path.GetFullPath("sorted.dat") , sortedFileHolder.FileInformation.FilePath);
        Assert.AreSame(sortedModel.Object, sortedFileHolder.Content);

        var unsortedFileHolder = _service.Load("unsorted.dat");
        Assert.AreEqual(_fileSystem.Path.GetFullPath("unsorted.dat"), unsortedFileHolder.FilePath);
        Assert.AreEqual(_fileSystem.Path.GetFullPath("unsorted.dat"), unsortedFileHolder.FileInformation.FilePath);
        Assert.AreSame(unsortedModel.Object, unsortedFileHolder.Content);
    }

    [TestMethod]
    public void Test_LoadAs_SortedAsUnsorted()
    {
        var unsortedModel = new Mock<IUnsortedDatModel>();
        unsortedModel.SetupGet(m => m.KeySortOder).Returns(DatFileType.NotOrdered);

        var sortedModel = new Mock<ISortedDatModel>();
        sortedModel.SetupGet(m => m.KeySortOder).Returns(DatFileType.OrderedByCrc32);
        sortedModel.Setup(s => s.ToUnsortedModel()).Returns(unsortedModel.Object);

      
        var sortedBinary = CreateSortedBinary();

        _fileSystem.Initialize()
            .WithFile("sorted.dat").Which(a => a.HasBytesContent(sortedBinary.Bytes));

        _reader.Setup(r => r.ReadBinary(It.IsAny<Stream>()))
            .Returns(sortedBinary);

        _converter.Setup(c => c.BinaryToModel(sortedBinary))
            .Returns(sortedModel.Object);

        var unsortedFileHolder = _service.LoadAs("sorted.dat", DatFileType.NotOrdered);

        Assert.AreEqual(_fileSystem.Path.GetFullPath("sorted.dat"), unsortedFileHolder.FilePath);
        Assert.AreEqual(_fileSystem.Path.GetFullPath("sorted.dat"), unsortedFileHolder.FileInformation.FilePath);
        Assert.AreSame(unsortedModel.Object, unsortedFileHolder.Content);
    }

    [TestMethod]
    public void Test_LoadAs_UnsortedAsSorted_Throws()
    {
        var unsortedModel = new Mock<IUnsortedDatModel>();
        unsortedModel.SetupGet(m => m.KeySortOder).Returns(DatFileType.NotOrdered);

        var unsortedBinary = CreateUnsortedBinary();

        _fileSystem.Initialize()
            .WithFile("unsorted.dat").Which(a => a.HasBytesContent(unsortedBinary.Bytes));

        _reader.Setup(r => r.ReadBinary(It.IsAny<Stream>()))
            .Returns(unsortedBinary);

        _converter.Setup(c => c.BinaryToModel(unsortedBinary))
            .Returns(unsortedModel.Object);

        Assert.ThrowsException<NotSupportedException>(() =>
            _service.LoadAs("unsorted.dat", DatFileType.OrderedByCrc32));
    }


    private static DatBinaryFile CreateUnsortedBinary()
    {
        return new DatBinaryFile(new DatHeader(2),
            new BinaryTable<IndexTableRecord>(new List<IndexTableRecord>
            {
                new(new Crc32(2), 1, 1),
                new(new Crc32(1), 1, 1)
            }),
            new BinaryTable<ValueTableRecord>(new List<ValueTableRecord>
            {
                new("b"),
                new("a"),
            }),
            new BinaryTable<KeyTableRecord>(new List<KeyTableRecord>
            {
                new("2", "2"),
                new("1", "1"),
            }));
    }

    private static DatBinaryFile CreateSortedBinary()
    {
        return new DatBinaryFile(new DatHeader(1),
            new BinaryTable<IndexTableRecord>(new List<IndexTableRecord>
            {
                new(new Crc32(1), 1, 1),
                new(new Crc32(2), 1, 1)
            }),
            new BinaryTable<ValueTableRecord>(new List<ValueTableRecord>
            {
                new("a"),
                new("b"),
            }),
            new BinaryTable<KeyTableRecord>(new List<KeyTableRecord>
            {
                new("1", "1"),
                new("2", "2")
            }));
    }
}