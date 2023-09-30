using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Validation;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Validation;

[TestClass]
public class MegFileSizeValidatorTest
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test__Validate_Null()
    {
        var validator = new MegFileSizeValidator();
        validator.Validate((IMegSizeValidationInformation)null!);
    }

    [TestMethod]
    public void Test__Validate_MetadataNull()
    {
        var sizeInfo = new Mock<IMegSizeValidationInformation>();
        sizeInfo.SetupGet(s => s.ArchiveSize).Returns(12);
        sizeInfo.SetupGet(s => s.BytesRead).Returns(1);
        sizeInfo.SetupGet(s => s.Metadata).Returns((IMegFileMetadata)null!);

        var validator = new MegFileSizeValidator();

        Assert.IsFalse(validator.Validate(sizeInfo.Object).IsValid);
    }

    [TestMethod]
    [DataRow(-1L, 1L)]
    [DataRow(0L, 0L)]
    [DataRow(1L, -1L)]
    [DataRow(0L, 1L)]
    [DataRow(1L, 0L)]
    [DataRow(2L, 1L)]
    public void Test__Validate_NotValid(long readBytes, long size)
    {
        var metadata = new Mock<IMegFileMetadata>();
        var sizeInfo = new Mock<IMegSizeValidationInformation>();
        sizeInfo.SetupGet(s => s.ArchiveSize).Returns(size);
        sizeInfo.SetupGet(s => s.BytesRead).Returns(readBytes);
        sizeInfo.SetupGet(s => s.Metadata).Returns(metadata.Object);

        var validator = new MegFileSizeValidator(); 
        
        Assert.IsFalse(validator.Validate(sizeInfo.Object).IsValid);
    }

    [TestMethod]
    [DataRow(1L, 12L)]
    [DataRow(1L, 1L)]
    public void Test__Validate_IsValid(long readBytes, long size)
    {
        var metadata = new Mock<IMegFileMetadata>();
        var sizeInfo = new Mock<IMegSizeValidationInformation>();
        sizeInfo.SetupGet(s => s.ArchiveSize).Returns(size);
        sizeInfo.SetupGet(s => s.BytesRead).Returns(readBytes);
        sizeInfo.SetupGet(s => s.Metadata).Returns(metadata.Object);

        var validator = new MegFileSizeValidator();

        Assert.IsTrue(validator.Validate(sizeInfo.Object).IsValid);
    }
}