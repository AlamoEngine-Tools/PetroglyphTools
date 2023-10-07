using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Validation;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Validation;

[TestClass]
public class MegFileSizeValidatorV1Test
{
    [TestMethod]
    public void Test__ValidateCore_EmptyMeg()
    {
        var header = new MegHeader(0, 0);
        var nameTable = new MegFileNameTable(new List<MegFileNameTableRecord>());
        var fileTable = new MegFileTable(new List<MegFileTableRecord>());
        var metadata = new MegMetadata(header, nameTable, fileTable);

        var sizeInfo = new Mock<IMegSizeValidationInformation>();
        sizeInfo.SetupGet(s => s.ArchiveSize).Returns(metadata.Size);
        sizeInfo.SetupGet(s => s.BytesRead).Returns(metadata.Size);
        sizeInfo.SetupGet(s => s.Metadata).Returns(metadata);

        var validator = new MegFileSizeValidator();
        Assert.IsTrue(validator.Validate(sizeInfo.Object).IsValid);


        sizeInfo.SetupGet(s => s.ArchiveSize).Returns(metadata.Size + 1);
        sizeInfo.SetupGet(s => s.BytesRead).Returns(metadata.Size);
        Assert.IsFalse(validator.Validate(sizeInfo.Object).IsValid);

        
        sizeInfo.SetupGet(s => s.ArchiveSize).Returns(metadata.Size);
        sizeInfo.SetupGet(s => s.BytesRead).Returns(metadata.Size - 1);
        Assert.IsFalse(validator.Validate(sizeInfo.Object).IsValid);
    }

    [TestMethod]
    public void Test__ValidateCore_OneFileWithEmptyData()
    {
        var header = new MegHeader(1, 1);
        var nameTable = new MegFileNameTable(new List<MegFileNameTableRecord>
        {
            new("A")
        });
        var fileTable = new MegFileTable(new List<MegFileTableRecord>
        {
            new(new Crc32(0), 0, 0, 0, 0 )
        });
        var metadata = new MegMetadata(header, nameTable, fileTable);

        var sizeInfo = new Mock<IMegSizeValidationInformation>();
        sizeInfo.SetupGet(s => s.ArchiveSize).Returns(metadata.Size);
        sizeInfo.SetupGet(s => s.BytesRead).Returns(metadata.Size);
        sizeInfo.SetupGet(s => s.Metadata).Returns(metadata);

        var validator = new MegFileSizeValidator();
        Assert.IsTrue(validator.Validate(sizeInfo.Object).IsValid);


        sizeInfo.SetupGet(s => s.ArchiveSize).Returns(metadata.Size + 1);
        Assert.IsFalse(validator.Validate(sizeInfo.Object).IsValid);
    }

    [TestMethod]
    public void Test__ValidateCore_FilesWithData()
    {
        var header = new MegHeader(2, 2);
        var nameTable = new MegFileNameTable(new List<MegFileNameTableRecord>
        {
            new("A"),
            new("B")
        });
        var fileTable = new MegFileTable(new List<MegFileTableRecord>
        {
            new(new Crc32(0), 0, 3, 0, 0 ),
            new(new Crc32(0), 0, 5, 0, 0 )
        });
        var metadata = new MegMetadata(header, nameTable, fileTable);

        var sizeInfo = new Mock<IMegSizeValidationInformation>();
        sizeInfo.SetupGet(s => s.ArchiveSize).Returns(metadata.Size + 3 + 5);
        sizeInfo.SetupGet(s => s.BytesRead).Returns(metadata.Size);
        sizeInfo.SetupGet(s => s.Metadata).Returns(metadata);

        var validator = new MegFileSizeValidator();
        Assert.IsTrue(validator.Validate(sizeInfo.Object).IsValid);

        sizeInfo.SetupGet(s => s.ArchiveSize).Returns(metadata.Size + 3 + 5 + 99);
        Assert.IsFalse(validator.Validate(sizeInfo.Object).IsValid);
    }
}