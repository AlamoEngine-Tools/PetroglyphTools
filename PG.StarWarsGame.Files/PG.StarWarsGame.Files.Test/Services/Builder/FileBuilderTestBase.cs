using System;
using System.IO;
using PG.StarWarsGame.Files.Services.Builder;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.Test.Services.Builder;

public abstract class FileBuilderTestBase<TBuilder, TModel, TFileInfo> : CommonTestBase
    where TBuilder : IFileBuilder<TModel, TFileInfo>
    where TModel : notnull 
    where TFileInfo : PetroglyphFileInformation
{
    protected virtual string DefaultFileName => "file.txt";

    protected virtual bool FileInfoIsAlwaysValid => false;
    protected abstract TBuilder CreateBuilder();
    protected abstract TFileInfo CreateFileInfo(bool valid, string path);
    protected abstract void AddDataToBuilder(TModel data, TBuilder builder);
    protected abstract (TModel Data, byte[] Bytes) CreateValidData();

    [Fact]
    public void Dispose_ThrowsOnBuild()
    {
        var builder = CreateBuilder();
        builder.Dispose();
        Assert.Throws<ObjectDisposedException>(() => builder.Build(CreateFileInfo(true, DefaultFileName), false));
        ExceptionUtilities.AssertDoesNotThrowException(builder.Dispose);
    }

    [Fact]
    public void ValidateFileInformation_NullArg_Throws()
    {
        using var builder = CreateBuilder();
        AddDataToBuilder(CreateValidData().Data, builder);
        Assert.Throws<ArgumentNullException>(() => builder.ValidateFileInformation(null!));
    }

    [Fact]
    public void Build_NullArg_Throws()
    {
        using var builder = CreateBuilder();
        AddDataToBuilder(CreateValidData().Data, builder);
        Assert.Throws<ArgumentNullException>(() => builder.Build(null!, false));
    }

    [Fact]
    public void Build()
    {
        using var builder = CreateBuilder();

        var fileInfo = CreateFileInfo(true, DefaultFileName);

        var dataInfo = CreateValidData();

        AddDataToBuilder(dataInfo.Data, builder);

        builder.Build(fileInfo, false);

        Assert.Equal(dataInfo.Bytes, FileSystem.File.ReadAllBytes(fileInfo.FilePath));
    }

    [Fact]
    public void Build_FileInfoNotValid_ThrowsInvalidOperationException()
    {
        if (FileInfoIsAlwaysValid)
            return;

        using var builder = CreateBuilder();

        var fileInfo = CreateFileInfo(false, DefaultFileName);

        var dataInfo = CreateValidData();

        AddDataToBuilder(dataInfo.Data, builder);

        Assert.Throws<InvalidOperationException>(() => builder.Build(fileInfo, false));
        Assert.False(FileSystem.File.Exists(fileInfo.FilePath));
    }

    [Fact]
    public void Build_DoNotOverwrite_Throws()
    {
        using var builder = CreateBuilder();

        var fileInfo = CreateFileInfo(true, DefaultFileName);

        var dataInfo = CreateValidData();
        AddDataToBuilder(dataInfo.Data, builder);

        FileSystem.Initialize().WithFile(fileInfo.FilePath).Which(x => x.HasBytesContent("cccc"u8.ToArray()));

        Assert.Throws<IOException>(() => builder.Build(fileInfo, false));

        Assert.True(FileSystem.File.Exists(fileInfo.FilePath));
        Assert.Equal("cccc"u8.ToArray(), FileSystem.File.ReadAllBytes(fileInfo.FilePath));
    }

    [Fact]
    public void Build_DoOverwrite()
    {
        using var builder = CreateBuilder();

        var fileInfo = CreateFileInfo(true, DefaultFileName);

        var dataInfo = CreateValidData();
        AddDataToBuilder(dataInfo.Data, builder);

        FileSystem.Initialize().WithFile(fileInfo.FilePath).Which(x => x.HasBytesContent("cccc"u8.ToArray()));

        builder.Build(fileInfo, true);

        Assert.True(FileSystem.File.Exists(fileInfo.FilePath));
        Assert.Equal(dataInfo.Bytes, FileSystem.File.ReadAllBytes(fileInfo.FilePath));
    }

    [Theory]
    [InlineData("./")]
    [InlineData("./..")]
    [InlineData("path/")]
    [InlineData("/")]
    public void Build_InvalidInfoPath_Throws(string path)
    {
        using var builder = CreateBuilder();

        var fileInfo = CreateFileInfo(true, path);
        var dataInfo = CreateValidData();
        AddDataToBuilder(dataInfo.Data, builder);

        if (FileInfoIsAlwaysValid)
            Assert.ThrowsAny<IOException>(() => builder.Build(fileInfo, false));
        else
        {
            // Validation *might catch it and throw something different.*
            Assert.ThrowsAny<Exception>(() => builder.Build(fileInfo, false));
        }
    }
}