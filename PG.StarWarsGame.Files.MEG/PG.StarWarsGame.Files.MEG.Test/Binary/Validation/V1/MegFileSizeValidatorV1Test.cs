using System.Collections.Generic;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using PG.StarWarsGame.Files.MEG.Binary.Validation.V1;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Validation.V1;

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

        var sizeInfo = new Mock<IMegBinaryValidationInformation>();
        sizeInfo.SetupGet(s => s.FileSize).Returns(metadata.Size);
        sizeInfo.SetupGet(s => s.BytesRead).Returns(metadata.Size);
        sizeInfo.SetupGet(s => s.Metadata).Returns(metadata);

        var validator = new V1SizeValidator();
        Assert.IsTrue(validator.TestValidate(sizeInfo.Object).IsValid);


        sizeInfo.SetupGet(s => s.FileSize).Returns(metadata.Size + 1);
        sizeInfo.SetupGet(s => s.BytesRead).Returns(metadata.Size);
        Assert.IsFalse(validator.TestValidate(sizeInfo.Object).IsValid);


        sizeInfo.SetupGet(s => s.FileSize).Returns(metadata.Size);
        sizeInfo.SetupGet(s => s.BytesRead).Returns(metadata.Size - 1);
        Assert.IsFalse(validator.TestValidate(sizeInfo.Object).IsValid);
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

        var sizeInfo = new Mock<IMegBinaryValidationInformation>();
        sizeInfo.SetupGet(s => s.FileSize).Returns(metadata.Size);
        sizeInfo.SetupGet(s => s.BytesRead).Returns(metadata.Size);
        sizeInfo.SetupGet(s => s.Metadata).Returns(metadata);

        var validator = new V1SizeValidator();
        Assert.IsTrue(validator.TestValidate(sizeInfo.Object).IsValid);


        sizeInfo.SetupGet(s => s.FileSize).Returns(metadata.Size + 1);
        Assert.IsFalse(validator.TestValidate(sizeInfo.Object).IsValid);
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

        var sizeInfo = new Mock<IMegBinaryValidationInformation>();
        sizeInfo.SetupGet(s => s.FileSize).Returns(metadata.Size + 3 + 5);
        sizeInfo.SetupGet(s => s.BytesRead).Returns(metadata.Size);
        sizeInfo.SetupGet(s => s.Metadata).Returns(metadata);

        var validator = new V1SizeValidator();
        Assert.IsTrue(validator.TestValidate(sizeInfo.Object).IsValid);

        sizeInfo.SetupGet(s => s.FileSize).Returns(metadata.Size + 3 + 5 + 99);
        Assert.IsFalse(validator.TestValidate(sizeInfo.Object).IsValid);
    }
}