using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Files;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PG.Testing;

namespace PG.Commons.Test.Files;

[TestClass]
public class FileHolderBaseTest
{
    [TestMethod]
    public void Test_Ctor_SetupProperties()
    {
        var fs = new MockFileSystem();
        var model = new object();
        var loggerMock = new Mock<ILogger>();
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        loggerFactoryMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(ILoggerFactory))).Returns(loggerFactoryMock.Object);
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        var holder = new TestFileHolder(model, new TestParam("test"), sp.Object);

        Assert.AreSame(model, holder.Content);
        Assert.AreEqual("test", holder.FilePath);
        Assert.AreEqual(string.Empty, holder.Directory);
        Assert.IsNotNull(holder.FileType);
        Assert.AreSame(sp.Object, holder.Services);
    }

    [PlatformSpecificTestMethod(TestConstants.PLATFORM_LINUX)]
    [DataRow("   ", "   ", "")]
    public void Test_PassingFileNames_Whitespace_Linux(string filePath, string? expectedFileName, string expectedDirectory)
    {
        var fs = new MockFileSystem();
        var model = new object();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        var holder = new TestFileHolder(model, new TestParam(filePath), sp.Object);

        if (expectedFileName is not null)
        {
            Assert.AreEqual(expectedFileName, holder.FileName);
            Assert.AreEqual(expectedDirectory, holder.Directory);
            Assert.AreEqual(filePath, holder.FilePath);
        }
    }

    [TestMethod]
    [DataRow("test", "test", "")]
    [DataRow("..", null, null)]
    [DataRow(".", null, null)]
    [DataRow("üöä", "üöä", "")]
    [DataRow("a/b", "b", "a")]
    [DataRow("test/\u00A0", "\u00A0", "test")]
    public void Test_PassingFileNames(string filePath, string? expectedFileName, string expectedDirectory)
    {
        var fs = new MockFileSystem();
        var model = new object();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);
        
        var holder = new TestFileHolder(model, new TestParam(filePath), sp.Object);

        if (expectedFileName is not null)
        {
            Assert.AreEqual(expectedFileName, holder.FileName);
            Assert.AreEqual(expectedDirectory, holder.Directory);
            Assert.AreEqual(filePath, holder.FilePath);
        }
    }

    [TestMethod]
    public void Test_Ctor_ThrowsArgumentNullException()
    {
        var fs = new MockFileSystem();
        
        var model = new object();
        IServiceProvider sp = null!;

        Assert.ThrowsException<ArgumentNullException>(() => new TestFileHolder(model, new TestParam("test"), sp));

        var spMock = new Mock<IServiceProvider>();
        spMock.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        Assert.ThrowsException<ArgumentNullException>(() => new TestFileHolder(model, null!, spMock.Object));

        Assert.ThrowsException<ArgumentNullException>(() => new TestFileHolder(null!, new TestParam("test"), spMock.Object));
    }

    [TestMethod]
    public void Test_Ctor_ThrowsNoFileSystem_Throws()
    { 
        var model = new object();
        var spMock = new Mock<IServiceProvider>();
        spMock.Setup(s => s.GetService(typeof(IFileSystem))).Returns((IFileSystem)null!);
        Assert.ThrowsException<InvalidOperationException>(() => new TestFileHolder(model, new TestParam("test"), spMock.Object));
    }

    [TestMethod]
    [DataRow("", typeof(ArgumentException))]
    [DataRow(null!, typeof(ArgumentNullException))]

    public void Test_Ctor_InvalidPath_EmptyOrNull_Throws(string path, Type type)
    {
        var fs = new MockFileSystem();
        var model = new object();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);
        ExceptionUtilities.AssertThrowsException(type, () => new TestFileHolder(model, new TestParam(path), sp.Object));
    }

    [PlatformSpecificTestMethod(TestConstants.PLATFORM_WINDOWS)]
#if NET
    [DataRow("   ", typeof(InvalidOperationException))]
#elif NETFRAMEWORK
    [DataRow("   ", typeof(ArgumentException))]
#endif
    public void Test_Ctor_InvalidPath_Whitespace_Windows_Throws(string path, Type type)
    {
        var fs = new MockFileSystem();
        var model = new object();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);
        ExceptionUtilities.AssertThrowsException(type, () => new TestFileHolder(model, new TestParam(path), sp.Object));
    }

    [TestMethod]
    public void Test_Ctor_NullLogger()
    {
        var fs = new MockFileSystem();
        var model = new object();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        var holder = new TestFileHolder(model, new TestParam("test"), sp.Object);
        Assert.AreEqual(NullLogger.Instance, holder.Logger);
    }

    private class TestParam(string filePath) : IFileHolderParam
    {
        public string FilePath { get; } = filePath;
    }

    private struct TestFileType : IAlamoFileType
    {
        public FileType Type { get; }
        public string FileExtension { get; }
    }

    private class TestFileHolder(object model, TestParam param, IServiceProvider serviceProvider)
        : FileHolderBase<TestParam, object, TestFileType>(model, param, serviceProvider);
}