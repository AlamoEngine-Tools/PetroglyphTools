using System;
using Moq;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Validation;

public class MegFileSizeValidatorTest
{
    [Fact]
    public void Test__Validate_Null()
    {
        var validator = new MegFileSizeValidator();
        Assert.Throws<ArgumentNullException>(() => validator.Validate(null!));
    }

    [Fact]
    public void Test__Validate_MetadataNull()
    {
        var sizeInfo = new Mock<IMegBinaryValidationInformation>();
        sizeInfo.SetupGet(s => s.FileSize).Returns(12);
        sizeInfo.SetupGet(s => s.BytesRead).Returns(1);
        sizeInfo.SetupGet(s => s.Metadata).Returns((IMegFileMetadata)null!);

        var validator = new MegFileSizeValidator();

        Assert.False(validator.Validate(sizeInfo.Object));
    }

    [Theory]
    [InlineData(-1L, 1L)]
    [InlineData(0L, 0L)]
    [InlineData(1L, -1L)]
    [InlineData(0L, 1L)]
    [InlineData(1L, 0L)]
    [InlineData(2L, 1L)]
    public void Test__Validate_NotValid(long readBytes, long size)
    {
        var metadata = new Mock<IMegFileMetadata>();
        var sizeInfo = new Mock<IMegBinaryValidationInformation>();
        sizeInfo.SetupGet(s => s.FileSize).Returns(size);
        sizeInfo.SetupGet(s => s.BytesRead).Returns(readBytes);
        sizeInfo.SetupGet(s => s.Metadata).Returns(metadata.Object);

        var validator = new MegFileSizeValidator(); 
        
        Assert.False(validator.Validate(sizeInfo.Object));
    }

    [Theory]
    [InlineData(1L, 12L)]
    [InlineData(1L, 1L)]
    public void Test__Validate_IsValid(long readBytes, long size)
    {
        var metadata = new Mock<IMegFileMetadata>();
        var sizeInfo = new Mock<IMegBinaryValidationInformation>();
        sizeInfo.SetupGet(s => s.FileSize).Returns(size);
        sizeInfo.SetupGet(s => s.BytesRead).Returns(readBytes);
        sizeInfo.SetupGet(s => s.Metadata).Returns(metadata.Object);

        var validator = new MegFileSizeValidator();

        Assert.True(validator.Validate(sizeInfo.Object));
    }
}