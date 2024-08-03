using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PG.Commons.Binary;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services;

public class DatFileServiceTest
{
    private readonly DatFileService _service;
    private readonly MockFileSystem _fileSystem = new();
    private readonly Mock<IDatBinaryConverter> _converter;
    private readonly Mock<IDatFileReader> _reader;

    public DatFileServiceTest()
    {
        _converter = new();
        _reader = new();
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        sc.AddSingleton(_converter.Object);
        sc.AddSingleton(_reader.Object);
        _service = new DatFileService(sc.BuildServiceProvider());
    }

    [Fact]
    public void Test_CreateDatFile_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _service.CreateDatFile(null!, new List<DatStringEntry>(), DatFileType.NotOrdered));

        Assert.Throws<ArgumentNullException>(() =>
            _service.CreateDatFile(_fileSystem.FileStream.New("test.dat", FileMode.Create), null!, DatFileType.NotOrdered));
    }

    [Fact]
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
                Assert.Equal(3, m.Count);
                Assert.Equal(inputEntries, entries);
            })
            .Returns(binary);

        var fs = _fileSystem.FileStream.New("test.dat", FileMode.Create);
        _service.CreateDatFile(fs, inputEntries, DatFileType.NotOrdered);
        fs.Dispose();

        Assert.Equal(binary.Bytes, _fileSystem.File.ReadAllBytes("test.dat"));

        _converter.Verify(c => c.ModelToBinary(It.IsAny<IDatModel>()), Times.Once);
    }

    [Fact]
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
                Assert.Equal(3, m.Count);
                Assert.Equal([
                    new("1", new Crc32(1), "value1"),
                    new("2", new Crc32(2), "value2"),
                    new("2", new Crc32(2), "value22")
                ], entries);
            })
            .Returns(binary);

        var fs = _fileSystem.FileStream.New("test.dat", FileMode.Create);
        _service.CreateDatFile(fs, inputEntries, DatFileType.OrderedByCrc32);
        fs.Dispose();

        Assert.Equal(binary.Bytes, _fileSystem.File.ReadAllBytes("test.dat"));

        _converter.Verify(c => c.ModelToBinary(It.IsAny<IDatModel>()), Times.Once);
    }

    [Theory]
    [InlineData(DatFileType.NotOrdered)]
    [InlineData(DatFileType.OrderedByCrc32)]
    public void Test_GetDatFileType(DatFileType expected)
    {
        _fileSystem.Initialize().WithFile("test.dat").Which(a => a.HasBytesContent([0x1]));

        _reader.Setup(r => r.PeekFileType(It.IsAny<Stream>()))
            .Callback((Stream s) =>
            {
                Assert.Equal(1, s.Length);
            })
            .Returns(expected);
        
        var sorted = _service.GetDatFileType("test.dat");

        Assert.Equal(expected, sorted);
        _reader.Verify(r => r.PeekFileType(It.IsAny<Stream>()), Times.Once);
    }

    [Fact]
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
        Assert.Equal(_fileSystem.Path.GetFullPath("sorted.dat") , sortedFileHolder.FilePath);
        Assert.Equal(_fileSystem.Path.GetFullPath("sorted.dat") , sortedFileHolder.FileInformation.FilePath);
        Assert.Same(sortedModel.Object, sortedFileHolder.Content);

        var unsortedFileHolder = _service.Load("unsorted.dat");
        Assert.Equal(_fileSystem.Path.GetFullPath("unsorted.dat"), unsortedFileHolder.FilePath);
        Assert.Equal(_fileSystem.Path.GetFullPath("unsorted.dat"), unsortedFileHolder.FileInformation.FilePath);
        Assert.Same(unsortedModel.Object, unsortedFileHolder.Content);
    }

    [Fact]
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

        Assert.Equal(_fileSystem.Path.GetFullPath("sorted.dat"), unsortedFileHolder.FilePath);
        Assert.Equal(_fileSystem.Path.GetFullPath("sorted.dat"), unsortedFileHolder.FileInformation.FilePath);
        Assert.Same(unsortedModel.Object, unsortedFileHolder.Content);
    }

    [Fact]
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

        Assert.Throws<NotSupportedException>(() =>
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