using System;
using System.IO;
using System.IO.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Files;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PG.Testing;
using Testably.Abstractions.Testing;

namespace PG.Commons.Test.Files;

[TestClass]
public class PetroglyphFileHolderTest
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

        fs.Initialize().WithFile("test");

        var holder = new TestFileHolder(model, new TestParam { FilePath = "test" }, sp.Object);

        Assert.AreSame(model, holder.Content);
        Assert.AreEqual(fs.Path.GetFullPath("test"), holder.FilePath);
        Assert.AreEqual(fs.Path.GetDirectoryName(fs.Path.GetFullPath("test")), holder.Directory);
        Assert.AreSame(sp.Object, holder.Services);
    }

    [PlatformSpecificTestMethod(TestPlatformIdentifier.Linux)]
    [DataRow("/   ", "   ", "/", "/   ")]
    [DataRow("./   ", "   ", "/", "/   ")]
    //[DataRow("   ", "   ", "/", "/   ")]  // Currently not possible due to https://github.com/TestableIO/System.IO.Abstractions/issues/1070
    public void Test_PassingFileNames_Whitespace_Linux(string filePath, string? expectedFileName, string expectedDirectory, string expectedFullPath)
    {
        var fs = new MockFileSystem();

        var model = new object();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        fs.Initialize().WithFile(filePath);

        var holder = new TestFileHolder(model, new TestParam { FilePath = filePath }, sp.Object);

        if (expectedFileName is not null)
        {
            Assert.AreEqual(expectedFileName, holder.FileName);
            Assert.AreEqual(expectedDirectory, holder.Directory);
            Assert.AreEqual(expectedFullPath, holder.FilePath);
        }
    }

    [PlatformSpecificTestMethod(TestPlatformIdentifier.Windows)]
    [DataRow("test.txt", "test.txt", "C:\\", "C:\\test.txt")]
    [DataRow("./test", "test", "C:\\", "C:\\test")]
    [DataRow("a/../test", "test", "C:\\", "C:\\test")]
    [DataRow("üöä", "üöä", "C:\\", "C:\\üöä")]
    [DataRow("a/b", "b", "C:\\a", "C:\\a\\b")]
#if NET
    [DataRow("test/\u00A0", "\u00A0", "C:\\test", "C:\\test\\\u00A0")]
#endif
    //[DataRow("\u00A0", "\u00A0", "C:\\\u00A0", "C:\\u00A0")] // Currently not possible due to https://github.com/TestableIO/System.IO.Abstractions/issues/1070
    public void Test_PassingFileNames_Windows(string filePath, string? expectedFileName, string expectedDirectory, string expectedFilePath)
    {
        var fs = new MockFileSystem();
        var model = new object();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        fs.Initialize().WithFile(filePath);

        var fi = fs.FileInfo.New(filePath);
        var e = fi.Exists;

        var holder = new TestFileHolder(model, new TestParam { FilePath = filePath }, sp.Object);

        if (expectedFileName is not null)
        {
            Assert.AreEqual(expectedFileName, holder.FileName);
            Assert.AreEqual(expectedDirectory, holder.Directory);
            Assert.AreEqual(expectedFilePath, holder.FilePath);
        }
    }

    [PlatformSpecificTestMethod(TestPlatformIdentifier.Linux)]
    [DataRow("test.txt", "test.txt", "/", "/test.txt")]
    [DataRow("./test", "test", "/", "/test")]
    [DataRow("a/../test", "test", "/", "/test")]
    [DataRow("üöä", "üöä", "/", "/üöä")]
    [DataRow("a/b", "b", "/a", "/a/b")]
    [DataRow("test/\u00A0", "\u00A0", "/test", "/test/\u00A0")]
    //[DataRow("\u00A0", "\u00A0", "/\u00A0", "/\u00A0")] // Currently not possible due to https://github.com/TestableIO/System.IO.Abstractions/issues/1070
    public void Test_PassingFileNames_Linux(string filePath, string? expectedFileName, string expectedDirectory, string expectedFilePath)
    {
        var fs = new MockFileSystem();
        var model = new object();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        fs.Initialize().WithFile(filePath);

        var holder = new TestFileHolder(model, new TestParam { FilePath = filePath }, sp.Object);

        if (expectedFileName is not null)
        {
            Assert.AreEqual(expectedFileName, holder.FileName);
            Assert.AreEqual(expectedDirectory, holder.Directory);
            Assert.AreEqual(expectedFilePath, holder.FilePath);
        }
    }

    [TestMethod]
    public void Test_Ctor_ThrowsArgumentNullException()
    {
        var fs = new MockFileSystem();
        
        var model = new object();
        IServiceProvider sp = null!;

        Assert.ThrowsException<ArgumentNullException>(() => new TestFileHolder(model, new TestParam { FilePath = "test" }, sp));

        var spMock = new Mock<IServiceProvider>();
        spMock.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        Assert.ThrowsException<ArgumentNullException>(() => new TestFileHolder(model, null!, spMock.Object));

        Assert.ThrowsException<ArgumentNullException>(() => new TestFileHolder(null!, new TestParam { FilePath = "test" }, spMock.Object));
    }

    [TestMethod]
    public void Test_Ctor_ThrowsNoFileSystem_Throws()
    { 
        var model = new object();
        var spMock = new Mock<IServiceProvider>();
        spMock.Setup(s => s.GetService(typeof(IFileSystem))).Returns((IFileSystem)null!);
        Assert.ThrowsException<InvalidOperationException>(() => new TestFileHolder(model, new TestParam { FilePath = "test" }, spMock.Object));
    }
    
    [PlatformSpecificTestMethod(TestPlatformIdentifier.Windows)]
    [DataRow("   ", typeof(ArgumentException))]
    public void Test_Ctor_InvalidPath_Whitespace_Windows_Throws(string path, Type type)
    {
        var fs = new MockFileSystem();
        var model = new object();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);
        ExceptionUtilities.AssertThrowsException(type, () => new TestFileHolder(model, new TestParam { FilePath = path }, sp.Object));
    }

    [TestMethod]
    [DataRow("dir/")]
    [DataRow("")]
    [DataRow("..")]
    [DataRow(".")]
    public void Test_Ctor_InvalidPaths_Throws(string path)
    {
        var fs = new MockFileSystem();
        var model = new object();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);
        
        Assert.ThrowsException<ArgumentException>(() => new TestFileHolder(model, new TestParam { FilePath = path }, sp.Object));
    }

    [TestMethod]
    public void Test_Ctor_FileNotFound_Throws()
    {
        var fs = new MockFileSystem();
        var model = new object();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        Assert.ThrowsException<FileNotFoundException>(() => new TestFileHolder(model, new TestParam { FilePath = "notfound.txt" }, sp.Object));
    }

    [TestMethod]
    public void Test_Ctor_NullLogger()
    {
        var fs = new MockFileSystem();
        var model = new object();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        fs.Initialize().WithFile("test");

        var holder = new TestFileHolder(model, new TestParam { FilePath = "test" }, sp.Object);
        Assert.AreEqual(NullLogger.Instance, holder.Logger);
    }

    private record TestParam : PetroglyphFileInformation;

    private class TestFileHolder(object model, TestParam fileInformation, IServiceProvider serviceProvider)
        : PetroglyphFileHolder<object, TestParam>(model, fileInformation, serviceProvider);
}