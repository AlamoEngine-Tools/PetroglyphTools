using Moq;
using System;
using System.IO.Abstractions;
using PG.StarWarsGame.Files.MTD.Data;
using PG.StarWarsGame.Files.MTD.Files;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MTD.Test.Files;

public class MtdFileTest
{
    private readonly Mock<IServiceProvider> _serviceProvider = new();
    private readonly MockFileSystem _fileSystem = new();

    public MtdFileTest()
    {
        _serviceProvider.Setup(sp => sp.GetService(typeof(IFileSystem))).Returns(_fileSystem);
    }

    [Fact]
    public void Ctor_ThrowsArgumentNullException()
    {
        var param = new MtdFileInformation
        {
            FilePath = "test.mtd",
            IsInsideMeg = false
        };
        var model = new Mock<IMegaTextureDirectory>();

        Assert.Throws<ArgumentNullException>(() => new MtdFile(null!, param, _serviceProvider.Object));
        Assert.Throws<ArgumentNullException>(() => new MtdFile(model.Object, null!, _serviceProvider.Object));
        Assert.Throws<ArgumentNullException>(() => new MtdFile(model.Object, param, null!));
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
        var model = new Mock<IMegaTextureDirectory>().Object;

        _fileSystem.Initialize().WithFile("test.mtd");

        var mtdFile = new MtdFile(model, param, _serviceProvider.Object);

        Assert.Same(model, mtdFile.Content); 
        Assert.Equal(_fileSystem.Path.GetFullPath(name), mtdFile.FileInformation.FilePath);
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
        var model = new Mock<IMegaTextureDirectory>().Object;

        _fileSystem.Initialize().WithFile("test.mtd");

        var mtdFile = new MtdFile(model, param, _serviceProvider.Object);

        Assert.Same(model, mtdFile.Content);
        Assert.Equal(name, mtdFile.FileInformation.FilePath);
    }
}