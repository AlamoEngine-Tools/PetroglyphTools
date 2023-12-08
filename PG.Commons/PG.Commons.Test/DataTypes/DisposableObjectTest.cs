using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using PG.Commons.DataTypes;
using ExceptionUtilities = PG.Testing.ExceptionUtilities;

namespace PG.Commons.Test.DataTypes;

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

        var throwIfDisposed = disposable.Object.GetType().GetMethod("ThrowIfDisposed", BindingFlags.NonPublic | BindingFlags.Instance);

        ExceptionUtilities.AssertDoesNotThrowException(() => throwIfDisposed!.Invoke(disposable.Object, null));

        disposable.Object.Dispose();

        disposable.Protected().Verify("DisposeManagedResources", Times.Once());
        disposable.Protected().Verify("DisposeNativeResources", Times.Once());

        Assert.IsTrue(disposable.Object.IsDisposed);

        // Disposing again
        disposable.Object.Dispose();

        Assert.IsTrue(disposable.Object.IsDisposed);

        disposable.Protected().Verify("DisposeManagedResources", Times.Once());
        disposable.Protected().Verify("DisposeNativeResources", Times.Once());


        ExceptionUtilities.AssertThrowsException_IgnoreTargetInvocationException<ObjectDisposedException>(() => throwIfDisposed!.Invoke(disposable.Object, null));
    }
}