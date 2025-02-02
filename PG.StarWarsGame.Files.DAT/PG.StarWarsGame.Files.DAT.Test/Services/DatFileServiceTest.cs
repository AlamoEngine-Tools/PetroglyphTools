using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services;

public class DatFileServiceTest : CommonDatTestBase
{
    private readonly DatFileService _service;

    public DatFileServiceTest()
    {
        _service = new DatFileService(ServiceProvider);
    }

    [Fact]
    public void CreateDatFile_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _service.CreateDatFile(null!, new List<DatStringEntry>(), DatFileType.NotOrdered));

        Assert.Throws<ArgumentNullException>(() =>
            _service.CreateDatFile(FileSystem.FileStream.New("test.dat", FileMode.Create), null!, DatFileType.NotOrdered));
    }

    [Fact]
    public void CreateDatFile_PreserveOrder()
    {
        var binary = DatTestData.CreateUnsortedBinary();
        var model = DatTestData.CreateUnsortedModel();

        var fs = FileSystem.FileStream.New("test.dat", FileMode.Create);
        _service.CreateDatFile(fs, model, DatFileType.NotOrdered);
        fs.Dispose();

        Assert.Equal(binary.Bytes, FileSystem.File.ReadAllBytes("test.dat"));
    }

    [Fact]
    public void CreateDatFile_SortEntries()
    {
        var binary = DatTestData.CreateSortedBinary();
        var model = DatTestData.CreateUnsortedModel();

        var fs = FileSystem.FileStream.New("test.dat", FileMode.Create);
        _service.CreateDatFile(fs, model, DatFileType.OrderedByCrc32);
        fs.Dispose();

        Assert.Equal(binary.Bytes, FileSystem.File.ReadAllBytes("test.dat"));
    }

    [Fact]
    public void GetDatFileType_MatchesExpected()
    {
        using (var fs = FileSystem.FileStream.New("MasterTextFile.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceTest), "Files.mastertextfile_english.dat");
            stream.CopyTo(fs);
        }
        using (var fs = FileSystem.FileStream.New("Credits.dat", FileMode.Create))
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
        var sortedBinary = DatTestData.CreateSortedBinary();
        var unsortedBinary = DatTestData.CreateUnsortedBinary();

        FileSystem.Initialize()
            .WithFile("sorted.dat").Which(a => a.HasBytesContent(sortedBinary.Bytes))
            .WithFile("unsorted.dat").Which(a => a.HasBytesContent(unsortedBinary.Bytes));

        var sortedFileHolder = _service.Load("sorted.dat");
        Assert.Equal(FileSystem.Path.GetFullPath("sorted.dat"), sortedFileHolder.FilePath);
        Assert.Equal(FileSystem.Path.GetFullPath("sorted.dat"), sortedFileHolder.FileInformation.FilePath);
        Assert.Equal(DatFileType.OrderedByCrc32, sortedFileHolder.Content.KeySortOrder);
        Assert.Equal(DatTestData.CreateSortedModel(), sortedFileHolder.Content.ToList());

        var unsortedFileHolder = _service.Load("unsorted.dat");
        Assert.Equal(FileSystem.Path.GetFullPath("unsorted.dat"), unsortedFileHolder.FilePath);
        Assert.Equal(FileSystem.Path.GetFullPath("unsorted.dat"), unsortedFileHolder.FileInformation.FilePath);
        Assert.Equal(DatFileType.NotOrdered, unsortedFileHolder.Content.KeySortOrder);
        Assert.Equal(DatTestData.CreateUnsortedModel(), unsortedFileHolder.Content.ToList());
    }

    [Fact]
    public void LoadAs_SortedAsUnsorted()
    {
        var sortedBinary = DatTestData.CreateSortedBinary();

        FileSystem.Initialize()
            .WithFile("sorted.dat").Which(a => a.HasBytesContent(sortedBinary.Bytes));

        var unsortedFileHolder = _service.LoadAs("sorted.dat", DatFileType.NotOrdered);

        Assert.Equal(FileSystem.Path.GetFullPath("sorted.dat"), unsortedFileHolder.FilePath);
        Assert.Equal(FileSystem.Path.GetFullPath("sorted.dat"), unsortedFileHolder.FileInformation.FilePath);
        // Entries are still sorted, but the key sort oder was adjusted
        Assert.Equal(DatFileType.NotOrdered, unsortedFileHolder.Content.KeySortOrder);
        Assert.Equal(DatTestData.CreateSortedModel(), unsortedFileHolder.Content.ToList());
    }

    [Fact]
    public void LoadAs_UnsortedAsSorted_Throws()
    {
        var unsortedBinary = DatTestData.CreateUnsortedBinary();

        FileSystem.Initialize()
            .WithFile("unsorted.dat").Which(a => a.HasBytesContent(unsortedBinary.Bytes));

        Assert.Throws<InvalidOperationException>(() =>
            _service.LoadAs("unsorted.dat", DatFileType.OrderedByCrc32));
    }

    [Fact]
    public void LoadStore_Sorted()
    {
        FileSystem.Initialize();
        using (var fs = FileSystem.FileStream.New("MasterTextFile.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceTest), "Files.mastertextfile_english.dat");
            stream.CopyTo(fs);
        }

        var datFile = _service.Load("MasterTextFile.dat").Content;
        Assert.Equal(DatFileType.OrderedByCrc32, datFile.KeySortOrder);

        using (var fs = FileSystem.FileStream.New("NewSorted.dat", FileMode.Create))
            _service.CreateDatFile(fs, datFile, DatFileType.OrderedByCrc32);

        var asUnsortedDatFile = _service.LoadAs("MasterTextFile.dat", DatFileType.NotOrdered).Content;
        Assert.Equal(DatFileType.NotOrdered, asUnsortedDatFile.KeySortOrder);

        using (var fs = FileSystem.FileStream.New("NewUnsorted.dat", FileMode.Create))
            _service.CreateDatFile(fs, asUnsortedDatFile, DatFileType.NotOrdered);


        using (var fs = FileSystem.FileStream.New("NewSorted.dat", FileMode.Create))
            _service.CreateDatFile(fs, datFile, DatFileType.OrderedByCrc32);
        using (var fs = FileSystem.FileStream.New("NewUnsorted.dat", FileMode.Create))
            _service.CreateDatFile(fs, asUnsortedDatFile, DatFileType.OrderedByCrc32);

        var expectedBytes = FileSystem.File.ReadAllBytes("MasterTextFile.dat");
        var actualBytesSorted = FileSystem.File.ReadAllBytes("NewSorted.dat");
        var actualBytesUnsorted = FileSystem.File.ReadAllBytes("NewUnsorted.dat");
        Assert.Equal(expectedBytes, actualBytesSorted);
        Assert.Equal(expectedBytes, actualBytesUnsorted);
    }

    [Fact]
    public void LoadStore_Unsorted()
    {
        FileSystem.Initialize();
        using (var fs = FileSystem.FileStream.New("Credits.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceTest), "Files.creditstext_english.dat");
            stream.CopyTo(fs);
        }

        var datFile = _service.Load("Credits.dat").Content;
        Assert.Equal(DatFileType.NotOrdered, datFile.KeySortOrder);

        using (var fs = FileSystem.FileStream.New("New.dat", FileMode.Create))
            _service.CreateDatFile(fs, datFile, DatFileType.NotOrdered);

        var expectedBytes = FileSystem.File.ReadAllBytes("Credits.dat");
        var actualBytesSorted = FileSystem.File.ReadAllBytes("New.dat");
        Assert.Equal(expectedBytes, actualBytesSorted);
    }

    [Fact]
    public void LoadStore_UnsortedAsSortedThrows()
    {
        FileSystem.Initialize();
        using (var fs = FileSystem.FileStream.New("Credits.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceTest), "Files.creditstext_english.dat");
            stream.CopyTo(fs);
        }

        var datFile = _service.Load("Credits.dat").Content;

        using (var fs = FileSystem.FileStream.New("New.dat", FileMode.Create))
            _service.CreateDatFile(fs, datFile, DatFileType.OrderedByCrc32);

        var creditBytes = FileSystem.File.ReadAllBytes("Credits.dat");
        var resortedBytes = FileSystem.File.ReadAllBytes("New.dat");
        Assert.NotEqual(creditBytes, resortedBytes);

        Assert.Throws<InvalidOperationException>(() => _service.LoadAs("Credits.dat", DatFileType.OrderedByCrc32));
    }

    [Fact]
    public void Load_Empty()
    {
        FileSystem.Initialize();
        using (var fs = FileSystem.FileStream.New("Empty.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceTest), "Files.Empty.dat");
            stream.CopyTo(fs);
        }

        var model = _service.Load("Empty.dat");
        Assert.Empty(model.Content);

        model = _service.Load(FileSystem.File.OpenRead("Empty.dat"));
        Assert.Empty(model.Content);
    }

    [Fact]
    public void Load_EmptyKeyWithValue()
    {
        FileSystem.Initialize();
        using (var fs = FileSystem.FileStream.New("EmptyKeyWithValue.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceTest), "Files.EmptyKeyWithValue.dat");
            stream.CopyTo(fs);
        }

        var model = _service.Load("EmptyKeyWithValue.dat");
        Assert.Single(model.Content);
        Assert.True(model.Content.ContainsKey(string.Empty));

        model = _service.Load(FileSystem.File.OpenRead("EmptyKeyWithValue.dat"));
        Assert.Single(model.Content);
        Assert.True(model.Content.ContainsKey(string.Empty));
    }

    [Fact]
    public void Load_Sorted_TwoEntriesDuplicate()
    {
        FileSystem.Initialize();
        using (var fs = FileSystem.FileStream.New("Sorted_TwoEntriesDuplicate.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceTest), "Files.Sorted_TwoEntriesDuplicate.dat");
            stream.CopyTo(fs);
        }

        var model = _service.Load("Sorted_TwoEntriesDuplicate.dat");
        Assert.Equal(2, model.Content.Count);
        Assert.Single(model.Content.Keys);

        model = _service.Load(FileSystem.File.OpenRead("Sorted_TwoEntriesDuplicate.dat"));
        Assert.Equal(2, model.Content.Count);
        Assert.Single(model.Content.Keys);
    }


    [Fact]
    public void LoadModifyCreate()
    {
        FileSystem.Initialize();
        using (var fs = FileSystem.FileStream.New("Index_WithDuplicates.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceTest), "Files.Index_WithDuplicates.dat");
            stream.CopyTo(fs);
        }

        var model = _service.LoadAs("Index_WithDuplicates.dat", DatFileType.NotOrdered).Content;
        Assert.Equal(DatFileType.NotOrdered, model.KeySortOrder);
        var modelService = ServiceProvider.GetRequiredService<IDatModelService>();
        Assert.True(modelService.GetDuplicateEntries(model).Any());
        var withoutDups = modelService.RemoveDuplicates(model);
        Assert.False(modelService.GetDuplicateEntries(withoutDups).Any());
        var sorted = modelService.SortModel(withoutDups);
        Assert.Equal(DatFileType.OrderedByCrc32, sorted.KeySortOrder);

        using (var fs = FileSystem.FileStream.New("newSorted.dat", FileMode.Create))
            _service.CreateDatFile(fs, sorted, DatFileType.OrderedByCrc32);
    }
}