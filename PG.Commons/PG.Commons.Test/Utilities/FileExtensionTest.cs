using System.IO;
using PG.Commons.Utilities;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.Commons.Test.Utilities;

public class FileExtensionTest
{
    private readonly MockFileSystem _fileSystem = new();

    [Fact]
    public void Test_CreateRandomHiddenTemporaryFile_DirectoryNotExist()
    {
        Assert.Throws<DirectoryNotFoundException>(() => _fileSystem.File.CreateRandomHiddenTemporaryFile());
        Assert.Throws<DirectoryNotFoundException>(() => _fileSystem.File.CreateRandomHiddenTemporaryFile("/test"));
    }

    [Fact]
    public void Test_CreateRandomHiddenTemporaryFile()
    {
        _fileSystem.Initialize();

        var tempStream = _fileSystem.File.CreateRandomHiddenTemporaryFile();
        var filePath = tempStream.Name;

        // Check file exists
        Assert.True(_fileSystem.File.Exists(filePath));
        Assert.True(_fileSystem.File.GetAttributes(filePath).HasFlag(FileAttributes.Hidden));

        // Check file read/write
        tempStream.Write([1,2,3], 0, 3);
        tempStream.Seek(0, SeekOrigin.Begin);

        var resultBuffer = new byte[3];
        _ = tempStream.Read(resultBuffer, 0, 3);
        Assert.Equal([1,2,3], resultBuffer);

        // Check deleted
        tempStream.Dispose();
        Assert.False(_fileSystem.File.Exists(filePath));
    }

    [Fact]
    public void Test_CreateRandomHiddenTemporaryFile_FromDirectory()
    {
        _fileSystem.Initialize().WithSubdirectory(_fileSystem.Path.GetFullPath("test"));

        var tempStream = _fileSystem.File.CreateRandomHiddenTemporaryFile(_fileSystem.Path.GetFullPath("test"));
        var filePath = tempStream.Name;

        Assert.True(filePath.StartsWith(_fileSystem.Path.GetFullPath("test")));

        // Check file exists
        Assert.True(_fileSystem.File.Exists(filePath));
        Assert.True(_fileSystem.File.GetAttributes(filePath).HasFlag(FileAttributes.Hidden));

        // Check file read/write
        tempStream.Write([1, 2, 3], 0, 3);
        tempStream.Seek(0, SeekOrigin.Begin);

        var resultBuffer = new byte[3];
        _ = tempStream.Read(resultBuffer, 0, 3);
        Assert.Equal([1, 2, 3], resultBuffer);

        // Check deleted
        tempStream.Dispose();
        Assert.False(_fileSystem.File.Exists(filePath));
    }
}