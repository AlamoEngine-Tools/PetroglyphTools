﻿using System;
using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Services;
using Testably.Abstractions.Testing;

namespace PG.Commons.Test.Services;

[TestClass]
public class ServiceBaseTest
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_Ctor_ThrowsNullArg()
    {
        _ = new MyService(null!);
    }

    [TestMethod]
    public void Test_Ctor_SetupProperties()
    {
        var fs = new MockFileSystem();
        var loggerMock = new Mock<ILogger>();
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        loggerFactoryMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);
        var spMock = new Mock<IServiceProvider>();
        spMock.Setup(s => s.GetService(typeof(ILoggerFactory))).Returns(loggerFactoryMock.Object);
        spMock.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);
        
        var service = new MyService(spMock.Object);
        Assert.AreEqual(spMock.Object, service.Services);
        Assert.AreEqual(loggerMock.Object, service.Logger);
        Assert.AreEqual(fs, service.FileSystem);
    }

    [TestMethod]
    public void Test_Ctor_NullLogger()
    {
        var fs = new MockFileSystem();
        var spMock = new Mock<IServiceProvider>();
        spMock.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);
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