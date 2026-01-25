using System;
using System.Collections.Generic;
using AnakinRaW.CommonUtilities.Testing.Extensions;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Binary.V1;
using PG.StarWarsGame.Files.MEG.Test.Binary.Metadata;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Validation.V1;

public class V1MegValidatorTest : CommonMegTestBase
{
    private readonly V1MegValidator _validator;

    public V1MegValidatorTest()
    {
        _validator = new V1MegValidator(ServiceProvider);
    }

    [Fact]
    public void Validate_NullMetadata_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _validator.Validate(null!, 1, 12));
    }

    [Fact]
    public void Validate_OneFileWithEmptyData()
    {
        var header = new MegHeader(1, 1);
        var nameTable = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord>
        {
            MegFileNameTableRecordTest.CreateNameRecord("A")
        });
        var fileTable = new MegFileTable(new List<MegFileTableRecord>
        {
            new(new Crc32(0), 0, 0, 0, 0 )
        });
        var metadata = new MegMetadata(header, nameTable, fileTable);

        Assert.DoesNotThrow(() => _validator.Validate(metadata, metadata.Size, metadata.Size));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    public void Validate_ActualFileSizeAsExpected(int additionalFileSize)
    {
        var header = new MegHeader(2, 2);
        var nameTable = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord>
        {
            MegFileNameTableRecordTest.CreateNameRecord("A"),
            MegFileNameTableRecordTest.CreateNameRecord("B")
        });
        var fileTable = new MegFileTable(new List<MegFileTableRecord>
        {
            new(new Crc32(0), 0, 3, 0, 0 ),
            new(new Crc32(0), 0, 5, 0, 0 )
        });
        var metadata = new MegMetadata(header, nameTable, fileTable);

        if (additionalFileSize == 0)
            Assert.DoesNotThrow(() => _validator.Validate(metadata, metadata.Size, metadata.Size + 3 + 5 + additionalFileSize));
        else
            Assert.Throws<BinaryCorruptedException>(() =>
                _validator.Validate(metadata, metadata.Size, metadata.Size + 3 + 5 + additionalFileSize));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    public void Validate_ActualMetadataSizeAsExpected(int additionalMetadataBytesRead)
    {
        var header = new MegHeader(0, 0);
        var nameTable = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord>());
        var fileTable = new MegFileTable(new List<MegFileTableRecord>());
        var metadata = new MegMetadata(header, nameTable, fileTable);

        if (additionalMetadataBytesRead == 0)
            Assert.DoesNotThrow(() => _validator.Validate(metadata, metadata.Size + additionalMetadataBytesRead, metadata.Size));
        else
            Assert.Throws<BinaryCorruptedException>(() =>
                _validator.Validate(metadata, metadata.Size + additionalMetadataBytesRead, metadata.Size));
    }

    [Theory]
    [InlineData(-1L, 1L)]
    [InlineData(1L, -1L)]
    [InlineData(0L, 1L)]
    [InlineData(1L, 0L)]
    [InlineData(2L, 1L)]
    public void Validate_MetadataSizeMismatch_ThrowsBinaryCorruptedException(long actualMetadataSize, long actualFileSize)
    {
        var header = new MegHeader(1, 1);
        var nameTable = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord>
        {
            MegFileNameTableRecordTest.CreateNameRecord("A")
        });
        var fileTable = new MegFileTable(new List<MegFileTableRecord>
        {
            new(new Crc32(0), 0, 0, 0, 0)
        });
        var metadata = new MegMetadata(header, nameTable, fileTable);

        Assert.Throws<BinaryCorruptedException>(() => _validator.Validate(metadata, actualMetadataSize, actualFileSize));
    }
}