using System;
using System.IO;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Binary.V1;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Validation.V1;

public class V1MegValidatorIntegrationTest : CommonMegTestBase
{
    private readonly V1MegValidator _validator;
    private readonly MegFileBinaryReaderV1 _binaryReader;

    public V1MegValidatorIntegrationTest()
    {
        _validator = new V1MegValidator(ServiceProvider);
        _binaryReader = new MegFileBinaryReaderV1(ServiceProvider);
    }

    [Fact]
    public void Validate_ValidMegFile_Succeeds()
    {
        var data = new MemoryStream(MegTestConstants.ContentMegFileV1);
        var metadata = (MegMetadata)_binaryReader.ReadBinary(data);

        _validator.Validate(metadata, data.Position, data.Length);
    }

    [Fact]
    public void Validate_MegFileWithIncorrectFileSizeInFileTable_ThrowsBinaryCorruptedException()
    {
        var invalidMegData = CreateMegWithIncorrectFileSize();
        var data = new MemoryStream(invalidMegData);

        var exception = Assert.Throws<BinaryCorruptedException>(() => _binaryReader.ReadBinary(data));
        Assert.Contains("does not match the expected file size", exception.Message);
    }

    [Fact]
    public void Validate_MegFileWithTruncatedData_ThrowsBinaryCorruptedException()
    {
        var invalidMegData = CreateMegWithTruncatedData();
        var data = new MemoryStream(invalidMegData);

        var exception = Assert.Throws<BinaryCorruptedException>(() => _binaryReader.ReadBinary(data));
        Assert.Contains("The size of the MEG file does not match the expected file size.", exception.Message);
    }

    private static byte[] CreateMegWithIncorrectFileSize()
    {
        var megData = new byte[MegTestConstants.ContentMegFileV1.Length];
        Array.Copy(MegTestConstants.ContentMegFileV1, megData, megData.Length);

        // MEG structure: Header(8) + FileNameTable(66) + FileTable(40) + FileData
        // File table starts at offset 66 (after header and file name table)
        // Each file table record is 20 bytes: CRC32(4) + Index(4) + FileSize(4) + Offset(4) + NameIndex(4)
        // First file record starts at offset 66
        // FileSize field is at offset 66 + 8 = 74
        
        // Original: File1.Size=377, File2.Size=5453, Total=5830
        // Change File1.Size from 377 to 500 (increase by 123)
        // This makes the sum of file sizes = 500 + 5453 = 5953
        // Expected archive size = 106 (metadata) + 5953 = 6059
        // Actual archive size = 106 + 5830 = 5936
        // Validator will detect: expectedArchiveSize (6059) != actualFileSize (5936)
        
        const int fileSizeOffset = 74;

        // 500(dec) = 1F4(hex) in little-endian
        megData[fileSizeOffset] = 0xF4; 
        megData[fileSizeOffset + 1] = 0x01;
        megData[fileSizeOffset + 2] = 0x00;
        megData[fileSizeOffset + 3] = 0x00;

        return megData;
    }

    private static byte[] CreateMegWithTruncatedData()
    {
        var megData = new byte[MegTestConstants.ContentMegFileV1.Length];
        Array.Copy(MegTestConstants.ContentMegFileV1, megData, megData.Length);

        // MEG structure: metadata ends at offset 106, file data follows
        // File1: offset=106, size=377
        // File2: offset=483, size=5453
        // Total file size should be: 106 + 377 + 5453 = 5936
        
        // Reduce File2.Size from 5453 to 5353 (reduce by 100)
        // This makes sum of file sizes = 377 + 5353 = 5730
        // Expected archive size = 106 + 5730 = 5836
        // Actual archive size = 5936
        // Validator will detect: expectedArchiveSize (5836) != actualFileSize (5936)
        
        const int file2SizeOffset = 94; // Second file record at offset 86, FileSize at +8

        // 5353(dec) = 14E9(hex) in little-endian
        megData[file2SizeOffset] = 0xE9; 
        megData[file2SizeOffset + 1] = 0x14;
        megData[file2SizeOffset + 2] = 0x00;
        megData[file2SizeOffset + 3] = 0x00;

        return megData;
    }
}