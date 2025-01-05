using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Services;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services;

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

    [Fact]
    public void Test_MergeSorted_Throws()
    {
        var sortedModel = new SortedDatModel([]);
        var unsortedModel = new UnsortedDatModel([]);

        Assert.Throws<ArgumentNullException>(() => Service.MergeSorted(null!, sortedModel, out _));
        Assert.Throws<ArgumentNullException>(() => Service.MergeSorted(sortedModel, null!, out _));


        Assert.Throws<ArgumentException>(() => Service.MergeSorted(unsortedModel, sortedModel, out _));
        Assert.Throws<ArgumentException>(() => Service.MergeSorted(sortedModel, unsortedModel, out _));
        Assert.Throws<ArgumentException>(() => Service.MergeSorted(unsortedModel, unsortedModel, out _));
    }

    [Fact]
    public void Test_MergeUnsorted_Throws()
    {
        var sortedModel = new SortedDatModel([]);
        var unsortedModel = new UnsortedDatModel([]);

        Assert.Throws<ArgumentNullException>(() => Service.MergeUnsorted(null!, unsortedModel, out _));
        Assert.Throws<ArgumentNullException>(() => Service.MergeUnsorted(unsortedModel, null!, out _));


        Assert.Throws<ArgumentException>(() => Service.MergeUnsorted(unsortedModel, sortedModel, out _));
        Assert.Throws<ArgumentException>(() => Service.MergeUnsorted(sortedModel, unsortedModel, out _));
        Assert.Throws<ArgumentException>(() => Service.MergeUnsorted(sortedModel, sortedModel, out _));
    }
}