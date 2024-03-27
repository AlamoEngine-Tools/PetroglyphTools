using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services;
using PG.Testing;
using Testably.Abstractions.Testing;

namespace PG.StarWarsGame.Files.DAT.Test.Services;

[TestClass]
public class DatFileServiceIntegrationTest
{
    private readonly MockFileSystem _fileSystem = new();
    private DatFileService _service;

    [TestInitialize]
    public void Setup()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        PGDomain.RegisterServices(sc);
        DatDomain.RegisterServices(sc);
        _service = new DatFileService(sc.BuildServiceProvider());
    }

    [TestMethod]
    public void Test_LoadStore_Sorted()
    {
        _fileSystem.Initialize();
        using (var fs = _fileSystem.FileStream.New("MasterTextFile.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceIntegrationTest), "Files.mastertextfile_english.dat");
            stream.CopyTo(fs);
        }

        var datFile = _service.Load("MasterTextFile.dat").Content;
        Assert.AreEqual(DatFileType.OrderedByCrc32, datFile.KeySortOder);

        using (var fs = _fileSystem.FileStream.New("NewSorted.dat", FileMode.Create))
            _service.CreateDatFile(fs, datFile, DatFileType.OrderedByCrc32);

        var asUnsortedDatFile = _service.LoadAs("MasterTextFile.dat", DatFileType.NotOrdered).Content;
        Assert.AreEqual(DatFileType.NotOrdered, asUnsortedDatFile.KeySortOder);

        using (var fs = _fileSystem.FileStream.New("NewUnsorted.dat", FileMode.Create))
            _service.CreateDatFile(fs, asUnsortedDatFile, DatFileType.NotOrdered);


        using (var fs = _fileSystem.FileStream.New("NewSorted.dat", FileMode.Create)) 
            _service.CreateDatFile(fs, datFile, DatFileType.OrderedByCrc32);
        using (var fs = _fileSystem.FileStream.New("NewUnsorted.dat", FileMode.Create))
            _service.CreateDatFile(fs, asUnsortedDatFile, DatFileType.OrderedByCrc32);

        var expectedBytes = _fileSystem.File.ReadAllBytes("MasterTextFile.dat");
        var actualBytesSorted = _fileSystem.File.ReadAllBytes("NewSorted.dat");
        var actualBytesUnsorted = _fileSystem.File.ReadAllBytes("NewUnsorted.dat");
        CollectionAssert.AreEqual(expectedBytes, actualBytesSorted);
        CollectionAssert.AreEqual(expectedBytes, actualBytesUnsorted);
    }

    [TestMethod]
    public void Test_LoadStore_Unsorted()
    {
        _fileSystem.Initialize();
        using (var fs = _fileSystem.FileStream.New("Credits.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceIntegrationTest), "Files.creditstext_english.dat");
            stream.CopyTo(fs);
        }

        var datFile = _service.Load("Credits.dat").Content;
        Assert.AreEqual(DatFileType.NotOrdered, datFile.KeySortOder);

        using (var fs = _fileSystem.FileStream.New("New.dat", FileMode.Create))
            _service.CreateDatFile(fs, datFile, DatFileType.NotOrdered);

        var expectedBytes = _fileSystem.File.ReadAllBytes("Credits.dat");
        var actualBytesSorted = _fileSystem.File.ReadAllBytes("New.dat");
        CollectionAssert.AreEqual(expectedBytes, actualBytesSorted);
    }

    [TestMethod]
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
        CollectionAssert.AreNotEqual(creditBytes, resortedBytes);

        Assert.ThrowsException<NotSupportedException>(() => _service.LoadAs("Credits.dat", DatFileType.OrderedByCrc32));
    }

    [TestMethod]
    public void Test_Load_Empty()
    {
        _fileSystem.Initialize();
        using (var fs = _fileSystem.FileStream.New("Empty.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceIntegrationTest), "Files.Empty.dat");
            stream.CopyTo(fs);
        }
        
        var model = _service.Load("Empty.dat");
        Assert.AreEqual(0, model.Content.Count);
    }

    [TestMethod]
    public void Test_Load_EmptyKeyWithValue()
    {
        _fileSystem.Initialize();
        using (var fs = _fileSystem.FileStream.New("EmptyKeyWithValue.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceIntegrationTest), "Files.EmptyKeyWithValue.dat");
            stream.CopyTo(fs);
        }

        var model = _service.Load("EmptyKeyWithValue.dat");
        Assert.AreEqual(1, model.Content.Count);
        Assert.IsTrue(model.Content.ContainsKey(string.Empty));
    }

    [TestMethod]
    public void Test_Load_Sorted_TwoEntriesDuplicate()
    {
        _fileSystem.Initialize();
        using (var fs = _fileSystem.FileStream.New("Sorted_TwoEntriesDuplicate.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceIntegrationTest), "Files.Sorted_TwoEntriesDuplicate.dat");
            stream.CopyTo(fs);
        }

        var model = _service.Load("Sorted_TwoEntriesDuplicate.dat");
        Assert.AreEqual(2, model.Content.Count);
        Assert.AreEqual(1, model.Content.Keys.Count);
    }


    //[TestMethod]
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