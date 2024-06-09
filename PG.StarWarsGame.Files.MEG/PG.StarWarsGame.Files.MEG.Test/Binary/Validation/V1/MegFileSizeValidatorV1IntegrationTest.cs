using Moq;
using System.IO;
using System;
using System.IO.Abstractions;
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
        var fs = new MockFileSystem();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        _binaryReader = new MegFileBinaryReaderV1(sp.Object);
    }

    [Fact]
    public void Test__ValidateCore_CorrectSize()
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
    public void Test__ValidateCore_IncorrectSize(int offsetBytesRead, int offsetArchiveSize)
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