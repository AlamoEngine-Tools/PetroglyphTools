using System;
using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PG.Commons.Services;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.Commons.Test.Services;

public class ServiceBaseTest
{
    [Fact]
    public void Test_Ctor_ThrowsNullArg()
    {
        Assert.Throws<ArgumentNullException>(() => new MyService(null!));
    }

    [Fact]
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
        Assert.Equal(spMock.Object, service.Services);
        Assert.Equal(loggerMock.Object, service.Logger);
        Assert.Equal(fs, service.FileSystem);
    }

    [Fact]
    public void Test_Ctor_NullLogger()
    {
        var fs = new MockFileSystem();
        var spMock = new Mock<IServiceProvider>();
        spMock.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);
        var service = new MyService(spMock.Object);
        Assert.Equal(NullLogger.Instance, service.Logger);
    }

    private class MyService(IServiceProvider services) : ServiceBase(services);
}