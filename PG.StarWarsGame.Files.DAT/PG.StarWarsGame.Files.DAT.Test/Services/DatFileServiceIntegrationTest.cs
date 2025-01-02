using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services;

public class DatFileServiceIntegrationTest
{
    private readonly MockFileSystem _fileSystem = new();
    private readonly IDatFileService _service;
    private readonly IServiceProvider _serviceProvider;

    public DatFileServiceIntegrationTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        PetroglyphCommons.ContributeServices(sc);
        sc.SupportDAT();
        _serviceProvider = sc.BuildServiceProvider();
        _service = _serviceProvider.GetRequiredService<IDatFileService>();
    }

    [Fact]
    public void Test_LoadStore_Sorted()
    {
        _fileSystem.Initialize();
        using (var fs = _fileSystem.FileStream.New("MasterTextFile.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceIntegrationTest), "Files.mastertextfile_english.dat");
            stream.CopyTo(fs);
        }

        var datFile = _service.Load("MasterTextFile.dat").Content;
        Assert.Equal(DatFileType.OrderedByCrc32, datFile.KeySortOder);

        using (var fs = _fileSystem.FileStream.New("NewSorted.dat", FileMode.Create))
            _service.CreateDatFile(fs, datFile, DatFileType.OrderedByCrc32);

        var asUnsortedDatFile = _service.LoadAs("MasterTextFile.dat", DatFileType.NotOrdered).Content;
        Assert.Equal(DatFileType.NotOrdered, asUnsortedDatFile.KeySortOder);

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
    public void Test_LoadStore_Unsorted()
    {
        _fileSystem.Initialize();
        using (var fs = _fileSystem.FileStream.New("Credits.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceIntegrationTest), "Files.creditstext_english.dat");
            stream.CopyTo(fs);
        }

        var datFile = _service.Load("Credits.dat").Content;
        Assert.Equal(DatFileType.NotOrdered, datFile.KeySortOder);

        using (var fs = _fileSystem.FileStream.New("New.dat", FileMode.Create))
            _service.CreateDatFile(fs, datFile, DatFileType.NotOrdered);

        var expectedBytes = _fileSystem.File.ReadAllBytes("Credits.dat");
        var actualBytesSorted = _fileSystem.File.ReadAllBytes("New.dat");
        Assert.Equal(expectedBytes, actualBytesSorted);
    }

    [Fact]
    public void Test_LoadStore_UnsortedAsSortedThrows()
    {
        _fileSystem.Initialize();
        using (var fs = _fileSystem.FileStream.New("Credits.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceIntegrationTest), "Files.creditstext_english.dat");
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
    public void Test_Load_Empty()
    {
        _fileSystem.Initialize();
        using (var fs = _fileSystem.FileStream.New("Empty.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceIntegrationTest), "Files.Empty.dat");
            stream.CopyTo(fs);
        }
        
        var model = _service.Load("Empty.dat");
        Assert.Empty(model.Content);

        model = _service.Load(_fileSystem.File.OpenRead("Empty.dat"));
        Assert.Empty(model.Content);
    }

    [Fact]
    public void Test_Load_EmptyKeyWithValue()
    {
        _fileSystem.Initialize();
        using (var fs = _fileSystem.FileStream.New("EmptyKeyWithValue.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceIntegrationTest), "Files.EmptyKeyWithValue.dat");
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
    public void Test_Load_Sorted_TwoEntriesDuplicate()
    {
        _fileSystem.Initialize();
        using (var fs = _fileSystem.FileStream.New("Sorted_TwoEntriesDuplicate.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceIntegrationTest), "Files.Sorted_TwoEntriesDuplicate.dat");
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
    public void Test_LoadModifyCreate()
    {
        _fileSystem.Initialize();
        using (var fs = _fileSystem.FileStream.New("Index_WithDuplicates.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceIntegrationTest), "Files.Index_WithDuplicates.dat");
            stream.CopyTo(fs);
        }

        var model = _service.LoadAs("Index_WithDuplicates.dat", DatFileType.NotOrdered).Content;
        Assert.Equal(DatFileType.NotOrdered, model.KeySortOder);
        var modelService = _serviceProvider.GetRequiredService<IDatModelService>();
        Assert.True(modelService.GetDuplicateEntries(model).Any());
        var withoutDups = modelService.RemoveDuplicates(model);
        Assert.False(modelService.GetDuplicateEntries(withoutDups).Any());
        var sorted = modelService.SortModel(withoutDups);
        Assert.Equal(DatFileType.OrderedByCrc32, sorted.KeySortOder);

        using (var fs = _fileSystem.FileStream.New("newSorted.dat", FileMode.Create)) 
            _service.CreateDatFile(fs, sorted, DatFileType.OrderedByCrc32);
    }
}