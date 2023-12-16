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
    public void Test__Ctor_SetupProperties()
    {
        var fs = new MockFileSystem();
        var model = new object();
        var loggerMock = new Mock<ILogger>();
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        loggerFactoryMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(ILoggerFactory))).Returns(loggerFactoryMock.Object);
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        var holder = new Mock<FileHolderBase<IFileHolderParam, object, TestFileType>>(model, new TestParam("test"), sp.Object).Object;

        Assert.AreSame(model, holder.Content);
        Assert.AreEqual("test", holder.FilePath);
        Assert.AreEqual(string.Empty, holder.Directory);
        Assert.IsNotNull(holder.FileType);
    }

    [TestMethod]
    [DataRow("test", "test", "")]
    [DataRow("..", null, null)]
    [DataRow(".", null, null)]
    [DataRow("üöä", "üöä", "")]
    [DataRow("a/b", "b", "a")]
    [DataRow("test/\u00A0", "\u00A0", "test")]
    public void Test__PassingFileNames(string filePath, string? expectedFileName, string expectedDirectory)
    {
        var fs = new FileSystem();
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
    public void Test__Ctor_ThrowsArgumentNullException()
    {
        var fs = new MockFileSystem();
        
        var model = new object();
        IServiceProvider sp = null!;


        ExceptionUtilities.AssertThrowsException_IgnoreTargetInvocationException<ArgumentNullException>(() => new Mock<FileHolderBase<IFileHolderParam, object, TestFileType>>(model, new TestParam("test"), sp).Object);

        var spMock = new Mock<IServiceProvider>();
        spMock.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        ExceptionUtilities.AssertThrowsException_IgnoreTargetInvocationException<ArgumentNullException>(() => new Mock<FileHolderBase<IFileHolderParam, object, TestFileType>>(model, null!, spMock.Object).Object);
    }

    [TestMethod]
    public void Test__Ctor_ThrowsNoFileSystem()
    { 
        var model = new object();
        var spMock = new Mock<IServiceProvider>();
        spMock.Setup(s => s.GetService(typeof(IFileSystem))).Returns((IFileSystem)null!);
        ExceptionUtilities.AssertThrowsException_IgnoreTargetInvocationException<InvalidOperationException>(() => new Mock<FileHolderBase<IFileHolderParam, object, TestFileType>>(model, new TestParam("test"), spMock.Object).Object);
    }

    [TestMethod]
    [DataRow("", typeof(ArgumentException))]
    [DataRow(null!, typeof(ArgumentNullException))]
#if NET
    [DataRow("   ", typeof(InvalidOperationException))]
#elif NETFRAMEWORK
    [DataRow("   ", typeof(ArgumentException))]
#endif

    public void Test__Ctor_InvalidPath(string path, Type type)
    {
        var fs = new MockFileSystem();
        var model = new object();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        ExceptionUtilities.AssertThrowsException_IgnoreTargetInvocationException(type, () => new Mock<FileHolderBase<IFileHolderParam, object, TestFileType>>(model, new TestParam(path), sp.Object).Object);
    }

    [TestMethod]
    public void Test__Ctor_NullLogger()
    {
        var fs = new MockFileSystem();
        var model = new object();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        var holder = new Mock<FileHolderBase<IFileHolderParam, object, TestFileType>>(model, new TestParam("test"), sp.Object).Object;

        Assert.AreEqual(NullLogger.Instance, holder.Logger);
    }

    private class TestParam : IFileHolderParam
    {
        public TestParam(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; }
    }

    private struct TestFileType : IAlamoFileType
    {
        public FileType Type { get; }
        public string FileExtension { get; }
    }

    private class TestFileHolder(object model, TestParam param, IServiceProvider serviceProvider)
        : FileHolderBase<TestParam, object, TestFileType>(model, param, serviceProvider);
}