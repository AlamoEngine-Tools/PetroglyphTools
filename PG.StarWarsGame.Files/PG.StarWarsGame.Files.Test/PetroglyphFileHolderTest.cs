using PG.Testing;
using System;
using System.IO;
using System.Runtime.InteropServices;
using AnakinRaW.CommonUtilities.Testing.Attributes;
using AnakinRaW.CommonUtilities.Testing.Extensions;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.Test;

public abstract class PetroglyphFileHolderTest<TModel, TFileInfo, THolder> : PGTestBase
    where TModel : class
    where TFileInfo : PetroglyphFileInformation
    where THolder : PetroglyphFileHolder<TModel, TFileInfo>
{
    protected virtual string DefaultFileName => "test.txt";

    protected abstract TModel CreateModel();

    protected abstract TFileInfo CreateFileInfo(string path, bool inMeg = false);

    protected abstract THolder CreateFileHolder(TModel model, TFileInfo fileInfo);

    [Theory]
    [InlineData("test.txt")]
    [InlineData("./test")]
    [InlineData("a/../test")]
    [InlineData("üöä")]
    [InlineData("a/b")]
    public void Ctor_LocalFile_SetupProperties(string filePath)
    {
        var model = CreateModel();
        FileSystem.Initialize().WithFile(filePath);
        var param = CreateFileInfo(filePath);

        var holder = CreateFileHolder(model, param);

        var expectedFilePath = FileSystem.Path.GetFullPath(filePath);

        Assert.Same(model, holder.Content);
        Assert.Same(model, ((IPetroglyphFileHolder)holder).Content);
        Assert.Same(ServiceProvider, holder.Services);
        Assert.NotNull(holder.Logger);

        Assert.Equal(FileSystem.Path.GetFileName(expectedFilePath), holder.FileName);
        Assert.Equal(expectedFilePath, holder.FilePath);
        Assert.Equal(FileSystem.Path.GetDirectoryName(expectedFilePath), holder.Directory);

        Assert.Equal(expectedFilePath, holder.FileInformation.FilePath);
        Assert.NotSame(param, holder.FileInformation);
        Assert.NotSame(param, ((IPetroglyphFileHolder)holder).FileInformation);
    }

    [PlatformSpecificTheory(TestPlatformIdentifier.Linux)]
    [InlineData("FOO\\BAR.XML")]
    [InlineData("DATA\\XML\\FOO.XML")]
    public void Ctor_LocalFile_BackslashIsLiteralFilenameChar_Linux(string filePath)
    {
        var model = CreateModel();
        FileSystem.Initialize().WithFile(filePath);
        var holder = CreateFileHolder(model, CreateFileInfo(filePath));

        Assert.Equal(filePath, holder.FileName);
        Assert.Equal("/", holder.Directory);
        Assert.Equal("/" + filePath, holder.FilePath);
    }

    [Theory]
    [InlineData("foo.xml")]
    [InlineData("FOO.XML")]
    [InlineData("DATA\\FOO.XML")]
    [InlineData("data/foo.xml")]
    [InlineData("DATA\\XML\\FOO.XML")]
    [InlineData("DATA/SUB\\FOO.XML")]
    [InlineData("DATA\\SUB/FOO.XML")]
    [InlineData("./DATA\\SUB/FOO.XML")]
    [InlineData(".\\DATA\\SUB/FOO.XML")]
    public void Ctor_InMeg_SetupProperties(string filePath)
    {
        if (!typeof(PetroglyphMegPackableFileInformation).IsAssignableFrom(typeof(TFileInfo)))
            return;

        var model = CreateModel();
        var param = CreateFileInfo(filePath, inMeg: true);

        var holder = CreateFileHolder(model, param);

        var expectedFilePath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? filePath
            : filePath.Replace('\\', '/');

        Assert.Equal(FileSystem.Path.GetFileName(expectedFilePath), holder.FileName);
        Assert.Equal(expectedFilePath, holder.FilePath);
        Assert.Equal(FileSystem.Path.GetDirectoryName(expectedFilePath), holder.Directory);

        Assert.Equal(filePath, holder.FileInformation.FilePath);
        Assert.NotSame(param, holder.FileInformation);
        Assert.Equal(param, holder.FileInformation);
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

        if (!typeof(PetroglyphMegPackableFileInformation).IsAssignableFrom(typeof(TFileInfo)))
            return;

        Assert.DoesNotThrow(() => CreateFileHolder(model, CreateFileInfo("notFound", true)));
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
