using PG.Commons.Utilities;
using PG.Testing;
using System;
using System.IO;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.Commons.Test.Utilities;

public class StreamExtensionsTest
{
    [Fact]
    public void GetFilePath_FileStream()
    {
        const string expectedPath = "testfile.txt";
        using var fileStream = new FileStream(expectedPath, FileMode.Create, FileAccess.Write, FileShare.None, 1024, FileOptions.DeleteOnClose);
        Assert.True(fileStream.TryGetFilePath(out var path1));
        Assert.True(fileStream.TryGetFilePath(out var path2, out var isMeg));
        Assert.False(isMeg);
        Assert.Equal(Path.GetFullPath(expectedPath), path1);
        Assert.Equal(Path.GetFullPath(expectedPath), path2);
        Assert.Equal(Path.GetFullPath(expectedPath), fileStream.TryGetFilePath());
        Assert.Equal(Path.GetFullPath(expectedPath), fileStream.GetFilePath());
        Assert.Equal(Path.GetFullPath(expectedPath), fileStream.GetFilePath(out isMeg));
        Assert.False(isMeg);
    }

    [Fact]
    public void GetFilePath_FileSystemStream()
    {
        const string expectedPath = "filesystemfile.txt";
        var fs = new MockFileSystem();
        var fileSystemStream = fs.FileStream.New(expectedPath, FileMode.Create);

        Assert.True(fileSystemStream.TryGetFilePath(out var path1));
        Assert.True(fileSystemStream.TryGetFilePath(out var path2, out var isMeg));
        Assert.False(isMeg);
        Assert.Equal(fs.Path.GetFullPath(expectedPath), path1);
        Assert.Equal(fs.Path.GetFullPath(expectedPath), path2);
        Assert.Equal(fs.Path.GetFullPath(expectedPath), fileSystemStream.TryGetFilePath());
        Assert.Equal(fs.Path.GetFullPath(expectedPath), fileSystemStream.GetFilePath());
        Assert.Equal(fs.Path.GetFullPath(expectedPath), fileSystemStream.GetFilePath(out isMeg));
        Assert.False(isMeg);
    }

    [Fact]
    public void GetFilePath_IMegFileDataStream()
    {
        const string expectedPath = "megfiledatafile.txt";
        var megFileDataStream = new TestMegDataStream(expectedPath, Stream.Null);

        Assert.True(megFileDataStream.TryGetFilePath(out var path1));
        Assert.True(megFileDataStream.TryGetFilePath(out var path2, out var isMeg));
        Assert.True(isMeg);
        Assert.Equal(expectedPath, path1);
        Assert.Equal(expectedPath, path2);
        Assert.Equal(expectedPath, megFileDataStream.TryGetFilePath());
        Assert.Equal(expectedPath, megFileDataStream.GetFilePath());
        Assert.Equal(expectedPath, megFileDataStream.GetFilePath(out isMeg));
        Assert.True(isMeg);
    }

    [Fact]
    public void GetFilePath_StreamWithoutPath_ThrowsInvalidOperationException()
    {
        var memoryStream = new MemoryStream();

        var isMeg = false;
        
        Assert.Throws<InvalidOperationException>(memoryStream.GetFilePath);
        Assert.Throws<InvalidOperationException>(() => memoryStream.GetFilePath(out isMeg));
        Assert.False(isMeg);

        Assert.Null(memoryStream.TryGetFilePath());
        Assert.False(memoryStream.TryGetFilePath(out var path1));
        Assert.False(memoryStream.TryGetFilePath(out var path2, out isMeg));
        Assert.Null(path1);
        Assert.Null(path2);
        Assert.False(isMeg);
    }
}