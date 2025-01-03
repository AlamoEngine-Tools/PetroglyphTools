using System;
using System.IO;
using System.IO.Abstractions;
using AnakinRaW.CommonUtilities;
using Microsoft.Extensions.DependencyInjection;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.Test;

public class PetroglyphFileHolderTest
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MockFileSystem _fileSystem = new();

    public PetroglyphFileHolderTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        _serviceProvider = sc.BuildServiceProvider();
    }

    [Fact]
    public void Ctor_SetupProperties()
    {
        var model = new object();

        _fileSystem.Initialize().WithFile("test");

        var param = new TestParam { FilePath = "test" };
        var holder = new TestFileHolder(model, param, _serviceProvider);

        Assert.Same(model, holder.Content);
        Assert.NotSame(holder.FileInformation, param);

        Assert.Equal(_fileSystem.Path.GetFullPath("test"), holder.FilePath);
        Assert.Equal(_fileSystem.Path.GetDirectoryName(_fileSystem.Path.GetFullPath("test")), holder.Directory);
        Assert.Same(_serviceProvider, holder.Services);
        Assert.NotNull(holder.Logger);
    }

    [Theory]
    [InlineData("test", true)]
    [InlineData("path/test", true)]
    [InlineData("test", false)]
    [InlineData("path/test", false)]
    public void Ctor_SetupProperties_MegSupport(string path, bool inMeg)
    {
        var model = new object();

        if (!inMeg)
            _fileSystem.Initialize().WithFile(path);

        var param = new MegTestParam { FilePath = path, IsInsideMeg = inMeg };
        Assert.Equal(inMeg, param.IsInsideMeg);

        var holder = new TestFileHolder(model, param, _serviceProvider);

        Assert.Same(model, holder.Content);
        Assert.Same(model, ((IPetroglyphFileHolder)holder).Content);
        Assert.NotSame(holder.FileInformation, param);
        Assert.NotSame(((IPetroglyphFileHolder)holder).FileInformation, param);

        if (inMeg)
        {
            Assert.Equal(param.FilePath, holder.FilePath);
            Assert.Equal(_fileSystem.Path.GetDirectoryName(path), holder.Directory);
            Assert.NotNull(holder.Directory);

            Assert.Equal(holder.FileInformation, param);
        }
        else
        {
            Assert.Equal(_fileSystem.Path.GetFullPath(path), holder.FilePath);
            Assert.Equal(_fileSystem.Path.GetDirectoryName(_fileSystem.Path.GetFullPath(path)), holder.Directory);

            Assert.NotEqual(holder.FileInformation, param);
        }

        Assert.Same(_serviceProvider, holder.Services);
    }

    [PlatformSpecificTheory(TestPlatformIdentifier.Linux)]
    [InlineData("/   ", "   ", "/", "/   ")]
    [InlineData("./   ", "   ", "/", "/   ")]
    //[InlineData("   ", "   ", "/", "/   ")]  // Currently not possible due to https://github.com/TestableIO/System.IO.Abstractions/issues/1070
    public void Test_PassingFileNames_Whitespace_Linux(string filePath, string? expectedFileName, string expectedDirectory, string expectedFullPath)
    {
        var model = new object();

        _fileSystem.Initialize().WithFile(filePath);

        var holder = new TestFileHolder(model, new TestParam { FilePath = filePath }, _serviceProvider);

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
       var model = new object();
       

        _fileSystem.Initialize().WithFile(filePath);

        var fi = _fileSystem.FileInfo.New(filePath);
        var e = fi.Exists;

        var holder = new TestFileHolder(model, new TestParam { FilePath = filePath }, _serviceProvider);

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
        var model = new object();
       
        _fileSystem.Initialize().WithFile(filePath);

        var holder = new TestFileHolder(model, new TestParam { FilePath = filePath }, _serviceProvider);

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
        var model = new object();
        Assert.Throws<ArgumentNullException>(() => new TestFileHolder(model, new TestParam { FilePath = "test" }, null!));
        Assert.Throws<ArgumentNullException>(() => new TestFileHolder(model, null!, _serviceProvider));
        Assert.Throws<ArgumentNullException>(() => new TestFileHolder(null!, new TestParam { FilePath = "test" }, _serviceProvider));
    }

    [PlatformSpecificTheory(TestPlatformIdentifier.Windows)]
    [InlineData("   ", typeof(ArgumentException))]
    public void Test_Ctor_InvalidPath_Whitespace_Windows_Throws(string path, Type type)
    {
        var model = new object();
        Assert.Throws(type, () => new TestFileHolder(model, new TestParam { FilePath = path }, _serviceProvider));
    }

    [Theory]
    [InlineData("dir/")]
    [InlineData("")]
    [InlineData("..")]
    [InlineData(".")]
    public void Test_Ctor_InvalidPaths_Throws(string path)
    {
        var model = new object();
        Assert.Throws<ArgumentException>(() => new TestFileHolder(model, new TestParam { FilePath = path }, _serviceProvider));
    }

    [Fact]
    public void Test_Ctor_FileNotFound_Throws()
    {
        var model = new object();

        Assert.Throws<FileNotFoundException>(() =>
            new TestFileHolder(model, new TestParam { FilePath = "notfound.txt" }, _serviceProvider));

        Assert.Throws<FileNotFoundException>(() =>
            new TestFileHolder(model, new MegTestParam { FilePath = "notfound.txt", IsInsideMeg = false }, _serviceProvider));

        ExceptionUtilities.AssertDoesNotThrowException(() =>
            new TestFileHolder(model, new MegTestParam { FilePath = "notfound.txt", IsInsideMeg = true }, _serviceProvider));
    }

    [Fact]
    public void Test_Dispose()
    {
        var model = new DisposableModel();

        _fileSystem.Initialize().WithFile("test");

        var disposableParam = new DisposableTestParam { FilePath = "test" };
        var holder = new TestFileHolder(model, disposableParam, _serviceProvider);

        holder.Dispose();
        Assert.False(disposableParam.IsDisposed);
        Assert.Throws<ObjectDisposedException>(() => holder.FileInformation);

        Assert.True(model.IsDisposed);
    }

    [Fact]
    public void Test_FileInformation()
    {
        var model = new DisposableModel();

        _fileSystem.Initialize().WithFile("test");

        var disposableParam = new DisposableTestParam { FilePath = "test" };
        var holder = new TestFileHolder(model, disposableParam, _serviceProvider);

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