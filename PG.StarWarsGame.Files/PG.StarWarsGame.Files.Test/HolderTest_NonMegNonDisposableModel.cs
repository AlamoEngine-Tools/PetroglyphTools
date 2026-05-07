using System;
using Xunit;

namespace PG.StarWarsGame.Files.Test;

public class HolderTest_NonMegNonDisposableModel : PetroglyphFileHolderTest<object, TestFileInfo, TestFileHolder<object, TestFileInfo>>
{
    protected override object CreateModel()
    {
        return new DisposableModel();
    }

    protected override TestFileInfo CreateFileInfo(string path, bool inMeg = false)
    {
        if (inMeg) 
            Assert.Fail();
        return new TestFileInfo { FilePath = path };
    }

    protected override TestFileHolder<object, TestFileInfo> CreateFileHolder(object model, TestFileInfo fileInfo)
    {
        return new TestFileHolder<object, TestFileInfo>(model, fileInfo, ServiceProvider);
    }

    [Fact]
    public void Ctor_ThrowsArgumentNullException()
    {
        var model = CreateModel();
        var fileInfo = CreateFileInfo(DefaultFileName);
        Assert.Throws<ArgumentNullException>(() => new TestFileHolder<object, TestFileInfo>(model, fileInfo, null!));
        Assert.Throws<ArgumentNullException>(() => new TestFileHolder<object, TestFileInfo>(model, null!, ServiceProvider));
        Assert.Throws<ArgumentNullException>(() => new TestFileHolder<object, TestFileInfo>(null!, fileInfo, ServiceProvider));
    }
}