using System;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Validation;

public abstract class MegFileSizeValidatorTestBase
{
    protected readonly MockFileSystem FileSystem = new();

    private protected abstract IMegFileMetadata CreateRandomMetadata();

    [Fact]
    public void Validate_Null_Throws()
    {
        var validator = new MegFileSizeValidator();
        Assert.Throws<ArgumentNullException>(() => validator.Validate(null!));
    }

    [Fact]
    public void Validate_MetadataNull()
    {
        var validator = new MegFileSizeValidator();

        Assert.False(validator.Validate(new MegBinaryValidationInformation
        {
            Metadata = null!,
            BytesRead = 1,
            FileSize = 12
        }));
    }

    [Theory]
    [InlineData(-1L, 1L)]
    [InlineData(0L, 0L)]
    [InlineData(1L, -1L)]
    [InlineData(0L, 1L)]
    [InlineData(1L, 0L)]
    [InlineData(2L, 1L)]
    public void Validate_AlwaysInvalid(long readBytes, long size)
    {
        var info = new MegBinaryValidationInformation
        {
            // Metadata should not matter, as the input data is already invalid
            Metadata = CreateRandomMetadata(),
            BytesRead = readBytes,
            FileSize = size
        };

        var validator = new MegFileSizeValidator();

        Assert.False(validator.Validate(info));
    }
}