using System;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Size;
using PG.StarWarsGame.Files.MEG.Binary.V1;
using PG.StarWarsGame.Files.MEG.Files;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary;

public class MegBinaryServiceFactoryTest : CommonMegTestBase
{
    private readonly MegBinaryServiceFactory _factory;

    public MegBinaryServiceFactoryTest()
    {
        _factory = new MegBinaryServiceFactory(ServiceProvider);
    }

    [Theory]
    [InlineData(MegFileVersion.V1, typeof(MegFileBinaryReaderV1))]
    public void GetReader_V1_ReturnsCorrectType(MegFileVersion version, Type expectedType)
    {
        var reader = _factory.GetReader(version);
        Assert.IsType(expectedType, reader);
    }

    [Theory]
    [InlineData(MegFileVersion.V1, typeof(MegBinaryConverterV1))]
    public void GetConverter_V1_ReturnsCorrectType(MegFileVersion version, Type expectedType)
    {
        var converter = _factory.GetConverter(version);
        Assert.IsType(expectedType, converter);
    }

    [Theory]
    [InlineData(MegFileVersion.V1, typeof(ConstructingMegArchiveBuilderV1))]
    public void GetConstructionBuilder_V1_ReturnsCorrectType(MegFileVersion version, Type expectedType)
    {
        var builder = _factory.GetConstructionBuilder(version);
        Assert.IsType(expectedType, builder);
    }

    [Theory]
    [InlineData(MegFileVersion.V1, typeof(MegV1SizeCalculator))]
    public void GetMegSizeCalculator_ReturnsCorrectType(MegFileVersion version, Type expectedType)
    {
        var calculator = _factory.GetMegSizeCalculator(version);
        Assert.IsType(expectedType, calculator);
    }

    [Theory]
    [InlineData(MegFileVersion.V2)]
    [InlineData(MegFileVersion.V3)]
    public void V2andV3_Unsupported_ThrowsNotImplementedException(MegFileVersion version)
    {
        Assert.Throws<NotImplementedException>(() => _factory.GetConstructionBuilder(version));
        Assert.Throws<NotImplementedException>(() => _factory.GetMegSizeCalculator(version));
        Assert.Throws<NotImplementedException>(() => _factory.GetConverter(version));
        Assert.Throws<NotImplementedException>(() => _factory.GetReader(version));
    }
}
