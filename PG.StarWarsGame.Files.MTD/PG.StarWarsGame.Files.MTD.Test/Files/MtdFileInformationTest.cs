using System;
using PG.StarWarsGame.Files.MTD.Files;
using Xunit;

namespace PG.StarWarsGame.Files.MTD.Test.Files;

public class MtdFileInformationTest
{
    [Fact]
    public void Test_Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new MtdFileInformation{FilePath = null!});
        Assert.Throws<ArgumentException>(() => new MtdFileInformation{FilePath = ""});
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Ctor(bool inMeg)
    {
        var fileInfo = new MtdFileInformation{FilePath = "path", IsInsideMeg = inMeg};
        Assert.Equal("path", fileInfo.FilePath);
        Assert.Equal(inMeg, fileInfo.IsInsideMeg);
    }
}