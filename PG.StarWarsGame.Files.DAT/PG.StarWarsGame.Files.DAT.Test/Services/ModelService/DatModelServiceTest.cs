using System.Collections.Generic;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Services;
using Testably.Abstractions.Testing;

namespace PG.StarWarsGame.Files.DAT.Test.Services;

public class SortedDatModelServiceTest : DatModelServiceTest
{
    private ISortedDatModel CreateSorted(IList<DatStringEntry> entries)
    {
        var sorted = Crc32Utilities.SortByCrc32(entries);
        return new SortedDatModel(sorted);
    }

    protected override IDatModel CreateModel(IList<DatStringEntry> entries)
    {
        return CreateSorted(entries);
    }
}

public class UnsortedDatModelServiceTest : DatModelServiceTest
{
    private IUnsortedDatModel CreateSorted(IList<DatStringEntry> entries)
    {
        return new UnsortedDatModel(entries);
    }

    protected override IDatModel CreateModel(IList<DatStringEntry> entries)
    {
        return CreateSorted(entries);
    }
}

public abstract partial class DatModelServiceTest
{
    private protected readonly DatModelService Service;
    private readonly MockFileSystem _fileSystem = new();

    protected abstract IDatModel CreateModel(IList<DatStringEntry> entries);

    protected DatModelServiceTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        Service = new DatModelService(sc.BuildServiceProvider());
    }
}