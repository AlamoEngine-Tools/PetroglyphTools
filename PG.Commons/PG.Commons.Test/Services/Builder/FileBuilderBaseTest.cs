using System;
using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using PG.Commons.Files;
using PG.Commons.Services.Builder;
using PG.Testing;
using Testably.Abstractions.Testing;

namespace PG.Commons.Test.Services.Builder;

[TestClass]
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
    
    [TestMethod]
    public void Test_Dispose_ThrowsOnBuild()
    {
        var builder = new Mock<FileBuilderBase<object, TestFileInfo>>(CreateServiceProvider());
        builder.Object.Dispose();
        Assert.ThrowsException<ObjectDisposedException>(() => builder.Object.Build(TestFileInfo.Create(), false));
        ExceptionUtilities.AssertDoesNotThrowException(builder.Object.Dispose);
    }

    [TestMethod]
    public void Test_ValidateFileInformation()
    {
        var data = new object();

        var builder = new Mock<FileBuilderBase<object, TestFileInfo>>(CreateServiceProvider());
        builder.SetupGet(b => b.BuilderData).Returns(data);

        Assert.ThrowsException<ArgumentNullException>(() => builder.Object.ValidateFileInformation(null!));

        var fileInfo = TestFileInfo.Create();

        builder.Protected().Setup<bool>("ValidateFileInformationCore", fileInfo, data, ItExpr.Ref<string>.IsAny)
            .Returns(true);

        var result = builder.Object.ValidateFileInformation(fileInfo);
        Assert.IsTrue(result);

        builder.Protected().Setup<bool>("ValidateFileInformationCore", fileInfo, data, ItExpr.Ref<string>.IsAny)
            .Returns(false);

        result = builder.Object.ValidateFileInformation(fileInfo);
        Assert.IsFalse(result);

        builder.Protected().Verify<bool>("ValidateFileInformationCore", Times.Exactly(2), fileInfo, data, ItExpr.Ref<string>.IsAny);
    }

    [TestMethod]
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
                Assert.AreNotEqual(_fileSystem.Path.GetFullPath(fs.Name), _fileSystem.Path.GetFullPath(fileInfo.FilePath));
                tmpFileName = fs.Name;
                fs.Write(expectedData, 0, 3);
            });

        builder.Protected().Setup<bool>("ValidateFileInformationCore", fileInfo, data, ItExpr.Ref<string>.IsAny)
            .Returns(true);

        builder.Object.Build(fileInfo, false);
        CollectionAssert.AreEqual(expectedData, _fileSystem.File.ReadAllBytes(fileInfo.FilePath));

        Assert.IsFalse(_fileSystem.File.Exists(tmpFileName));
    }

    [TestMethod]
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

        Assert.ThrowsException<InvalidOperationException>(() => builder.Object.Build(fileInfo, false));
        Assert.IsFalse(_fileSystem.File.Exists(fileInfo.FilePath));
    }

    [TestMethod]
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

        Assert.ThrowsException<IOException>(() => builder.Object.Build(fileInfo, false));
        Assert.IsFalse(_fileSystem.File.Exists(fileInfo.FilePath));
    }

    [TestMethod]
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

        Assert.ThrowsException<IOException>(() => builder.Object.Build(fileInfo, false));
        Assert.IsTrue(_fileSystem.File.Exists(fileInfo.FilePath));
        CollectionAssert.AreNotEqual(expectedData, _fileSystem.File.ReadAllBytes(fileInfo.FilePath));
    }

    [TestMethod]
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
            CollectionAssert.AreEqual(expectedData, actualBytes);
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

    [TestMethod]
    [DataRow("./")]
    [DataRow("./..")]
    [DataRow("path/")]
    [DataRow("/")]
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

        Assert.ThrowsException<IOException>(() => builder.Object.Build(fileInfo, false));
    }
}