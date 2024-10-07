using System.IO;
using System;
using PG.Commons.Utilities;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.Commons.Test.Utilities;

public class StreamExtensionsTest
{
    [Fact]
    public void Test_GetFilePath_FileStream()
    {
        var expectedPath = "testfile.txt";
        using var fileStream = new FileStream(expectedPath, FileMode.Create, FileAccess.Write, FileShare.None, 1024, FileOptions.DeleteOnClose);
        Assert.Equal(Path.GetFullPath(expectedPath), fileStream.GetFilePath());
        Assert.Equal(Path.GetFullPath(expectedPath), fileStream.GetFilePath(out var isMeg));
        Assert.False(isMeg);
    }

    [Fact]
    public void Test_GetFilePath_FileSystemStream()
    {
        var fs = new MockFileSystem();
        var expectedPath = "filesystemfile.txt";
        var fileSystemStream = fs.FileStream.New(expectedPath, FileMode.Create);
        Assert.Equal(fs.Path.GetFullPath(expectedPath), fileSystemStream.GetFilePath());
        Assert.Equal(fs.Path.GetFullPath(expectedPath), fileSystemStream.GetFilePath(out var isMeg));
        Assert.False(isMeg);
    }

    [Fact]
    public void Test_GetFilePath_IMegFileDataStream()
    {
        var expectedPath = "megfiledatafile.txt";
        var megFileDataStream = new TestMegDataStream("megfiledatafile.txt", Stream.Null);

        Assert.Equal(expectedPath, megFileDataStream.GetFilePath());
        Assert.Equal(expectedPath, megFileDataStream.GetFilePath(out var isMeg));
        Assert.True(isMeg);
    }

    [Fact]
    public void Test_GetFilePath_StreamWithoutPath_ThrowsInvalidOperationException()
    {
        var memoryStream = new MemoryStream();
        Assert.Throws<InvalidOperationException>(memoryStream.GetFilePath);
    }
}