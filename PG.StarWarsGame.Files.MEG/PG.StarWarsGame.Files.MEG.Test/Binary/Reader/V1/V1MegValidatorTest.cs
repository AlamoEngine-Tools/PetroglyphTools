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
            new(new Crc32(0), 0, 3, 54, 0 ),
            new(new Crc32(0), 1, 5, 54 + 3, 1 )
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

    [Fact]
    public void Validate_FileNameIndexOutOfRange_ThrowsBinaryCorruptedException()
    {
        var header = new MegHeader(1, 1);
        var nameTable = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord>
        {
            MegFileNameTableRecordTest.CreateNameRecord("A")
        });
        var fileTable = new MegFileTable(new List<MegFileTableRecord>
        {
            new(new Crc32(1), 0, 10, 54, 1)
        });
        var metadata = new MegMetadata(header, nameTable, fileTable);

        var exception = Assert.Throws<BinaryCorruptedException>(() => _validator.Validate(metadata, metadata.Size, metadata.Size + 10));
        Assert.Equal("File record (CRC: 1) has an out-of-range filename index: 1.", exception.Message);
    }

    [Fact]
    public void Validate_DuplicateFileNameIndex_ThrowsBinaryCorruptedException()
    {
        var header = new MegHeader(2, 2);
        var nameTable = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord>
        {
            MegFileNameTableRecordTest.CreateNameRecord("A"),
            MegFileNameTableRecordTest.CreateNameRecord("B")
        });
        var fileTable = new MegFileTable(new List<MegFileTableRecord>
        {
            new(new Crc32(1), 0, 10, 54, 0),
            new(new Crc32(2), 1, 10, 64, 0)
        });
        var metadata = new MegMetadata(header, nameTable, fileTable);

        var exception = Assert.Throws<BinaryCorruptedException>(() => _validator.Validate(metadata, metadata.Size, metadata.Size + 20));
        Assert.Equal("File record (CRC: 2) has a duplicate filename index: 0.", exception.Message);
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
    [Fact]
    public void Validate_FileOffsetInMetadata_ThrowsBinaryCorruptedException()
    {
        var header = new MegHeader(1, 1);
        var nameTable = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord>
        {
            MegFileNameTableRecordTest.CreateNameRecord("A")
        });

        // Metadata size: 8 + (2 + 1) + 20 = 31
        // File offset set to 30 (within metadata)
        var fileTable = new MegFileTable(new List<MegFileTableRecord>
        {
            new(new Crc32(0), 0, 10, 30, 0)
        });
        var metadata = new MegMetadata(header, nameTable, fileTable);

        var exception = Assert.Throws<BinaryCorruptedException>(() =>
            _validator.Validate(metadata, 31, 41));
        Assert.Equal("The content of file record (CRC: 0) starts within the metadata.", exception.Message);
    }

    [Fact]
    public void Validate_OverlappingFiles_ThrowsBinaryCorruptedException()
    {
        var header = new MegHeader(2, 2);
        var nameTable = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord>
        {
            MegFileNameTableRecordTest.CreateNameRecord("A"),
            MegFileNameTableRecordTest.CreateNameRecord("B")
        });

        // Metadata size: 8 + (2 + 1) + (2 + 1) + 2 * 20 = 54
        // File A: offset 54, size 10
        // File B: offset 60, size 10 -> overlaps with A
        var fileTable = new MegFileTable(new List<MegFileTableRecord>
        {
            new(new Crc32(1), 0, 10, 54, 0),
            new(new Crc32(2), 1, 10, 60, 1)
        });
        var metadata = new MegMetadata(header, nameTable, fileTable);

        var exception = Assert.Throws<BinaryCorruptedException>(() =>
            _validator.Validate(metadata, 54, 74));
        Assert.Equal("The content of file record (CRC: 2) overlaps with the content of a previous record.", exception.Message);
    }
}