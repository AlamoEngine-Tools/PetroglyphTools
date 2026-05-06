using System;
using Xunit;

namespace PG.StarWarsGame.Files.Test;

public class HolderTest_MegAndDisposableModel : PetroglyphFileHolderTest<DisposableModel, TestMegFileInfo, TestFileHolder<DisposableModel, TestMegFileInfo>>
{
    protected override DisposableModel CreateModel()
    {
        return new DisposableModel();
    }

    protected override TestMegFileInfo CreateFileInfo(string path, bool inMeg = false)
    {
        return new TestMegFileInfo { FilePath = path, IsInsideMeg = inMeg};
    }

    protected override TestFileHolder<DisposableModel, TestMegFileInfo> CreateFileHolder(DisposableModel model, TestMegFileInfo fileInfo)
    {
        return new TestFileHolder<DisposableModel, TestMegFileInfo>(model, fileInfo, ServiceProvider);
    }

    [Fact]
    public void Ctor_ThrowsArgumentNullException()
    {
        var model = CreateModel();
        var fileInfo = CreateFileInfo(DefaultFileName);
        Assert.Throws<ArgumentNullException>(() => new TestFileHolder<DisposableModel, TestMegFileInfo>(model, fileInfo, null!));
        Assert.Throws<ArgumentNullException>(() => new TestFileHolder<DisposableModel, TestMegFileInfo>(model, null!, ServiceProvider));
        Assert.Throws<ArgumentNullException>(() => new TestFileHolder<DisposableModel, TestMegFileInfo>(null!, fileInfo, ServiceProvider));
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