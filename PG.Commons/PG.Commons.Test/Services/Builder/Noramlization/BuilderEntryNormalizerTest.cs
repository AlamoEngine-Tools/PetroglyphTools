using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Services.Builder.Normalization;

namespace PG.Commons.Test.Services.Builder.Noramlization;

[TestClass]
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

[TestClass]
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

    [TestMethod]
    public void TestNormalize_Fails()
    {
        var normalizer = new Mock<BuilderEntryNormalizerBase<T>>(new ServiceCollection().BuildServiceProvider());

        var t = CreateT();

        normalizer.Setup(n => n.Normalize(It.IsAny<T>()))
            .Callback((T tt) =>
            {
                Assert.AreEqual(tt, CreateT());
            })
            .Throws<Exception>(() => new InvalidOperationException("Test"));

        var result = normalizer.Object.TryNormalize(ref t, out var message);
        Assert.IsFalse(result);
        Assert.AreEqual("Test", message);
        Assert.ThrowsException<InvalidOperationException>(() => normalizer.Object.Normalize(CreateT()));
    }

    [TestMethod]
    public void TestNormalize_Success()
    {
        var normalizer = new Mock<BuilderEntryNormalizerBase<T>>(new ServiceCollection().BuildServiceProvider());

        var t = CreateT();

        normalizer.Setup(n => n.Normalize(It.IsAny<T>()))
            .Callback((T tt) =>
            {
                Assert.AreEqual(tt, CreateT());
            })
            .Returns(CreateTNormalized());

        var result = normalizer.Object.TryNormalize(ref t, out var message);
        Assert.IsTrue(result);
        Assert.IsNull(message);

        Assert.AreEqual(CreateTNormalized(), normalizer.Object.Normalize(CreateT()));
    }
}