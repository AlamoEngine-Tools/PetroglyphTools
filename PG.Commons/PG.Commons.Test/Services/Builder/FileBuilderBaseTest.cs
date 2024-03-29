using System;
using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using PG.Commons.Files;
using PG.Commons.Services.Builder;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.Commons.Test.Services.Builder;

public class FileBuilderBaseTest
{
    private readonly MockFileSystem _fileSystem = new();

    public record TestFileInfo : PetroglyphFileInformation
    {
        public static TestFileInfo Create()
        {
            return new TestFileInfo
            {
                FilePath = "path.txt"
            };
        }
    }

    private IServiceProvider CreateServiceProvider()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        return sc.BuildServiceProvider();
    }
    
    [Fact]
    public void Test_Dispose_ThrowsOnBuild()
    {
        var builder = new Mock<FileBuilderBase<object, TestFileInfo>>(CreateServiceProvider());
        builder.Object.Dispose();
        Assert.Throws<ObjectDisposedException>(() => builder.Object.Build(TestFileInfo.Create(), false));
        ExceptionUtilities.AssertDoesNotThrowException(builder.Object.Dispose);
    }

    [Fact]
    public void Test_ValidateFileInformation()
    {
        var data = new object();

        var builder = new Mock<FileBuilderBase<object, TestFileInfo>>(CreateServiceProvider());
        builder.SetupGet(b => b.BuilderData).Returns(data);

        Assert.Throws<ArgumentNullException>(() => builder.Object.ValidateFileInformation(null!));

        var fileInfo = TestFileInfo.Create();

        builder.Protected().Setup<bool>("ValidateFileInformationCore", fileInfo, data, ItExpr.Ref<string>.IsAny)
            .Returns(true);

        var result = builder.Object.ValidateFileInformation(fileInfo);
        Assert.True(result);

        builder.Protected().Setup<bool>("ValidateFileInformationCore", fileInfo, data, ItExpr.Ref<string>.IsAny)
            .Returns(false);

        result = builder.Object.ValidateFileInformation(fileInfo);
        Assert.False(result);

        builder.Protected().Verify<bool>("ValidateFileInformationCore", Times.Exactly(2), fileInfo, data, ItExpr.Ref<string>.IsAny);
    }

    [Fact]
    public void Test_Build()
    {
        var fileInfo = TestFileInfo.Create();
        var data = new object();
        var expectedData = new byte[] { 1, 2, 3 };

        var builder = new Mock<FileBuilderBase<object, TestFileInfo>>(CreateServiceProvider());
        builder.SetupGet(b => b.BuilderData).Returns(data);

        string tmpFileName = null!;
        builder.Protected().Setup("BuildFileCore", ItExpr.IsAny<FileSystemStream>(), fileInfo, data)
            .Callback((FileSystemStream fs, TestFileInfo _, object _) =>
            {
                Assert.NotEqual(_fileSystem.Path.GetFullPath(fs.Name), _fileSystem.Path.GetFullPath(fileInfo.FilePath));
                tmpFileName = fs.Name;
                fs.Write(expectedData, 0, 3);
            });

        builder.Protected().Setup<bool>("ValidateFileInformationCore", fileInfo, data, ItExpr.Ref<string>.IsAny)
            .Returns(true);

        builder.Object.Build(fileInfo, false);
        Assert.Equal(expectedData, _fileSystem.File.ReadAllBytes(fileInfo.FilePath));

        Assert.False(_fileSystem.File.Exists(tmpFileName));
    }

    [Fact]
    public void Test_Build_FileInfoNotValid_ThrowsInvalidOperationException()
    {
        var fileInfo = TestFileInfo.Create();
        var data = new object();
        var expectedData = new byte[] { 1, 2, 3 };

        var builder = new Mock<FileBuilderBase<object, TestFileInfo>>(CreateServiceProvider());
        builder.SetupGet(b => b.BuilderData).Returns(data);

        builder.Protected().Setup("BuildFileCore", ItExpr.IsAny<FileSystemStream>(), fileInfo, data)
            .Callback((FileSystemStream fs, TestFileInfo _, object _) =>
            {
                fs.Write(expectedData, 0, 3);
            });

        builder.Protected().Setup<bool>("ValidateFileInformationCore", fileInfo, data, ItExpr.Ref<string>.IsAny)
            .Returns(false);

        Assert.Throws<InvalidOperationException>(() => builder.Object.Build(fileInfo, false));
        Assert.False(_fileSystem.File.Exists(fileInfo.FilePath));
    }

    [Fact]
    public void Test_Build_WritingFails_Throws()
    {

        var fileInfo = TestFileInfo.Create();
        var data = new object();

        var builder = new Mock<FileBuilderBase<object, TestFileInfo>>(CreateServiceProvider());
        builder.SetupGet(b => b.BuilderData).Returns(data);

        builder.Protected().Setup("BuildFileCore", ItExpr.IsAny<FileSystemStream>(), fileInfo, data)
            .Throws<IOException>();

        builder.Protected().Setup<bool>("ValidateFileInformationCore", fileInfo, data, ItExpr.Ref<string>.IsAny)
            .Returns(true);

        Assert.Throws<IOException>(() => builder.Object.Build(fileInfo, false));
        Assert.False(_fileSystem.File.Exists(fileInfo.FilePath));
    }

    [Fact]
    public void Test_Build_DoNotOverwrite_Throws()
    {
        var fileInfo = TestFileInfo.Create();
        var data = new object();
        var expectedData = new byte[] { 1, 2, 3 };

        _fileSystem.Initialize().WithFile(fileInfo.FilePath);

        var builder = new Mock<FileBuilderBase<object, TestFileInfo>>(CreateServiceProvider());
        builder.SetupGet(b => b.BuilderData).Returns(data);

        builder.Protected().Setup("BuildFileCore", ItExpr.IsAny<FileSystemStream>(), fileInfo, data)
            .Callback((FileSystemStream fs, TestFileInfo _, object _) =>
            {
                fs.Write(expectedData, 0, 3);
            });

        builder.Protected().Setup<bool>("ValidateFileInformationCore", fileInfo, data, ItExpr.Ref<string>.IsAny)
            .Returns(true);

        Assert.Throws<IOException>(() => builder.Object.Build(fileInfo, false));
        Assert.True(_fileSystem.File.Exists(fileInfo.FilePath));
        Assert.NotEqual(expectedData, _fileSystem.File.ReadAllBytes(fileInfo.FilePath));
    }

    [Fact]
    public void Test_CreateMegArchive_RealFileSystem_OverrideCurrentMeg()
    {
        var fileInfo = TestFileInfo.Create();
        var data = new object();
        var expectedData = new byte[] { 1, 2, 3 };

        // This Test does not use the MockFileSystem but the actual FileSystem
        var fs = new FileSystem();

        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(fs);


        var builder = new Mock<FileBuilderBase<object, TestFileInfo>>(sc.BuildServiceProvider());
        builder.SetupGet(b => b.BuilderData).Returns(data);

        builder.Protected().Setup("BuildFileCore", ItExpr.IsAny<FileSystemStream>(), fileInfo, data)
            .Callback((FileSystemStream fs, TestFileInfo _, object _) =>
            {
                fs.Write(expectedData, 0, 3);
            });

        builder.Protected().Setup<bool>("ValidateFileInformationCore", fileInfo, data, ItExpr.Ref<string>.IsAny)
            .Returns(true);

        try
        {
            fs.File.WriteAllBytes(fileInfo.FilePath, [0, 0, 0]);

            builder.Object.Build(fileInfo, true);

            var actualBytes = fs.File.ReadAllBytes(fileInfo.FilePath);
            Assert.Equal(expectedData, actualBytes);
        }
        finally
        {
            try
            {
                fs.File.Delete(fileInfo.FilePath);
            }
            catch
            {
                // Ignore
            }
        }
    }

    [Theory]
    [InlineData("./")]
    [InlineData("./..")]
    [InlineData("path/")]
    [InlineData("/")]
    public void Test_Build_InvalidInfoPath_Throws(string path)
    {
        var fileInfo = new TestFileInfo
        {
            FilePath = path
        };
        var data = new object();

        var builder = new Mock<FileBuilderBase<object, TestFileInfo>>(CreateServiceProvider());
        builder.SetupGet(b => b.BuilderData).Returns(data);

        builder.Protected().Setup<bool>("ValidateFileInformationCore", fileInfo, data, ItExpr.Ref<string>.IsAny)
            .Returns(true);

        Assert.Throws<IOException>(() => builder.Object.Build(fileInfo, false));
    }
}