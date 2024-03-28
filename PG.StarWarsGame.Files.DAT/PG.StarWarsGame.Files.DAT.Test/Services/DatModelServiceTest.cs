using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Services;
using Testably.Abstractions.Testing;

namespace PG.StarWarsGame.Files.DAT.Test.Services;

//[TestClass]
//public class SortedDatModelServiceTest : DatModelServiceTest
//{
//    private ISortedDatModel CreateSorted(IList<DatStringEntry> entries)
//    {
//        return new SortedDatModel(entries);
//    }

//    protected override IDatModel CreateModel(IList<DatStringEntry> entries)
//    {
//        return CreateSorted(entries);
//    }
//}

public abstract class DatModelServiceTest
{
    private DatModelService _service;
    private readonly MockFileSystem _fileSystem = new();

    protected abstract IDatModel CreateModel(IList<DatStringEntry> entries);

    [TestInitialize]
    public void Setup()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        _service = new DatModelService(sc.BuildServiceProvider());
    }

    [DataTestMethod]
    [DynamicData(nameof(GetDuplicateEntries_ModelsWithoutDuplicates), DynamicDataSourceType.Method)]
    public void Test_GetDuplicateEntries_NoDuplicates(IList<DatStringEntry> entries)
    {
        var model = CreateModel(entries);
        var duplicates = _service.GetDuplicateEntries(model);
        Assert.AreEqual(0, duplicates.Keys.Count());
    }

    private static IEnumerable<object[]> GetDuplicateEntries_ModelsWithoutDuplicates()
    {
        yield return
        [
            new List<DatStringEntry>()
        ];
        yield return
        [
            new List<DatStringEntry>
            {
                new("key", new Crc32(1), "value")
            }
        ];
        yield return
        [
            new List<DatStringEntry>
            {
                new("key1", new Crc32(1), "value1"),
                new("key2", new Crc32(2), "value2"),
            }
        ];
    }

    [TestMethod]
    public void Test_GetDuplicateEntries_HasDuplicates()
    {
        var model = CreateModel(new List<DatStringEntry>
        {

        });
        var duplicates = _service.GetDuplicateEntries(model);
    }
}