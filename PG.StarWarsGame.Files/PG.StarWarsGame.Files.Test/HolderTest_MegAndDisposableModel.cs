using System;
using Xunit;

namespace PG.StarWarsGame.Files.Test;

public class HolderTest_MegAndDisposableModel : PetroglyphFileHolderTest<DisposableModel, MegTestParam, TestFileHolder<DisposableModel, MegTestParam>>
{
    protected override DisposableModel CreateModel()
    {
        return new DisposableModel();
    }

    protected override MegTestParam CreateFileInfo(string path, bool inMeg = false)
    {
        return new MegTestParam { FilePath = path, IsInsideMeg = inMeg};
    }

    protected override TestFileHolder<DisposableModel, MegTestParam> CreateFileHolder(DisposableModel model, MegTestParam fileInfo)
    {
        return new TestFileHolder<DisposableModel, MegTestParam>(model, fileInfo, ServiceProvider);
    }

    [Fact]
    public void Test_Ctor_ThrowsArgumentNullException()
    {
        var model = CreateModel();
        var fileInfo = CreateFileInfo(DefaultFileName);
        Assert.Throws<ArgumentNullException>(() => new TestFileHolder<DisposableModel, MegTestParam>(model, fileInfo, null!));
        Assert.Throws<ArgumentNullException>(() => new TestFileHolder<DisposableModel, MegTestParam>(model, null!, ServiceProvider));
        Assert.Throws<ArgumentNullException>(() => new TestFileHolder<DisposableModel, MegTestParam>(null!, fileInfo, ServiceProvider));
    }

    [Fact]
    public void Dispose_ModelIsAlsoDisposed()
    {
        var model = CreateModel();
        FileSystem.File.Create(DefaultFileName);
        var holder = CreateFileHolder(model, CreateFileInfo(DefaultFileName));
        holder.Dispose();

        Assert.True(model.IsDisposed);
    }

    [Fact]
    public void Dispose_OriginalFileInfoIsNotDisposed()
    {
        var model = CreateModel();
        FileSystem.File.Create(DefaultFileName);
        var fileInfo = CreateFileInfo(DefaultFileName);
        var holder = CreateFileHolder(model, fileInfo);
        holder.Dispose();

        Assert.False(fileInfo.IsDisposed);
    }
}