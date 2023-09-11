using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.IO.Abstractions.TestingHelpers;
using System.IO;
using System;
using System.IO.Abstractions;
using PG.StarWarsGame.Files.MEG.Binary.V1;
using PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.V1;

[TestClass]
public class MegFileSizeValidatorV1IntegrationTest
{
    private MegFileSizeValidatorV1 _service = null!;
    private MegFileBinaryServiceV1 _binaryService = null!;


    [TestInitialize]
    public void SetUp()
    {
        var fs = new MockFileSystem();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);
        _service = new MegFileSizeValidatorV1(sp.Object);

        _binaryService = new MegFileBinaryServiceV1(sp.Object);
    }

    [TestMethod]
    public void Test__ValidateCore_CorrectSize()
    {
        var data = new MemoryStream(MegTestConstants.CONTENT_MEG_FILE_V1);
        var metadata = _binaryService.ReadBinary(data);

        Assert.IsTrue(_service.ValidateCore(data.Position, data.Length, (MegMetadata)metadata));
    }

    [TestMethod]
    public void Test__ValidateCore_IncorrectSize()
    {
        var data = new MemoryStream(MegTestConstants.CONTENT_MEG_FILE_V1);
        var metadata = _binaryService.ReadBinary(data);

        Assert.IsFalse(_service.ValidateCore(metadata.Size, data.Length + 1, (MegMetadata)metadata));
        Assert.IsFalse(_service.ValidateCore(metadata.Size, data.Length + 2, (MegMetadata)metadata));
        Assert.IsFalse(_service.ValidateCore(metadata.Size, data.Length - 1, (MegMetadata)metadata));

        Assert.IsFalse(_service.ValidateCore(metadata.Size - 1, data.Length, (MegMetadata)metadata));
        Assert.IsFalse(_service.ValidateCore(metadata.Size + 1, data.Length, (MegMetadata)metadata));
    }
}