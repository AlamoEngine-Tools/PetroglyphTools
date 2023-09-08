using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Services;

namespace PG.Commons.Test.Services;

[TestClass]
public class ServiceBaseTest
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test__Ctor_ThrowsNullArg()
    {
        _ = new MyService(null!);
    }

    [TestMethod]
    public void Test__Ctor_SetupProperties()
    {
        var loggerMock = new Mock<ILogger>();
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        loggerFactoryMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);
        var spMock = new Mock<IServiceProvider>();
        spMock.Setup(s => s.GetService(typeof(ILoggerFactory))).Returns(loggerFactoryMock.Object);
        
        var service = new MyService(spMock.Object);
        Assert.AreEqual(spMock.Object, service.Services);
        Assert.AreEqual(loggerMock.Object, service.Logger);
    }

    [TestMethod]
    public void Test__Ctor_NullLogger()
    {
        var spMock = new Mock<IServiceProvider>();
        var service = new MyService(spMock.Object);
        Assert.AreEqual(NullLogger.Instance, service.Logger);
    }

    private class MyService : ServiceBase
    {
        public MyService(IServiceProvider services) : base(services)
        {
        }
    }
}