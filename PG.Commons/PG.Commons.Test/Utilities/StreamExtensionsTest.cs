﻿using System.IO;
using System;
using Moq;
using PG.Commons.Utilities;
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
        var actualPath = fileStream.GetFilePath();

        Assert.Equal(Path.GetFullPath(expectedPath), actualPath);
    }

    [Fact]
    public void Test_GetFilePath_FileSystemStream()
    {
        var fs = new MockFileSystem();
        var expectedPath = "filesystemfile.txt";
        var fileSystemStream = fs.FileStream.New(expectedPath, FileMode.Create);
        var actualPath = fileSystemStream.GetFilePath();
        Assert.Equal(fs.Path.GetFullPath(expectedPath), actualPath);
    }

    [Fact]
    public void Test_GetFilePath_IMegFileDataStream()
    {
        var expectedPath = "megfiledatafile.txt";
        var megFileDataStream = new Mock<TestMegDataStream>();
        megFileDataStream.SetupGet(s => s.EntryPath).Returns(expectedPath);

        string actualPath = megFileDataStream.Object.GetFilePath();

        Assert.Equal(expectedPath, actualPath);
    }

    [Fact]
    public void Test_GetFilePath_StreamWithoutPath_ThrowsInvalidOperationException()
    {
        var memoryStream = new MemoryStream();
        Assert.Throws<InvalidOperationException>(memoryStream.GetFilePath);
    }

    public abstract class TestMegDataStream : Stream, IMegFileDataStream
    {
        public abstract string EntryPath { get; }
    }
}