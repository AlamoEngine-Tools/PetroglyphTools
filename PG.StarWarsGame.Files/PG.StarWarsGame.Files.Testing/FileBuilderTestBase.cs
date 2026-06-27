using PG.StarWarsGame.Files.Services.Builder;
using PG.Testing;
using System;
using System.IO;
using AnakinRaW.CommonUtilities.Testing.Extensions;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.Testing;

/// <summary>
/// Provides a shared set of tests for verifying the behavior of an <see cref="IFileBuilder{TModel,TFileInfo}"/> implementation.
/// </summary>
/// <typeparam name="TBuilder">The type of the file builder under test.</typeparam>
/// <typeparam name="TModel">The type of the model that the builder produces output from.</typeparam>
/// <typeparam name="TFileInfo">The type of the file information that describes the builder's output.</typeparam>
public abstract class FileBuilderTestBase<TBuilder, TModel, TFileInfo> : PGTestBase
    where TBuilder : IFileBuilder<TModel, TFileInfo>
    where TModel : notnull
    where TFileInfo : PetroglyphFileInformation
{
    /// <summary>
    /// Gets the default name of the file that the builder writes during the tests.
    /// </summary>
    /// <value>The default file name.</value>
    protected virtual string DefaultFileName => "file.txt";

    /// <summary>
    /// Gets a value that indicates whether the file information produced by the builder is always considered valid.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the file information is always valid; otherwise, <see langword="false"/>. The default is <see langword="false"/>.
    /// </value>
    protected virtual bool FileInfoIsAlwaysValid => false;

    /// <summary>
    /// Creates a new instance of the file builder under test.
    /// </summary>
    /// <returns>The newly created file builder.</returns>
    protected abstract TBuilder CreateBuilder();

    /// <summary>
    /// Creates the file information for the specified path.
    /// </summary>
    /// <param name="valid"><see langword="true"/> to create valid file information; otherwise, <see langword="false"/>.</param>
    /// <param name="path">The path of the file the information describes.</param>
    /// <returns>The created file information.</returns>
    protected abstract TFileInfo CreateFileInfo(bool valid, string path);

    /// <summary>
    /// Adds the specified data to the builder.
    /// </summary>
    /// <param name="data">The data to add to the builder.</param>
    /// <param name="builder">The builder the data is added to.</param>
    protected abstract void AddDataToBuilder(TModel data, TBuilder builder);

    /// <summary>
    /// Creates a model and its expected serialized byte representation that together form valid builder input.
    /// </summary>
    /// <returns>A tuple containing the valid model and the bytes the builder is expected to produce from it.</returns>
    protected abstract (TModel Data, byte[] Bytes) CreateValidData();

    [Fact]
    public void Dispose_ThrowsOnBuild()
    {
        var builder = CreateBuilder();
        builder.Dispose();
        Assert.Throws<ObjectDisposedException>(() => builder.Build(CreateFileInfo(true, DefaultFileName), false));
        Assert.DoesNotThrow(builder.Dispose);
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