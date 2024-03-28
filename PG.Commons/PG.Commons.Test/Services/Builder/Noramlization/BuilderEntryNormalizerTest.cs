using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PG.Commons.Services.Builder.Normalization;
using Xunit;

namespace PG.Commons.Test.Services.Builder.Noramlization;

public class BuilderEntryNormalizerTest_String : BuilderEntryNormalizerTest<string>
{
    protected override string CreateT()
    {
        return "";
    }

    protected override string CreateTNormalized()
    {
        return "123";
    }
}

public class BuilderEntryNormalizerTest_Int32 : BuilderEntryNormalizerTest<int>
{
    protected override int CreateT()
    {
        return 0;
    }

    protected override int CreateTNormalized()
    {
        return 1;
    }
}

public abstract class BuilderEntryNormalizerTest<T> where T : IEquatable<T>
{
    protected abstract T CreateT();
    protected abstract T CreateTNormalized();

    [Fact]
    public void TestNormalize_Fails()
    {
        var normalizer = new Mock<BuilderEntryNormalizerBase<T>>(new ServiceCollection().BuildServiceProvider());

        var t = CreateT();

        normalizer.Setup(n => n.Normalize(It.IsAny<T>()))
            .Callback((T tt) =>
            {
                Assert.Equal(tt, CreateT());
            })
            .Throws<Exception>(() => new InvalidOperationException("Test"));

        var result = normalizer.Object.TryNormalize(ref t, out var message);
        Assert.False(result);
        Assert.Equal("Test", message);
        Assert.Throws<InvalidOperationException>(() => normalizer.Object.Normalize(CreateT()));
    }

    [Fact]
    public void TestNormalize_Success()
    {
        var normalizer = new Mock<BuilderEntryNormalizerBase<T>>(new ServiceCollection().BuildServiceProvider());

        var t = CreateT();

        normalizer.Setup(n => n.Normalize(It.IsAny<T>()))
            .Callback((T tt) =>
            {
                Assert.Equal(tt, CreateT());
            })
            .Returns(CreateTNormalized());

        var result = normalizer.Object.TryNormalize(ref t, out var message);
        Assert.True(result);
        Assert.Null(message);

        Assert.Equal(CreateTNormalized(), normalizer.Object.Normalize(CreateT()));
    }
}