using System;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Validation;

public class MegBinaryValidatorTest : CommonMegTestBase
{
    [Fact]
    public void Validate_Composite_Valid()
    {
        var validator = new MegBinaryValidator(ServiceProvider);

        // true | true --> true
        Assert.True(validator.Validate(new MegBinaryValidationInformation
        {
            Metadata = new MegMetadata(default, new BinaryTable<MegFileNameTableRecord>([]), new MegFileTable([])),
            BytesRead = 8, // Empty file size
            FileSize = 8 // Empty file size
        }));
    }

    [Fact]
    public void Validate_Composite_Invalid1()
    {
        var validator = new MegBinaryValidator(ServiceProvider);

        // false | true --> false
        Assert.False(validator.Validate(new MegBinaryValidationInformation
        {
            Metadata = new MegMetadata(default, new BinaryTable<MegFileNameTableRecord>([]), new MegFileTable([])),
            BytesRead = 0,
            FileSize = 8 // Empty file size
        }));
    }

    [Fact]
    public void Validate_Composite_Invalid2()
    {
        var validator = new MegBinaryValidator(ServiceProvider);

        // true | false --> false
        Assert.False(validator.Validate(new MegBinaryValidationInformation
        {
            Metadata = new MegMetadata(
                new MegHeader(2, 2),
                new BinaryTable<MegFileNameTableRecord>([
                    new MegFileNameTableRecord("a", "a"),
                    new MegFileNameTableRecord("b", "b")
                ]),
                new MegFileTable([
                    new MegFileTableRecord(new Crc32(1), 1u, 0, 8 + 40 + 6, 1),
                    new MegFileTableRecord(new Crc32(0), 0u, 0, 8 + 40 + 6, 0),
                ])),
            BytesRead = 8 + 40 + 6,
            FileSize = 8 + 40 + 6
        }));
    }

    [Fact]
    public void Validate_Composite_Invalid3()
    {
        var validator = new MegBinaryValidator(ServiceProvider);

        // true | false --> false
        Assert.False(validator.Validate(new MegBinaryValidationInformation
        {
            Metadata = new MegMetadata(
                new MegHeader(2, 2),
                new BinaryTable<MegFileNameTableRecord>([
                    new MegFileNameTableRecord("a", "a"),
                    new MegFileNameTableRecord("b", "b")
                ]),
                new MegFileTable([
                    new MegFileTableRecord(new Crc32(1), 1u, 0, 8 + 40 + 6, 1),
                    new MegFileTableRecord(new Crc32(0), 0u, 0, 8 + 40 + 6, 0),
                ])),
            BytesRead = 0,
            FileSize = 8 + 40 + 6
        }));
    }
}