using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services;

public class DatFileServiceIntegrationTest
{
    private readonly MockFileSystem _fileSystem = new();
    private readonly DatFileService _service;

    public DatFileServiceIntegrationTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        PGDomain.RegisterServices(sc);
        DatDomain.RegisterServices(sc);
        _service = new DatFileService(sc.BuildServiceProvider());
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

        Assert.Throws<NotSupportedException>(() => _service.LoadAs("Credits.dat", DatFileType.OrderedByCrc32));
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
        Assert.Equal(0, model.Content.Count);
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
        Assert.Equal(1, model.Content.Count);
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
        Assert.Equal(1, model.Content.Keys.Count);
    }


    //[Fact]
    public void Test()
    {
        var fileSystem = new FileSystem();

        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(fileSystem);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        PGDomain.RegisterServices(sc);
        DatDomain.RegisterServices(sc);
        var sp = sc.BuildServiceProvider();
        var service = new DatFileService(sp);

        var path = "C:/test/MasterTextFile_german.dat";

        using (var fs = fileSystem.FileStream.New(path, FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceIntegrationTest), "Files.mastertextfile_english.dat");
            stream.CopyTo(fs);
        }

        var datFile = service.Load(path).Content;

        var entries = datFile.ToList();

        var crcT = sp.GetRequiredService<ICrc32HashingService>()
            .GetCrc32("TEXT_TOOLTIP_TARKIN_01", DatFileConstants.TextKeyEncoding);
        entries.Add(new DatStringEntry("TEXT_TOOLTIP_TARKIN_01", crcT, "bla bla bla"));

        var pietIndex = entries.FindIndex(e => e.Key == "TEXT_TOOLTIP_VEERS_01");
        var cPiet = entries[pietIndex];
        var newPiet = new DatStringEntry(cPiet.Key, cPiet.Crc32, "This is some text\nwith real\r\nline breaks and \t a tab.");
        entries[pietIndex] = newPiet;

        using var writeFs = fileSystem.FileStream.New(path, FileMode.Open, FileAccess.Write);

        service.CreateDatFile(writeFs, entries, DatFileType.OrderedByCrc32);
    }
}