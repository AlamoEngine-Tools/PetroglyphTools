using System.Drawing;
using PG.StarWarsGame.Files.MTD.Binary.Metadata;
using PG.StarWarsGame.Files.MTD.Data;
using Xunit;

namespace PG.StarWarsGame.Files.MTD.Test;

public class MtdEntryInformationContainer
{
    public string ExpectedName { get; init; }

    public Rectangle ExpectedArea { get; init; }

    public bool ExpectedAlpha { get; init; }

    internal void AsserEquals(MtdBinaryFileInfo fileInfo)
    {
        Assert.Equal(ExpectedName, fileInfo.Name);
        Assert.Equal(ExpectedArea.X, (int)fileInfo.X);
        Assert.Equal(ExpectedArea.Y, (int)fileInfo.Y);
        Assert.Equal(ExpectedArea.Width, (int)fileInfo.Width);
        Assert.Equal(ExpectedArea.Height, (int)fileInfo.Height);

        Assert.Equal(ExpectedAlpha, fileInfo.Alpha);
    }

    public void AsserEquals(MegaTextureFileIndex fileInfo)
    {
        Assert.Equal(ExpectedName, fileInfo.FileName);
        Assert.Equal(ExpectedArea.X, fileInfo.Area.X);
        Assert.Equal(ExpectedArea.Y, fileInfo.Area.Y);
        Assert.Equal(ExpectedArea.Width, fileInfo.Area.Width);
        Assert.Equal(ExpectedArea.Height, fileInfo.Area.Height);

        Assert.Equal(ExpectedAlpha, fileInfo.HasAlpha);
    }
}