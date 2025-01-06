using System;
using System.Collections.Generic;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Services;
using PG.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services;

public abstract partial class DatModelServiceTest : CommonTestBase
{
    private protected readonly DatModelService Service;

    protected abstract IDatModel CreateModel(IList<DatStringEntry> entries);

    protected DatModelServiceTest()
    {
        Service = new DatModelService(ServiceProvider);
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