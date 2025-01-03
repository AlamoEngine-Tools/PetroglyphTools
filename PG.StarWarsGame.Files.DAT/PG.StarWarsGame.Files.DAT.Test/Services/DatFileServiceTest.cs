using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services;

public class DatFileServiceTest
{
    private readonly DatFileService _service;
    private readonly MockFileSystem _fileSystem = new();
    private readonly IServiceProvider _serviceProvider;

    public DatFileServiceTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        sc.AddSingleton<IFileSystem>(_fileSystem);
        PetroglyphCommons.ContributeServices(sc);
        sc.SupportDAT();
        _serviceProvider = sc.BuildServiceProvider();
        _service = new DatFileService(_serviceProvider);
    }

    [Fact]
    public void CreateDatFile_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _service.CreateDatFile(null!, new List<DatStringEntry>(), DatFileType.NotOrdered));

        Assert.Throws<ArgumentNullException>(() =>
            _service.CreateDatFile(_fileSystem.FileStream.New("test.dat", FileMode.Create), null!, DatFileType.NotOrdered));
    }

    [Fact]
    public void CreateDatFile_PreserveOrder()
    {
        var binary = CreateUnsortedBinary();
        var model = CreateUnsortedModel();

        var fs = _fileSystem.FileStream.New("test.dat", FileMode.Create);
        _service.CreateDatFile(fs, model, DatFileType.NotOrdered);
        fs.Dispose();

        Assert.Equal(binary.Bytes, _fileSystem.File.ReadAllBytes("test.dat"));
    }

    [Fact]
    public void CreateDatFile_SortEntries()
    {
        var binary = CreateSortedBinary();
        var model = CreateUnsortedModel();

        var fs = _fileSystem.FileStream.New("test.dat", FileMode.Create);
        _service.CreateDatFile(fs, model, DatFileType.OrderedByCrc32);
        fs.Dispose();

        Assert.Equal(binary.Bytes, _fileSystem.File.ReadAllBytes("test.dat"));
    }

    [Fact]
    public void GetDatFileType_MatchesExpected()
    {
        using (var fs = _fileSystem.FileStream.New("MasterTextFile.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceTest), "Files.mastertextfile_english.dat");
            stream.CopyTo(fs);
        }
        using (var fs = _fileSystem.FileStream.New("Credits.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceTest), "Files.creditstext_english.dat");
            stream.CopyTo(fs);
        }
        
        Assert.Equal(DatFileType.OrderedByCrc32, _service.GetDatFileType("MasterTextFile.dat"));
        Assert.Equal(DatFileType.NotOrdered, _service.GetDatFileType("Credits.dat"));
    }

    [Fact]
    public void Load()
    {
        var sortedBinary = CreateSortedBinary();
        var unsortedBinary = CreateUnsortedBinary();

        _fileSystem.Initialize()
            .WithFile("sorted.dat").Which(a => a.HasBytesContent(sortedBinary.Bytes))
            .WithFile("unsorted.dat").Which(a => a.HasBytesContent(unsortedBinary.Bytes));

        var sortedFileHolder =_service.Load("sorted.dat");
        Assert.Equal(_fileSystem.Path.GetFullPath("sorted.dat") , sortedFileHolder.FilePath);
        Assert.Equal(_fileSystem.Path.GetFullPath("sorted.dat") , sortedFileHolder.FileInformation.FilePath);
        Assert.Equal(DatFileType.OrderedByCrc32, sortedFileHolder.Content.KeySortOrder);
        Assert.Equal(CreateSortedModel(), sortedFileHolder.Content.ToList());

        var unsortedFileHolder = _service.Load("unsorted.dat");
        Assert.Equal(_fileSystem.Path.GetFullPath("unsorted.dat"), unsortedFileHolder.FilePath);
        Assert.Equal(_fileSystem.Path.GetFullPath("unsorted.dat"), unsortedFileHolder.FileInformation.FilePath);
        Assert.Equal(DatFileType.NotOrdered, unsortedFileHolder.Content.KeySortOrder);
        Assert.Equal(CreateUnsortedModel(), unsortedFileHolder.Content.ToList());
    }

    [Fact]
    public void LoadAs_SortedAsUnsorted()
    {
        var sortedBinary = CreateSortedBinary();

        _fileSystem.Initialize()
            .WithFile("sorted.dat").Which(a => a.HasBytesContent(sortedBinary.Bytes));

        var unsortedFileHolder = _service.LoadAs("sorted.dat", DatFileType.NotOrdered);

        Assert.Equal(_fileSystem.Path.GetFullPath("sorted.dat"), unsortedFileHolder.FilePath);
        Assert.Equal(_fileSystem.Path.GetFullPath("sorted.dat"), unsortedFileHolder.FileInformation.FilePath);
        // Entries are still sorted, but the key sort oder was adjusted
        Assert.Equal(DatFileType.NotOrdered, unsortedFileHolder.Content.KeySortOrder);
        Assert.Equal(CreateSortedModel(), unsortedFileHolder.Content.ToList());
    }

    [Fact]
    public void LoadAs_UnsortedAsSorted_Throws()
    {
        var unsortedBinary = CreateUnsortedBinary();

        _fileSystem.Initialize()
            .WithFile("unsorted.dat").Which(a => a.HasBytesContent(unsortedBinary.Bytes));

        Assert.Throws<InvalidOperationException>(() =>
            _service.LoadAs("unsorted.dat", DatFileType.OrderedByCrc32));
    }

    [Fact]
    public void LoadStore_Sorted()
    {
        _fileSystem.Initialize();
        using (var fs = _fileSystem.FileStream.New("MasterTextFile.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceTest), "Files.mastertextfile_english.dat");
            stream.CopyTo(fs);
        }

        var datFile = _service.Load("MasterTextFile.dat").Content;
        Assert.Equal(DatFileType.OrderedByCrc32, datFile.KeySortOrder);

        using (var fs = _fileSystem.FileStream.New("NewSorted.dat", FileMode.Create))
            _service.CreateDatFile(fs, datFile, DatFileType.OrderedByCrc32);

        var asUnsortedDatFile = _service.LoadAs("MasterTextFile.dat", DatFileType.NotOrdered).Content;
        Assert.Equal(DatFileType.NotOrdered, asUnsortedDatFile.KeySortOrder);

        using (var fs = _fileSystem.FileStream.New("NewUnsorted.dat", FileMode.Create))
            _service.CreateDatFile(fs, asUnsortedDatFile, DatFileType.NotOrdered);


        using (var fs = _fileSystem.FileStream.New("NewSorted.dat", FileMode.Create))
            _service.CreateDatFile(fs, datFile, DatFileType.OrderedByCrc32);
        using (var fs = _fileSystem.FileStream.New("NewUnsorted.dat", FileMode.Create))
            _service.CreateDatFile(fs, asUnsortedDatFile, DatFileType.OrderedByCrc32);

        var expectedBytes = _fileSystem.File.ReadAllBytes("MasterTextFile.dat");
        var actualBytesSorted = _fileSystem.File.ReadAllBytes("NewSorted.dat");
        var actualBytesUnsorted = _fileSystem.File.ReadAllBytes("NewUnsorted.dat");
        Assert.Equal(expectedBytes, actualBytesSorted);
        Assert.Equal(expectedBytes, actualBytesUnsorted);
    }

    [Fact]
    public void LoadStore_Unsorted()
    {
        _fileSystem.Initialize();
        using (var fs = _fileSystem.FileStream.New("Credits.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceTest), "Files.creditstext_english.dat");
            stream.CopyTo(fs);
        }

        var datFile = _service.Load("Credits.dat").Content;
        Assert.Equal(DatFileType.NotOrdered, datFile.KeySortOrder);

        using (var fs = _fileSystem.FileStream.New("New.dat", FileMode.Create))
            _service.CreateDatFile(fs, datFile, DatFileType.NotOrdered);

        var expectedBytes = _fileSystem.File.ReadAllBytes("Credits.dat");
        var actualBytesSorted = _fileSystem.File.ReadAllBytes("New.dat");
        Assert.Equal(expectedBytes, actualBytesSorted);
    }

    [Fact]
    public void LoadStore_UnsortedAsSortedThrows()
    {
        _fileSystem.Initialize();
        using (var fs = _fileSystem.FileStream.New("Credits.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceTest), "Files.creditstext_english.dat");
            stream.CopyTo(fs);
        }

        var datFile = _service.Load("Credits.dat").Content;

        using (var fs = _fileSystem.FileStream.New("New.dat", FileMode.Create))
            _service.CreateDatFile(fs, datFile, DatFileType.OrderedByCrc32);

        var creditBytes = _fileSystem.File.ReadAllBytes("Credits.dat");
        var resortedBytes = _fileSystem.File.ReadAllBytes("New.dat");
        Assert.NotEqual(creditBytes, resortedBytes);

        Assert.Throws<InvalidOperationException>(() => _service.LoadAs("Credits.dat", DatFileType.OrderedByCrc32));
    }

    [Fact]
    public void Load_Empty()
    {
        _fileSystem.Initialize();
        using (var fs = _fileSystem.FileStream.New("Empty.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceTest), "Files.Empty.dat");
            stream.CopyTo(fs);
        }

        var model = _service.Load("Empty.dat");
        Assert.Empty(model.Content);

        model = _service.Load(_fileSystem.File.OpenRead("Empty.dat"));
        Assert.Empty(model.Content);
    }

    [Fact]
    public void Load_EmptyKeyWithValue()
    {
        _fileSystem.Initialize();
        using (var fs = _fileSystem.FileStream.New("EmptyKeyWithValue.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceTest), "Files.EmptyKeyWithValue.dat");
            stream.CopyTo(fs);
        }

        var model = _service.Load("EmptyKeyWithValue.dat");
        Assert.Single(model.Content);
        Assert.True(model.Content.ContainsKey(string.Empty));

        model = _service.Load(_fileSystem.File.OpenRead("EmptyKeyWithValue.dat"));
        Assert.Single(model.Content);
        Assert.True(model.Content.ContainsKey(string.Empty));
    }

    [Fact]
    public void Load_Sorted_TwoEntriesDuplicate()
    {
        _fileSystem.Initialize();
        using (var fs = _fileSystem.FileStream.New("Sorted_TwoEntriesDuplicate.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceTest), "Files.Sorted_TwoEntriesDuplicate.dat");
            stream.CopyTo(fs);
        }

        var model = _service.Load("Sorted_TwoEntriesDuplicate.dat");
        Assert.Equal(2, model.Content.Count);
        Assert.Single(model.Content.Keys);

        model = _service.Load(_fileSystem.File.OpenRead("Sorted_TwoEntriesDuplicate.dat"));
        Assert.Equal(2, model.Content.Count);
        Assert.Single(model.Content.Keys);
    }


    [Fact]
    public void LoadModifyCreate()
    {
        _fileSystem.Initialize();
        using (var fs = _fileSystem.FileStream.New("Index_WithDuplicates.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceTest), "Files.Index_WithDuplicates.dat");
            stream.CopyTo(fs);
        }

        var model = _service.LoadAs("Index_WithDuplicates.dat", DatFileType.NotOrdered).Content;
        Assert.Equal(DatFileType.NotOrdered, model.KeySortOrder);
        var modelService = _serviceProvider.GetRequiredService<IDatModelService>();
        Assert.True(modelService.GetDuplicateEntries(model).Any());
        var withoutDups = modelService.RemoveDuplicates(model);
        Assert.False(modelService.GetDuplicateEntries(withoutDups).Any());
        var sorted = modelService.SortModel(withoutDups);
        Assert.Equal(DatFileType.OrderedByCrc32, sorted.KeySortOrder);

        using (var fs = _fileSystem.FileStream.New("newSorted.dat", FileMode.Create))
            _service.CreateDatFile(fs, sorted, DatFileType.OrderedByCrc32);
    }


    private static DatBinaryFile CreateUnsortedBinary()
    {
        return new DatBinaryFile(new DatHeader(2),
            new BinaryTable<IndexTableRecord>(new List<IndexTableRecord>
            {
                CreateIndexRecord(false),
                CreateIndexRecord(true),
            }),
            new BinaryTable<ValueTableRecord>(new List<ValueTableRecord>
            {
                new("b"),
                new("a"),
            }),
            new BinaryTable<KeyTableRecord>(new List<KeyTableRecord>
            {
                CreateKeyRecord(DatTestData.BigCrcValue),
                CreateKeyRecord(DatTestData.SmallCrcValue),
            }));
    }

    private static IReadOnlyList<DatStringEntry> CreateUnsortedModel()
    {
        return [
            CreateEntry(false, "b"),
            CreateEntry(true, "a"),
        ];
    }

    private static IReadOnlyList<DatStringEntry> CreateSortedModel()
    {
        return [
            CreateEntry(true, "a"),
            CreateEntry(false, "b"),
        ];
    }

    private static DatBinaryFile CreateSortedBinary()
    {
        return new DatBinaryFile(new DatHeader(2),
            new BinaryTable<IndexTableRecord>(new List<IndexTableRecord>
            {
                CreateIndexRecord(true),
                CreateIndexRecord(false),
            }),
            new BinaryTable<ValueTableRecord>(new List<ValueTableRecord>
            {
                new("a"),
                new("b"),
            }),
            new BinaryTable<KeyTableRecord>(new List<KeyTableRecord>
            {
                CreateKeyRecord(DatTestData.SmallCrcValue),
                CreateKeyRecord(DatTestData.BigCrcValue),
            }));
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
}