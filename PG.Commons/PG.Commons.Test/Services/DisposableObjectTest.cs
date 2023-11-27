using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using PG.Commons.DataTypes;

namespace PG.Commons.Test.Services;

[TestClass]
public class DisposableObjectTest
{
    [TestMethod]
    public void DisposeTest()
    {
        var disposable = new Mock<DisposableObject>
        {
            CallBase = true
        };

        Assert.IsFalse(disposable.Object.IsDisposed);

        disposable.Object.Dispose();

        disposable.Protected().Verify("DisposeManagedResources", Times.Once());
        disposable.Protected().Verify("DisposeNativeResources", Times.Once());

        Assert.IsTrue(disposable.Object.IsDisposed);

        // Disposing again
        disposable.Object.Dispose();

        Assert.IsTrue(disposable.Object.IsDisposed);

        disposable.Protected().Verify("DisposeManagedResources", Times.Once());
        disposable.Protected().Verify("DisposeNativeResources", Times.Once());

    }
}