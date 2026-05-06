using System;
using Xunit;

namespace PG.StarWarsGame.Files.Test;

public class PetroglyphFileInformationTest
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void EmptyPath_Throws(string? path)
    {
        Assert.ThrowsAny<ArgumentException>(() => _ = new TestMegFileInfo
        {
            FilePath = path!
        });
    }

    [Theory]
    [InlineData("file.txt")]
    [InlineData("FILE.TXT")]
    [InlineData("path/file.txt")]
    [InlineData("PATH\\FILE.TXT")]
    public void FilePath_ContainsRawData(string path)
    {
        Assert.Equal(path, new TestFileInfo
        {
            FilePath = path,
        }.FilePath);
        Assert.Equal(path, new TestMegFileInfo
        {
            FilePath = path,
            IsInsideMeg = true
        }.FilePath);
        Assert.Equal(path, new TestMegFileInfo
        {
            FilePath = path,
            IsInsideMeg = false
        }.FilePath);
    }

    [Fact]
    public void Dispose()
    {
        var info = new TestMegFileInfo
        {
            FilePath = "somePath"
        };
        Assert.False(info.IsDisposed);
        info.Dispose();
        Assert.True(info.IsDisposed);
    }
}