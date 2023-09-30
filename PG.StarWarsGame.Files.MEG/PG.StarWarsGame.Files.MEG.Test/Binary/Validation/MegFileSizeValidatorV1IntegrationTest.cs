using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO.Abstractions.TestingHelpers;
using System.IO;
using System;
using System.IO.Abstractions;
using PG.StarWarsGame.Files.MEG.Binary.V1;
using PG.StarWarsGame.Files.MEG.Binary.Validation;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Validation;

[TestClass]
public class MegFileSizeValidatorV1IntegrationTest
{
    private readonly MegFileSizeValidator _validator = new();
    private MegFileBinaryServiceV1 _binaryService = null!;


    [TestInitialize]
    public void SetUp()
    {
        var fs = new MockFileSystem();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);

        _binaryService = new MegFileBinaryServiceV1(sp.Object);
    }

    [TestMethod]
    public void Test__ValidateCore_CorrectSize()
    {
        var data = new MemoryStream(MegTestConstants.CONTENT_MEG_FILE_V1);
        var metadata = _binaryService.ReadBinary(data);

        var sizeInfo = new MegSizeValidationInformation
        {
            BytesRead = data.Position,
            ArchiveSize = data.Length,
            Metadata = metadata
        };

        Assert.IsTrue(_validator.Validate(sizeInfo).IsValid);
    }

    [TestMethod]
    [DataRow(0, 1)]
    [DataRow(0, 2)]
    [DataRow(0, -1)]
    [DataRow(-1, 0)]
    [DataRow(1, 0)]
    public void Test__ValidateCore_IncorrectSize(int offsetBytesRead, int offsetArchiveSize)
    {
        var data = new MemoryStream(MegTestConstants.CONTENT_MEG_FILE_V1);
        var metadata = _binaryService.ReadBinary(data);

        var sizeInfo = new MegSizeValidationInformation
        {
            BytesRead = data.Position + offsetBytesRead,
            ArchiveSize = data.Length + offsetArchiveSize,
            Metadata = metadata
        };

        Assert.IsFalse(_validator.Validate(sizeInfo).IsValid);
        Assert.IsFalse(_validator.Validate(sizeInfo).IsValid);
        Assert.IsFalse(_validator.Validate(sizeInfo).IsValid);

        Assert.IsFalse(_validator.Validate(sizeInfo).IsValid);
        Assert.IsFalse(_validator.Validate(sizeInfo).IsValid);
    }
}