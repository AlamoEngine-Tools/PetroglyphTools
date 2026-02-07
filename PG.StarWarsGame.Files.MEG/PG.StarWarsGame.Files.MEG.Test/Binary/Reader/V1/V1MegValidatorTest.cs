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

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Reader.V1;

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
        {
            var exception = Assert.Throws<BinaryCorruptedException>(() =>
                _validator.Validate(metadata, metadata.Size, metadata.Size + 3 + 5 + additionalFileSize));
            Assert.Equal("The size of the MEG file does not match the expected file size.", exception.Message);
        }
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

    [Fact]
    public void Validate_OutOfOrderEntries_Succeeds()
    {
        var header = new MegHeader(2, 2);
        var nameTable = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord>
        {
            MegFileNameTableRecordTest.CreateNameRecord("A"),
            MegFileNameTableRecordTest.CreateNameRecord("B")
        });

        // Metadata size for 2 files: 54
        // B (CRC 1) at offset 64, size 10
        // A (CRC 2) at offset 54, size 10
        var fileTable = new MegFileTable(new List<MegFileTableRecord>
        {
            new(new Crc32(1), 0, 10, 64, 0),
            new(new Crc32(2), 1, 10, 54, 1)
        });
        var metadata = new MegMetadata(header, nameTable, fileTable);

        Assert.DoesNotThrow(() => _validator.Validate(metadata, 54, 74));
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

    [Fact]
    public void Validate_OverlappingEntries_ThrowsBinaryCorruptedException()
    {
        var header = new MegHeader(2, 2);
        var nameTable = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord>
        {
            MegFileNameTableRecordTest.CreateNameRecord("A"),
            MegFileNameTableRecordTest.CreateNameRecord("B")
        });
        
        // Metadata size for 2 files in V1: 8 (header) + 2*2 (name lengths) + 2 (names) + 2*20 (records) = 14 + 40 = 54
        // A: offset 54, size 10. End: 64.
        // B: offset 60, size 10. (Overlaps A)
        var fileTable = new MegFileTable(new List<MegFileTableRecord>
        {
            new(new Crc32(0), 0, 10, 54, 0),
            new(new Crc32(1), 1, 10, 60, 1)
        });
        var metadata = new MegMetadata(header, nameTable, fileTable);
        
        var exception = Assert.Throws<BinaryCorruptedException>(() => 
            _validator.Validate(metadata, 54, 54 + 10 + 10));
        Assert.Equal("The MEG file has overlapping entries.", exception.Message);
    }

    [Fact]
    public void Validate_GapsBetweenEntries_ThrowsBinaryCorruptedException()
    {
        var header = new MegHeader(2, 2);
        var nameTable = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord>
        {
            MegFileNameTableRecordTest.CreateNameRecord("A"),
            MegFileNameTableRecordTest.CreateNameRecord("B")
        });
        
        // A: offset 54, size 10. End: 64.
        // B: offset 65, size 10. (Gap of 1 byte)
        var fileTable = new MegFileTable(new List<MegFileTableRecord>
        {
            new(new Crc32(0), 0, 10, 54, 0),
            new(new Crc32(1), 1, 10, 65, 1)
        });
        var metadata = new MegMetadata(header, nameTable, fileTable);
        
        var exception = Assert.Throws<BinaryCorruptedException>(() => 
            _validator.Validate(metadata, 54, 54 + 10 + 10));
        Assert.Equal("The MEG file has gaps between entries.", exception.Message);
    }

    [Fact]
    public void Validate_EntryExceedsFileSize_ThrowsBinaryCorruptedException()
    {
        var header = new MegHeader(1, 1);
        var nameTable = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord>
        {
            MegFileNameTableRecordTest.CreateNameRecord("A")
        });
        
        // Metadata size for 1 file in V1: 8 + 2 + 1 + 20 = 31
        // A: offset 31, size 10. End: 41.
        // Actual file size: 40.
        var fileTable = new MegFileTable(new List<MegFileTableRecord>
        {
            new(new Crc32(0), 0, 10, 31, 0)
        });
        var metadata = new MegMetadata(header, nameTable, fileTable);
        
        var exception = Assert.Throws<BinaryCorruptedException>(() => 
            _validator.Validate(metadata, 31, 40));
        Assert.Equal("The size of the MEG file does not match the expected file size.", exception.Message);
    }
}