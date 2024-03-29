using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
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
        var sortedModel = new Mock<IDatModel>();
        sortedModel.SetupGet(m => m.KeySortOder).Returns(DatFileType.OrderedByCrc32);

        var unsortedModel = new Mock<IDatModel>();
        unsortedModel.SetupGet(m => m.KeySortOder).Returns(DatFileType.NotOrdered);

        Assert.Throws<ArgumentNullException>(() => Service.MergeSorted(null!, sortedModel.Object, out _));
        Assert.Throws<ArgumentNullException>(() => Service.MergeSorted(sortedModel.Object, null!, out _));


        Assert.Throws<ArgumentException>(() => Service.MergeSorted(unsortedModel.Object, sortedModel.Object, out _));
        Assert.Throws<ArgumentException>(() => Service.MergeSorted(sortedModel.Object, unsortedModel.Object, out _));
        Assert.Throws<ArgumentException>(() => Service.MergeSorted(unsortedModel.Object, unsortedModel.Object, out _));
    }

    [Fact]
    public void Test_MergeUnsorted_Throws()
    {
        var sortedModel = new Mock<IDatModel>();
        sortedModel.SetupGet(m => m.KeySortOder).Returns(DatFileType.OrderedByCrc32);

        var unsortedModel = new Mock<IDatModel>();
        unsortedModel.SetupGet(m => m.KeySortOder).Returns(DatFileType.NotOrdered);

        Assert.Throws<ArgumentNullException>(() => Service.MergeUnsorted(null!, unsortedModel.Object, out _));
        Assert.Throws<ArgumentNullException>(() => Service.MergeUnsorted(unsortedModel.Object, null!, out _));


        Assert.Throws<ArgumentException>(() => Service.MergeUnsorted(unsortedModel.Object, sortedModel.Object, out _));
        Assert.Throws<ArgumentException>(() => Service.MergeUnsorted(sortedModel.Object, unsortedModel.Object, out _));
        Assert.Throws<ArgumentException>(() => Service.MergeUnsorted(sortedModel.Object, sortedModel.Object, out _));
    }
}