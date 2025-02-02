using System;
using PG.StarWarsGame.Files.MTD.Data;
using PG.StarWarsGame.Files.MTD.Files;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MTD.Test.Files;

public class MtdFileTest : CommonMtdTestBase
{
    [Fact]
    public void Ctor_ThrowsArgumentNullException()
    {
        var param = new MtdFileInformation
        {
            FilePath = "test.mtd",
            IsInsideMeg = false
        };
        var model = new MegaTextureDirectory([]);

        Assert.Throws<ArgumentNullException>(() => new MtdFile(null!, param, ServiceProvider));
        Assert.Throws<ArgumentNullException>(() => new MtdFile(model, null!, ServiceProvider));
        Assert.Throws<ArgumentNullException>(() => new MtdFile(model, param, null!));
    }

    [Fact]
    public void Ctor_SetupProperties()
    {
        const string name = "test.mtd";
        var param = new MtdFileInformation
        {
            FilePath = name,
            IsInsideMeg = false,
        };
        var model = new MegaTextureDirectory([]);

        FileSystem.Initialize().WithFile("test.mtd");

        var mtdFile = new MtdFile(model, param, ServiceProvider);

        Assert.Same(model, mtdFile.Content); 
        Assert.Equal(FileSystem.Path.GetFullPath(name), mtdFile.FileInformation.FilePath);
    }

    [Fact]
    public void Ctor_InMeg_SetupProperties()
    {
        const string name = "test.mtd";
        var param = new MtdFileInformation
        {
            FilePath = name,
            IsInsideMeg = true,
        };
        var model = new MegaTextureDirectory([]);

        FileSystem.Initialize().WithFile("test.mtd");

        var mtdFile = new MtdFile(model, param, ServiceProvider);

        Assert.Same(model, mtdFile.Content);
        Assert.Equal(name, mtdFile.FileInformation.FilePath);
    }
}