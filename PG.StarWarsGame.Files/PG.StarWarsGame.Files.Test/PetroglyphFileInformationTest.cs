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
        Assert.ThrowsAny<ArgumentException>(() => _ = new MegTestParam
        {
            FilePath = path!
        });
    }

    [Fact]
    public void Dispose()
    {
        var info = new MegTestParam
        {
            FilePath = "somePath"
        };
        Assert.False(info.IsDisposed);
        info.Dispose();
        Assert.True(info.IsDisposed);
    }
}