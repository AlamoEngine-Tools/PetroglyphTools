using System;
using System.IO;
using System.IO.Abstractions;
using AnakinRaW.CommonUtilities;
using Moq;
using PG.Commons.Files;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.Commons.Test.Files;

public class PetroglyphFileHolderTest
{
    [Fact]
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

        var param = new TestParam { FilePath = "test" };
        var holder = new TestFileHolder(model, param, sp.Object);

        Assert.Same(model, holder.Content);
        Assert.NotSame(holder.FileInformation, param);

        Assert.Equal(fs.Path.GetFullPath("test"), holder.FilePath);
        Assert.Equal(fs.Path.GetDirectoryName(fs.Path.GetFullPath("test")), holder.Directory);
        Assert.Same(sp.Object, holder.Services);
    }

    [Theory]
    [InlineData("test", true)]
    [InlineData("path/test", true)]
    [InlineData("test", false)]
    [InlineData("path/test", false)]
    public void Test_Ctor_SetupProperties_MegSupport(string path, bool inMeg)
    {
        var fs = new MockFileSystem();
        var model = new object();
        var loggerMock = new Mock<ILogger>();
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        loggerFactoryMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(ILoggerFactory))).Returns(loggerFactoryMock.Object);
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);


        if (!inMeg)
            fs.Initialize().WithFile(path);

        var param = new MegTestParam { FilePath = path, IsInsideMeg = inMeg };
        Assert.Equal(inMeg, param.IsInsideMeg);

        var holder = new TestFileHolder(model, param, sp.Object);

        Assert.Same(model, holder.Content);
        Assert.NotSame(holder.FileInformation, param);

        if (inMeg)
        {
            Assert.Equal(param.FilePath, holder.FilePath);
            Assert.Equal(fs.Path.GetDirectoryName(path), holder.Directory);
            Assert.NotNull(holder.Directory);

            Assert.Equal(holder.FileInformation, param);
        }
        else
        {
            Assert.Equal(fs.Path.GetFullPath(path), holder.FilePath);
            Assert.Equal(fs.Path.GetDirectoryName(fs.Path.GetFullPath(path)), holder.Directory);

            Assert.NotEqual(holder.FileInformation, param);
        }

        Assert.Same(sp.Object, holder.Services);
    }

    [PlatformSpecificTheory(TestPlatformIdentifier.Linux)]
    [InlineData("/   ", "   ", "/", "/   ")]
    [InlineData("./   ", "   ", "/", "/   ")]
    //[InlineData("   ", "   ", "/", "/   ")]  // Currently not possible due to https://github.com/TestableIO/System.IO.Abstractions/issues/1070
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
            Assert.Equal(expectedFileName, holder.FileName);
            Assert.Equal(expectedDirectory, holder.Directory);
            Assert.Equal(expectedFullPath, holder.FilePath);
        }
    }

    [PlatformSpecificTheory(TestPlatformIdentifier.Windows)]
    [InlineData("test.txt", "test.txt", "C:\\", "C:\\test.txt")]
    [InlineData("./test", "test", "C:\\", "C:\\test")]
    [InlineData("a/../test", "test", "C:\\", "C:\\test")]
    [InlineData("üöä", "üöä", "C:\\", "C:\\üöä")]
    [InlineData("a/b", "b", "C:\\a", "C:\\a\\b")]
#if NET
    [InlineData("test/\u00A0", "\u00A0", "C:\\test", "C:\\test\\\u00A0")]
#endif
    //[InlineData("\u00A0", "\u00A0", "C:\\\u00A0", "C:\\u00A0")] // Currently not possible due to https://github.com/TestableIO/System.IO.Abstractions/issues/1070
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
            Assert.Equal(expectedFileName, holder.FileName);
            Assert.Equal(expectedDirectory, holder.Directory);
            Assert.Equal(expectedFilePath, holder.FilePath);
        }
    }

    [PlatformSpecificTheory(TestPlatformIdentifier.Linux)]
    [InlineData("test.txt", "test.txt", "/", "/test.txt")]
    [InlineData("./test", "test", "/", "/test")]
    [InlineData("a/../test", "test", "/", "/test")]
    [InlineData("üöä", "üöä", "/", "/üöä")]
    [InlineData("a/b", "b", "/a", "/a/b")]
    [InlineData("test/\u00A0", "\u00A0", "/test", "/test/\u00A0")]
   // [InlineData("\u00A0", "\u00A0", "/\u00A0", "/\u00A0")] // Currently not possible due to https://github.com/TestableIO/System.IO.Abstractions/issues/1070
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
            Assert.Equal(expectedFileName, holder.FileName);
            Assert.Equal(expectedDirectory, holder.Directory);
            Assert.Equal(expectedFilePath, holder.FilePath);
        }
    }

    [Fact]
    public void Test_Ctor_ThrowsArgumentNullException()
    {
        var fs = new MockFileSystem();
        
        var model = new object();
        IServiceProvider sp = null!;

        Assert.Throws<ArgumentNullException>(() => new TestFileHolder(model, new TestParam { FilePath = "test" }, sp));

        var spMock = new Mock<IServiceProvider>();
        spMock.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        Assert.Throws<ArgumentNullException>(() => new TestFileHolder(model, null!, spMock.Object));

        Assert.Throws<ArgumentNullException>(() => new TestFileHolder(null!, new TestParam { FilePath = "test" }, spMock.Object));
    }

    [Fact]
    public void Test_Ctor_ThrowsNoFileSystem_Throws()
    { 
        var model = new object();
        var spMock = new Mock<IServiceProvider>();
        spMock.Setup(s => s.GetService(typeof(IFileSystem))).Returns((IFileSystem)null!);
        Assert.Throws<InvalidOperationException>(() => new TestFileHolder(model, new TestParam { FilePath = "test" }, spMock.Object));
    }
    
    [PlatformSpecificTheory(TestPlatformIdentifier.Windows)]
    [InlineData("   ", typeof(ArgumentException))]
    public void Test_Ctor_InvalidPath_Whitespace_Windows_Throws(string path, Type type)
    {
        var fs = new MockFileSystem();
        var model = new object();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);
        Assert.Throws(type, () => new TestFileHolder(model, new TestParam { FilePath = path }, sp.Object));
    }

    [Theory]
    [InlineData("dir/")]
    [InlineData("")]
    [InlineData("..")]
    [InlineData(".")]
    public void Test_Ctor_InvalidPaths_Throws(string path)
    {
        var fs = new MockFileSystem();
        var model = new object();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);
        
        Assert.Throws<ArgumentException>(() => new TestFileHolder(model, new TestParam { FilePath = path }, sp.Object));
    }

    [Fact]
    public void Test_Ctor_FileNotFound_Throws()
    {
        var fs = new MockFileSystem();
        var model = new object();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        Assert.Throws<FileNotFoundException>(() =>
            new TestFileHolder(model, new TestParam { FilePath = "notfound.txt" }, sp.Object));

        Assert.Throws<FileNotFoundException>(() =>
            new TestFileHolder(model, new MegTestParam { FilePath = "notfound.txt", IsInsideMeg = false}, sp.Object));

        ExceptionUtilities.AssertDoesNotThrowException(() =>
            new TestFileHolder(model, new MegTestParam { FilePath = "notfound.txt", IsInsideMeg = true }, sp.Object));
    }

    [Fact]
    public void Test_Ctor_NullLogger()
    {
        var fs = new MockFileSystem();
        var model = new object();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        fs.Initialize().WithFile("test");

        var holder = new TestFileHolder(model, new TestParam { FilePath = "test" }, sp.Object);
        Assert.Equal(NullLogger.Instance, holder.Logger);
    }

    [Fact]
    public void Test_Dispose()
    {
        var fs = new MockFileSystem();
        var model = new DisposableModel();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        fs.Initialize().WithFile("test");

        var disposableParam = new DisposableTestParam { FilePath = "test" };
        var holder = new TestFileHolder(model, disposableParam, sp.Object);

        holder.Dispose();
        Assert.False(disposableParam.IsDisposed);
        Assert.Throws<ObjectDisposedException>(() => holder.FileInformation);

        Assert.True(model.IsDisposed);
    }

    [Fact]
    public void Test_FileInformation()
    {
        var fs = new MockFileSystem();
        var model = new DisposableModel();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        fs.Initialize().WithFile("test");

        var disposableParam = new DisposableTestParam { FilePath = "test" };
        var holder = new TestFileHolder(model, disposableParam, sp.Object);

        disposableParam.Dispose();

        Assert.False(((DisposableTestParam)holder.FileInformation).IsDisposed);

        var a = holder.FileInformation;
        var b = holder.FileInformation;
        Assert.NotSame(a, b);
    }

    private record TestParam : PetroglyphFileInformation;

    private record MegTestParam : PetroglyphMegPackableFileInformation;

    private record DisposableTestParam : PetroglyphMegPackableFileInformation
    {
        public bool IsDisposed { get; private set; }

        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
            base.Dispose(disposing);
        }
    }

    private class DisposableModel : DisposableObject;


    private class TestFileHolder(object model, PetroglyphFileInformation fileInformation, IServiceProvider serviceProvider)
        : PetroglyphFileHolder<object, PetroglyphFileInformation>(model, fileInformation, serviceProvider);
}