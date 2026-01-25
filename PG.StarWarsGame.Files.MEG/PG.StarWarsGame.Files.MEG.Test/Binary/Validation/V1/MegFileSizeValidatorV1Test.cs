using System;
using System.Collections.Generic;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using PG.StarWarsGame.Files.MEG.Test.Binary.Metadata;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Validation.V1;

public class MegFileSizeValidatorV1Test : MegFileSizeValidatorTestBase
{
    private protected override IMegFileMetadata CreateRandomMetadata()
    {
        var random = new Random();

        var numFiles = (uint)random.Next(0, 100);

        var header = new MegHeader(numFiles, numFiles);

        var fileNames = new List<MegFileNameTableRecord>();
        var records = new List<MegFileTableRecord>();
        for (uint i = 0; i < numFiles; i++)
        {
            var path = FileSystem.Path.GetRandomFileName();
            fileNames.Add(new MegFileNameTableRecord(path, path));

            var size = (uint)random.Next(1, 255);
            records.Add(new MegFileTableRecord(new Crc32(i), i, size, size, i));
        }

        return new MegMetadata(header, 
            new BinaryTable<MegFileNameTableRecord>(fileNames), 
            new MegFileTable(records));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    public void Validate_EmptyMeg(int additionalMetadataBytesRead)
    {
        var header = new MegHeader(0, 0);
        var nameTable = new BinaryTable<MegFileNameTableRecord>(new List<MegFileNameTableRecord>());
        var fileTable = new MegFileTable(new List<MegFileTableRecord>());
        var metadata = new MegMetadata(header, nameTable, fileTable);

        var info = new MegBinaryValidationInformation
        {
            Metadata = metadata,
            BytesRead = metadata.Size + additionalMetadataBytesRead,
            FileSize = metadata.Size
        };

        var validator = new MegFileSizeValidator();

        if (additionalMetadataBytesRead == 0)
            Assert.True(validator.Validate(info));
        else
            Assert.False(validator.Validate(info));
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

        var info = new MegBinaryValidationInformation
        {
            Metadata = metadata,
            BytesRead = metadata.Size,
            FileSize = metadata.Size
        };

        var validator = new MegFileSizeValidator();
        Assert.True(validator.Validate(info));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    public void Validate_FilesWithData(int additionalFileSize)
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

        var info = new MegBinaryValidationInformation
        {
            Metadata = metadata,
            BytesRead = metadata.Size,
            FileSize = metadata.Size + 3 + 5 + additionalFileSize
        };

        var validator = new MegFileSizeValidator();

        if (additionalFileSize == 0)
            Assert.True(validator.Validate(info));
        else
            Assert.False(validator.Validate(info));
    }
}