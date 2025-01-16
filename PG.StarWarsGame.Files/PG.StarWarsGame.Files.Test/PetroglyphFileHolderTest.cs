using System;
using System.IO;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.Test;

public abstract class PetroglyphFileHolderTest<TModel, TFileInfo, THolder> : CommonTestBase
    where TModel : class
    where TFileInfo : PetroglyphFileInformation
    where THolder : PetroglyphFileHolder<TModel, TFileInfo>
{
    protected virtual string DefaultFileName => "test.txt";

    protected abstract TModel CreateModel();

    protected abstract TFileInfo CreateFileInfo(string path, bool inMeg = false);

    protected abstract THolder CreateFileHolder(TModel model, TFileInfo fileInfo);


    [Fact]
    public void Ctor_SetupProperties()
    {
        var model = CreateModel();

        FileSystem.Initialize().WithFile(DefaultFileName);
        var param = CreateFileInfo(DefaultFileName);

        var holder = CreateFileHolder(model, param);

        Assert.Same(model, holder.Content);
        Assert.Same(model, ((IPetroglyphFileHolder)holder).Content);
        Assert.NotSame(param, holder.FileInformation);
        Assert.NotSame(param, ((IPetroglyphFileHolder)holder).FileInformation);
        Assert.Equal(FileSystem.Path.GetFullPath(param.FilePath), FileSystem.Path.GetFullPath(holder.FileInformation.FilePath));
        Assert.Equal(FileSystem.Path.GetFullPath(param.FilePath), FileSystem.Path.GetFullPath(((IPetroglyphFileHolder)holder).FileInformation.FilePath));

        Assert.Equal(FileSystem.Path.GetFullPath(DefaultFileName), holder.FilePath);
        Assert.Equal(FileSystem.Path.GetDirectoryName(FileSystem.Path.GetFullPath(DefaultFileName)), holder.Directory);
        Assert.Same(ServiceProvider, holder.Services);
        Assert.NotNull(holder.Logger);
    }

    [Theory]
    [InlineData("test", true)]
    [InlineData("path/test", true)]
    [InlineData("test", false)]
    [InlineData("path/test", false)]
    public void Ctor_SetupProperties_MegSupport(string path, bool inMeg)
    {
        if (!typeof(TFileInfo).IsAssignableFrom(typeof(PetroglyphMegPackableFileInformation)))
            return;

        var model = CreateModel();

        if (!inMeg)
            FileSystem.Initialize().WithFile(path);

        var param = CreateFileInfo(path, inMeg);
        Assert.Equal(inMeg, (param as PetroglyphMegPackableFileInformation)!.IsInsideMeg);

        var holder = CreateFileHolder(model, param);

        Assert.Same(model, holder.Content);
        Assert.Same(model, ((IPetroglyphFileHolder)holder).Content);
        Assert.NotSame(holder.FileInformation, param);
        Assert.NotSame(((IPetroglyphFileHolder)holder).FileInformation, param);

        if (inMeg)
        {
            Assert.Equal(param.FilePath, holder.FilePath);
            Assert.Equal(FileSystem.Path.GetDirectoryName(path), holder.Directory);
            Assert.NotNull(holder.Directory);

            Assert.Equal(holder.FileInformation, param);
        }
        else
        {
            Assert.Equal(FileSystem.Path.GetFullPath(path), holder.FilePath);
            Assert.Equal(FileSystem.Path.GetDirectoryName(FileSystem.Path.GetFullPath(path)), holder.Directory);

            Assert.NotEqual(holder.FileInformation, param);
        }

        Assert.Same(ServiceProvider, holder.Services);
    }

    [PlatformSpecificTheory(TestPlatformIdentifier.Linux)]
    [InlineData("/   ", "   ", "/", "/   ")]
    [InlineData("./   ", "   ", "/", "/   ")]
    //[InlineData("   ", "   ", "/", "/   ")]  // Currently not possible due to https://github.com/TestableIO/System.IO.Abstractions/issues/1070
    public void PassingFileNames_Whitespace_Linux(string filePath, string? expectedFileName, string expectedDirectory, string expectedFullPath)
    {
        var model = CreateModel();
        FileSystem.Initialize().WithFile(filePath);
        var holder = CreateFileHolder(model, CreateFileInfo(filePath));

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
    public void PassingFileNames_Windows(string filePath, string? expectedFileName, string expectedDirectory, string expectedFilePath)
    {
        var model = CreateModel();
        FileSystem.Initialize().WithFile(filePath);
        var holder = CreateFileHolder(model, CreateFileInfo(filePath));

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
    public void PassingFileNames_Linux(string filePath, string? expectedFileName, string expectedDirectory, string expectedFilePath)
    {
        var model = CreateModel();
        FileSystem.Initialize().WithFile(filePath);
        var holder = CreateFileHolder(model, CreateFileInfo(filePath));

        if (expectedFileName is not null)
        {
            Assert.Equal(expectedFileName, holder.FileName);
            Assert.Equal(expectedDirectory, holder.Directory);
            Assert.Equal(expectedFilePath, holder.FilePath);
        }
    }

    [PlatformSpecificTheory(TestPlatformIdentifier.Windows)]
    [InlineData("   ", typeof(ArgumentException))]
    public void Ctor_InvalidPath_Whitespace_Windows_Throws(string path, Type type)
    {
        var model = CreateModel();
        var fileInfo = CreateFileInfo(path);
        Assert.Throws(type, () => CreateFileHolder(model, fileInfo));
    }

    [Theory]
    [InlineData("dir/")]
    [InlineData("..")]
    [InlineData(".")]
    public void Ctor_InvalidPaths_Throws(string path)
    {
        var model = CreateModel();
        var fileInfo = CreateFileInfo(path);
        Assert.Throws<ArgumentException>(() => CreateFileHolder(model, fileInfo));
    }

    [Fact]
    public void Ctor_FileNotFound_Throws()
    {
        var model = CreateModel();

        Assert.Throws<FileNotFoundException>(() => CreateFileHolder(model, CreateFileInfo("notFound")));

        if (!typeof(TFileInfo).IsAssignableFrom(typeof(PetroglyphMegPackableFileInformation)))
            return;

        ExceptionUtilities.AssertDoesNotThrowException(() => CreateFileHolder(model, CreateFileInfo("notFound", true)));
    }

    [Fact]
    public void Dispose()
    {
        var model = CreateModel();

        FileSystem.Initialize().WithFile(DefaultFileName);

        var disposableParam = CreateFileInfo(DefaultFileName);
        var holder = CreateFileHolder(model, disposableParam);

        holder.Dispose();
        Assert.Throws<ObjectDisposedException>(() => holder.FileInformation);
    }

    [Fact]
    public void FileInformation_ReturnsCopy()
    {
        var model = CreateModel();

        FileSystem.Initialize().WithFile(DefaultFileName);
        var disposableParam = CreateFileInfo(DefaultFileName);
        var holder = CreateFileHolder(model, disposableParam);
        
        var a = holder.FileInformation;
        var b = holder.FileInformation;
        Assert.NotSame(a, b);
        Assert.Equal(a, b);
    }

}