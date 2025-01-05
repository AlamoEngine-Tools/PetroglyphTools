using System;
using Xunit;

namespace PG.StarWarsGame.Files.Test;

public class HolderTest_NonMegNonDisposableModel : PetroglyphFileHolderTest<object, TestParam, TestFileHolder<object, TestParam>>
{
    protected override object CreateModel()
    {
        return new DisposableModel();
    }

    protected override TestParam CreateFileInfo(string path, bool inMeg = false)
    {
        if (inMeg) 
            Assert.Fail();
        return new TestParam { FilePath = path };
    }

    protected override TestFileHolder<object, TestParam> CreateFileHolder(object model, TestParam fileInfo)
    {
        return new TestFileHolder<object, TestParam>(model, fileInfo, ServiceProvider);
    }

    [Fact]
    public void Test_Ctor_ThrowsArgumentNullException()
    {
        var model = CreateModel();
        var fileInfo = CreateFileInfo(DefaultFileName);
        Assert.Throws<ArgumentNullException>(() => new TestFileHolder<object, TestParam>(model, fileInfo, null!));
        Assert.Throws<ArgumentNullException>(() => new TestFileHolder<object, TestParam>(model, null!, ServiceProvider));
        Assert.Throws<ArgumentNullException>(() => new TestFileHolder<object, TestParam>(null!, fileInfo, ServiceProvider));
    }
}