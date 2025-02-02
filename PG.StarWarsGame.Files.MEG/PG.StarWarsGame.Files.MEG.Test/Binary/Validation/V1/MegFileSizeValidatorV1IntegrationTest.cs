using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons;
using PG.StarWarsGame.Files.MEG.Binary.V1;
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Validation.V1;

public class MegFileSizeValidatorV1IntegrationTest
{
    private readonly MegFileSizeValidator _validator = new();
    private readonly MegFileBinaryReaderV1 _binaryReader;

    public MegFileSizeValidatorV1IntegrationTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        PetroglyphCommons.ContributeServices(sc);
        sc.SupportMEG();
        _binaryReader = new MegFileBinaryReaderV1(sc.BuildServiceProvider());
    }

    [Fact]
    public void ValidateCore_CorrectSize()
    {
        var data = new MemoryStream(MegTestConstants.ContentMegFileV1);
        var metadata = _binaryReader.ReadBinary(data);

        var sizeInfo = new MegBinaryValidationInformation
        {
            BytesRead = data.Position,
            FileSize = data.Length,
            Metadata = metadata
        };

        Assert.True(_validator.Validate(sizeInfo));
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(0, 2)]
    [InlineData(0, -1)]
    [InlineData(-1, 0)]
    [InlineData(1, 0)]
    public void ValidateCore_IncorrectSize(int offsetBytesRead, int offsetArchiveSize)
    {
        var data = new MemoryStream(MegTestConstants.ContentMegFileV1);
        var metadata = _binaryReader.ReadBinary(data);

        var sizeInfo = new MegBinaryValidationInformation
        {
            BytesRead = data.Position + offsetBytesRead,
            FileSize = data.Length + offsetArchiveSize,
            Metadata = metadata
        };

        Assert.False(_validator.Validate(sizeInfo));
        Assert.False(_validator.Validate(sizeInfo));
        Assert.False(_validator.Validate(sizeInfo));

        Assert.False(_validator.Validate(sizeInfo));
        Assert.False(_validator.Validate(sizeInfo));
    }
}