using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.V1;
using PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.V1;

[TestClass]
public class MegFileSizeValidatorV1Test
{
    private MegFileSizeValidatorV1 _service = null!;

    [TestInitialize]
    public void SetUp()
    {
        var fs = new MockFileSystem();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fs);
        _service = new MegFileSizeValidatorV1(sp.Object);
    }

    [TestMethod]
    public void Test__ValidateCore_EmptyMeg()
    {
        var header = new MegHeader(0, 0);
        var nameTable = new MegFileNameTable(new List<MegFileNameTableRecord>());
        var fileTable = new MegFileTable(new List<MegFileContentTableRecord>());
        var metadata = new MegMetadata(header, nameTable, fileTable);

        Assert.IsTrue(_service.ValidateCore(metadata.Size, metadata.Size, metadata));
        Assert.IsFalse(_service.ValidateCore(metadata.Size + 1, metadata.Size -1, metadata));
    }

    [TestMethod]
    public void Test__ValidateCore_OneFileWithEmptyData()
    {
        var header = new MegHeader(1, 1);
        var nameTable = new MegFileNameTable(new List<MegFileNameTableRecord>
        {
            new("A")
        });
        var fileTable = new MegFileTable(new List<MegFileContentTableRecord>
        {
            new(new Crc32(0), 0, 0, 0, 0 )
        });
        var metadata = new MegMetadata(header, nameTable, fileTable);
        var headerSize = metadata.Size;
        Assert.IsTrue(_service.ValidateCore(headerSize, headerSize, metadata));
        Assert.IsFalse(_service.ValidateCore(headerSize, headerSize  + 1, metadata));
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
        var fileTable = new MegFileTable(new List<MegFileContentTableRecord>
        {
            new(new Crc32(0), 0, 3, 0, 0 ),
            new(new Crc32(0), 0, 5, 0, 0 )
        });
        var metadata = new MegMetadata(header, nameTable, fileTable);

        Assert.IsTrue(_service.ValidateCore(metadata.Size, metadata.Size + 3 + 5, metadata));
    }
}