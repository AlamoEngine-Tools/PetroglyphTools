using AnakinRaW.CommonUtilities.Testing;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Services;
using PG.Testing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services;

public class DatServiceTest : PGTestBase
{
    private readonly IDatService _service;

    public DatServiceTest()
    {
        _service = ServiceProvider.GetRequiredService<IDatService>();
    }

    protected override void SetupServices(IServiceCollection serviceCollection)
    {
        base.SetupServices(serviceCollection);
        serviceCollection.SupportDAT();
    }

    [Fact]
    public void CreateDatBinary_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _service.CreateDatBinary(null!, new List<DatStringEntry>(), DatLayoutKind.NotOrdered));

        Assert.Throws<ArgumentNullException>(() =>
            _service.CreateDatBinary(FileSystem.FileStream.New("test.dat", FileMode.Create), null!, DatLayoutKind.NotOrdered));
    }

    [Fact]
    public void CreateDatBinary_PreserveOrder()
    {
        var binary = DatTestData.CreateUnsortedBinary();
        var model = DatTestData.CreateUnsortedModel();

        var fs = FileSystem.FileStream.New("test.dat", FileMode.Create);
        _service.CreateDatBinary(fs, model, DatLayoutKind.NotOrdered);
        fs.Dispose();

        Assert.Equal(binary.Bytes, FileSystem.File.ReadAllBytes("test.dat"));
    }

    [Fact]
    public void CreateDatBinary_SortEntries()
    {
        var binary = DatTestData.CreateSortedBinary();
        var model = DatTestData.CreateUnsortedModel();

        var fs = FileSystem.FileStream.New("test.dat", FileMode.Create);
        _service.CreateDatBinary(fs, model, DatLayoutKind.OrderedByCrc32);
        fs.Dispose();

        Assert.Equal(binary.Bytes, FileSystem.File.ReadAllBytes("test.dat"));
    }

    [Fact]
    public void CreateDatBinary_MemoryStream_PreserveOrder()
    {
        var binary = DatTestData.CreateUnsortedBinary();
        var model = DatTestData.CreateUnsortedModel();

        using var ms = new MemoryStream();
        _service.CreateDatBinary(ms, model, DatLayoutKind.NotOrdered);

        Assert.Equal(binary.Bytes, ms.ToArray());
    }

    [Fact]
    public void CreateDatBinary_MemoryStream_SortEntries()
    {
        var binary = DatTestData.CreateSortedBinary();
        var model = DatTestData.CreateUnsortedModel();

        using var ms = new MemoryStream();
        _service.CreateDatBinary(ms, model, DatLayoutKind.OrderedByCrc32);

        Assert.Equal(binary.Bytes, ms.ToArray());
    }

    [Fact]
    public void GetDatLayoutKind_MatchesExpected()
    {
        using (var fs = FileSystem.FileStream.New("MasterTextFile.dat", FileMode.Create))
        {
            using var stream = TestingHelpers.GetEmbeddedResource(typeof(DatServiceTest), "Files.mastertextfile_english.dat");
            stream.CopyTo(fs);
        }
        using (var fs = FileSystem.FileStream.New("Credits.dat", FileMode.Create))
        {
            using var stream = TestingHelpers.GetEmbeddedResource(typeof(DatServiceTest), "Files.creditstext_english.dat");
            stream.CopyTo(fs);
        }

        Assert.Equal(DatLayoutKind.OrderedByCrc32, _service.GetDatLayoutKind("MasterTextFile.dat"));
        Assert.Equal(DatLayoutKind.NotOrdered, _service.GetDatLayoutKind("Credits.dat"));
    }

    [Fact]
    public void LoadFile()
    {
        var sortedBinary = DatTestData.CreateSortedBinary();
        var unsortedBinary = DatTestData.CreateUnsortedBinary();

        FileSystem.Initialize()
            .WithFile("sorted.dat").Which(a => a.HasBytesContent(sortedBinary.Bytes))
            .WithFile("unsorted.dat").Which(a => a.HasBytesContent(unsortedBinary.Bytes));

        var sortedFileHolder = _service.LoadFile("sorted.dat");
        Assert.Equal(FileSystem.Path.GetFullPath("sorted.dat"), sortedFileHolder.FilePath);
        Assert.Equal(FileSystem.Path.GetFullPath("sorted.dat"), sortedFileHolder.FileInformation.FilePath);
        Assert.Equal(DatLayoutKind.OrderedByCrc32, sortedFileHolder.Content.Layout);
        Assert.Equal(DatTestData.CreateSortedModel(), sortedFileHolder.Content.ToList());

        var unsortedFileHolder = _service.LoadFile("unsorted.dat");
        Assert.Equal(FileSystem.Path.GetFullPath("unsorted.dat"), unsortedFileHolder.FilePath);
        Assert.Equal(FileSystem.Path.GetFullPath("unsorted.dat"), unsortedFileHolder.FileInformation.FilePath);
        Assert.Equal(DatLayoutKind.NotOrdered, unsortedFileHolder.Content.Layout);
        Assert.Equal(DatTestData.CreateUnsortedModel(), unsortedFileHolder.Content.ToList());
    }

    [Fact]
    public void LoadFileAs_SortedAsUnsorted()
    {
        var sortedBinary = DatTestData.CreateSortedBinary();

        FileSystem.Initialize()
            .WithFile("sorted.dat").Which(a => a.HasBytesContent(sortedBinary.Bytes));

        var unsortedFileHolder = _service.LoadFileAs("sorted.dat", DatLayoutKind.NotOrdered);

        Assert.Equal(FileSystem.Path.GetFullPath("sorted.dat"), unsortedFileHolder.FilePath);
        Assert.Equal(FileSystem.Path.GetFullPath("sorted.dat"), unsortedFileHolder.FileInformation.FilePath);
        // Entries are still sorted, but the key sort oder was adjusted
        Assert.Equal(DatLayoutKind.NotOrdered, unsortedFileHolder.Content.Layout);
        Assert.Equal(DatTestData.CreateSortedModel(), unsortedFileHolder.Content.ToList());
    }

    [Fact]
    public void LoadFileAs_UnsortedAsSorted_Throws()
    {
        var unsortedBinary = DatTestData.CreateUnsortedBinary();

        FileSystem.Initialize()
            .WithFile("unsorted.dat").Which(a => a.HasBytesContent(unsortedBinary.Bytes));

        Assert.Throws<InvalidOperationException>(() =>
            _service.LoadFileAs("unsorted.dat", DatLayoutKind.OrderedByCrc32));
    }

    [Fact]
    public void LoadStore_Sorted()
    {
        FileSystem.Initialize();
        using (var fs = FileSystem.FileStream.New("MasterTextFile.dat", FileMode.Create))
        {
            using var stream = TestingHelpers.GetEmbeddedResource(typeof(DatServiceTest), "Files.mastertextfile_english.dat");
            stream.CopyTo(fs);
        }

        var datFile = _service.LoadFile("MasterTextFile.dat").Content;
        Assert.Equal(DatLayoutKind.OrderedByCrc32, datFile.Layout);

        using (var fs = FileSystem.FileStream.New("NewSorted.dat", FileMode.Create))
            _service.CreateDatBinary(fs, datFile, DatLayoutKind.OrderedByCrc32);

        var asUnsortedDatFile = _service.LoadFileAs("MasterTextFile.dat", DatLayoutKind.NotOrdered).Content;
        Assert.Equal(DatLayoutKind.NotOrdered, asUnsortedDatFile.Layout);

        using (var fs = FileSystem.FileStream.New("NewUnsorted.dat", FileMode.Create))
            _service.CreateDatBinary(fs, asUnsortedDatFile, DatLayoutKind.NotOrdered);


        using (var fs = FileSystem.FileStream.New("NewSorted.dat", FileMode.Create))
            _service.CreateDatBinary(fs, datFile, DatLayoutKind.OrderedByCrc32);
        using (var fs = FileSystem.FileStream.New("NewUnsorted.dat", FileMode.Create))
            _service.CreateDatBinary(fs, asUnsortedDatFile, DatLayoutKind.OrderedByCrc32);

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
            using var stream = TestingHelpers.GetEmbeddedResource(typeof(DatServiceTest), "Files.creditstext_english.dat");
            stream.CopyTo(fs);
        }

        var datFile = _service.LoadFile("Credits.dat").Content;
        Assert.Equal(DatLayoutKind.NotOrdered, datFile.Layout);

        using (var fs = FileSystem.FileStream.New("New.dat", FileMode.Create))
            _service.CreateDatBinary(fs, datFile, DatLayoutKind.NotOrdered);

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
            using var stream = TestingHelpers.GetEmbeddedResource(typeof(DatServiceTest), "Files.creditstext_english.dat");
            stream.CopyTo(fs);
        }

        var datFile = _service.LoadFile("Credits.dat").Content;

        using (var fs = FileSystem.FileStream.New("New.dat", FileMode.Create))
            _service.CreateDatBinary(fs, datFile, DatLayoutKind.OrderedByCrc32);

        var creditBytes = FileSystem.File.ReadAllBytes("Credits.dat");
        var resortedBytes = FileSystem.File.ReadAllBytes("New.dat");
        Assert.NotEqual(creditBytes, resortedBytes);

        Assert.Throws<InvalidOperationException>(() => _service.LoadFileAs("Credits.dat", DatLayoutKind.OrderedByCrc32));
    }

    [Fact]
    public void LoadFile_Empty()
    {
        FileSystem.Initialize();
        using (var fs = FileSystem.FileStream.New("Empty.dat", FileMode.Create))
        {
            using var stream = TestingHelpers.GetEmbeddedResource(typeof(DatServiceTest), "Files.Empty.dat");
            stream.CopyTo(fs);
        }

        var model = _service.LoadFile("Empty.dat");
        Assert.Empty(model.Content);

        model = _service.LoadFile(FileSystem.File.OpenRead("Empty.dat"));
        Assert.Empty(model.Content);
    }

    [Fact]
    public void LoadFile_EmptyKeyWithValue()
    {
        FileSystem.Initialize();
        using (var fs = FileSystem.FileStream.New("EmptyKeyWithValue.dat", FileMode.Create))
        {
            using var stream = TestingHelpers.GetEmbeddedResource(typeof(DatServiceTest), "Files.EmptyKeyWithValue.dat");
            stream.CopyTo(fs);
        }

        var model = _service.LoadFile("EmptyKeyWithValue.dat");
        Assert.Single(model.Content);
        Assert.True(model.Content.ContainsKey(string.Empty));

        model = _service.LoadFile(FileSystem.File.OpenRead("EmptyKeyWithValue.dat"));
        Assert.Single(model.Content);
        Assert.True(model.Content.ContainsKey(string.Empty));
    }

    [Fact]
    public void LoadFile_Sorted_TwoEntriesDuplicate()
    {
        FileSystem.Initialize();
        using (var fs = FileSystem.FileStream.New("Sorted_TwoEntriesDuplicate.dat", FileMode.Create))
        {
            using var stream = TestingHelpers.GetEmbeddedResource(typeof(DatServiceTest), "Files.Sorted_TwoEntriesDuplicate.dat");
            stream.CopyTo(fs);
        }

        var model = _service.LoadFile("Sorted_TwoEntriesDuplicate.dat");
        Assert.Equal(2, model.Content.Count);
        Assert.Single(model.Content.Keys);

        model = _service.LoadFile(FileSystem.File.OpenRead("Sorted_TwoEntriesDuplicate.dat"));
        Assert.Equal(2, model.Content.Count);
        Assert.Single(model.Content.Keys);
    }


    [Fact]
    public void LoadModifyCreate()
    {
        FileSystem.Initialize();
        using (var fs = FileSystem.FileStream.New("Index_WithDuplicates.dat", FileMode.Create))
        {
            using var stream = TestingHelpers.GetEmbeddedResource(typeof(DatServiceTest), "Files.Index_WithDuplicates.dat");
            stream.CopyTo(fs);
        }

        var model = _service.LoadFileAs("Index_WithDuplicates.dat", DatLayoutKind.NotOrdered).Content;
        Assert.Equal(DatLayoutKind.NotOrdered, model.Layout);
        var modelService = ServiceProvider.GetRequiredService<IDatModelService>();
        Assert.True(modelService.GetDuplicateEntries(model).Any());
        var withoutDups = modelService.RemoveDuplicates(model);
        Assert.False(modelService.GetDuplicateEntries(withoutDups).Any());
        var sorted = modelService.SortModel(withoutDups);
        Assert.Equal(DatLayoutKind.OrderedByCrc32, sorted.Layout);

        using (var fs = FileSystem.FileStream.New("newSorted.dat", FileMode.Create))
            _service.CreateDatBinary(fs, sorted, DatLayoutKind.OrderedByCrc32);
    }

    [Fact]
    public void LoadModel()
    {
        var sorted = DatTestData.CreateSortedBinary().Bytes;
        foreach (var model in new[]
                 {
                     _service.LoadModel(new MemoryStream(sorted)),
                     _service.LoadModel(sorted),
                     _service.LoadModel(sorted.AsSpan())
                 })
        {
            Assert.Equal(DatLayoutKind.OrderedByCrc32, model.Layout);
            Assert.Equal(DatTestData.CreateSortedModel(), model.ToList());
        }

        var unsorted = DatTestData.CreateUnsortedBinary().Bytes;
        foreach (var model in new[]
                 {
                     _service.LoadModel(new MemoryStream(unsorted)),
                     _service.LoadModel(unsorted),
                     _service.LoadModel(unsorted.AsSpan())
                 })
        {
            Assert.Equal(DatLayoutKind.NotOrdered, model.Layout);
            Assert.Equal(DatTestData.CreateUnsortedModel(), model.ToList());
        }
    }

    [Fact]
    public void LoadModelAs_SortedAsUnsorted()
    {
        var sorted = DatTestData.CreateSortedBinary().Bytes;
        foreach (var model in new[]
                 {
                     _service.LoadModelAs(new MemoryStream(sorted), DatLayoutKind.NotOrdered),
                     _service.LoadModelAs(sorted, DatLayoutKind.NotOrdered),
                     _service.LoadModelAs(sorted.AsSpan(), DatLayoutKind.NotOrdered)
                 })
        {
            // Entries are still sorted, but the key sort order was adjusted.
            Assert.Equal(DatLayoutKind.NotOrdered, model.Layout);
            Assert.Equal(DatTestData.CreateSortedModel(), model.ToList());
        }
    }

    [Fact]
    public void LoadModelAs_UnsortedAsSorted_Throws()
    {
        var unsorted = DatTestData.CreateUnsortedBinary().Bytes;

        Assert.Throws<InvalidOperationException>(() => _service.LoadModelAs(new MemoryStream(unsorted), DatLayoutKind.OrderedByCrc32));
        Assert.Throws<InvalidOperationException>(() => _service.LoadModelAs(unsorted, DatLayoutKind.OrderedByCrc32));
        Assert.Throws<InvalidOperationException>(() => _service.LoadModelAs(unsorted.AsSpan(), DatLayoutKind.OrderedByCrc32));
    }

    [Fact]
    public void GetDatLayoutKind_FromMemory_MatchesExpected()
    {
        var sorted = DatTestData.CreateSortedBinary().Bytes;
        var unsorted = DatTestData.CreateUnsortedBinary().Bytes;

        Assert.Equal(DatLayoutKind.OrderedByCrc32, _service.GetDatLayoutKind(new MemoryStream(sorted)));
        Assert.Equal(DatLayoutKind.OrderedByCrc32, _service.GetDatLayoutKind(sorted));
        Assert.Equal(DatLayoutKind.OrderedByCrc32, _service.GetDatLayoutKind(sorted.AsSpan()));

        Assert.Equal(DatLayoutKind.NotOrdered, _service.GetDatLayoutKind(new MemoryStream(unsorted)));
        Assert.Equal(DatLayoutKind.NotOrdered, _service.GetDatLayoutKind(unsorted));
        Assert.Equal(DatLayoutKind.NotOrdered, _service.GetDatLayoutKind(unsorted.AsSpan()));
    }

    [Fact]
    public void LoadModel_NonSeekableStream()
    {
        using var nonSeekable = new NonSeekableReadStream(DatTestData.CreateUnsortedBinary().Bytes);

        var model = _service.LoadModel(nonSeekable);

        Assert.Equal(DatLayoutKind.NotOrdered, model.Layout);
        Assert.Equal(DatTestData.CreateUnsortedModel(), model.ToList());
    }

    [Fact]
    public void LoadModel_NullArgs_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _service.LoadModel((Stream)null!));
        Assert.Throws<ArgumentNullException>(() => _service.LoadModel((byte[])null!));
        Assert.Throws<ArgumentNullException>(() => _service.LoadModelAs((Stream)null!, DatLayoutKind.NotOrdered));
        Assert.Throws<ArgumentNullException>(() => _service.LoadModelAs((byte[])null!, DatLayoutKind.NotOrdered));
        Assert.Throws<ArgumentNullException>(() => _service.GetDatLayoutKind((Stream)null!));
        Assert.Throws<ArgumentNullException>(() => _service.GetDatLayoutKind((byte[])null!));
    }
}