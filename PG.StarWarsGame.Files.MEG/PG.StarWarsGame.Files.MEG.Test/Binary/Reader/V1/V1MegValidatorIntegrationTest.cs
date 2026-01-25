using System;
using System.IO;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Binary.V1;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Reader.V1;

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

        const int fileSizeOffset = 74;
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

        const int file2SizeOffset = 94;
        megData[file2SizeOffset] = 0xE9;
        megData[file2SizeOffset + 1] = 0x14;
        megData[file2SizeOffset + 2] = 0x00;
        megData[file2SizeOffset + 3] = 0x00;

        return megData;
    }
}