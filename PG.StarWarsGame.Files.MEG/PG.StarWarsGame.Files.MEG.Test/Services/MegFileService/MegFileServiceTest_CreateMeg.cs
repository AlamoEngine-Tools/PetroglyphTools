using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

public partial class MegFileServiceTest
{
    [TestMethod]
    public void Test_CreateMegArchive_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _megFileService.CreateMegArchive(null!, new List<MegFileDataEntryBuilderInfo>(), false));
        Assert.ThrowsException<ArgumentNullException>(() => _megFileService.CreateMegArchive(new MegFileHolderParam{ FilePath = "Path"}, null!, false));
        Assert.ThrowsException<ArgumentException>(() => _megFileService.CreateMegArchive(new MegFileHolderParam{ FilePath = null!}, new List<MegFileDataEntryBuilderInfo>(), false));
        Assert.ThrowsException<ArgumentException>(() => _megFileService.CreateMegArchive(new MegFileHolderParam{ FilePath = ""}, new List<MegFileDataEntryBuilderInfo>(), false));
        Assert.ThrowsException<ArgumentException>(() => _megFileService.CreateMegArchive(new MegFileHolderParam{ FilePath = "   "}, new List<MegFileDataEntryBuilderInfo>(), false));
    }


    [TestMethod]
    public void Test_CreateMegArchive_Override()
    {
        var items = new List<VirtualMegDataEntryReference>()
        {
           // new VirtualMegDataEntryReference()
        };
        
        var fileParams = new MegFileHolderParam
        {
            FilePath = "a.meg",
            FileVersion = MegFileVersion.V2
        };
        var builderEntries = new List<MegFileDataEntryBuilderInfo>();

        var constructingArchive = new Mock<IConstructingMegArchive>();
        constructingArchive.SetupGet(c => c.MegVersion).Returns(MegFileVersion.V2);
        constructingArchive.Setup(ca => ca.GetEnumerator()).Returns(items.GetEnumerator());
        
        _constructingArchiveBuilder.Setup(c => c.BuildConstructingMegArchive(builderEntries))
            .Returns(constructingArchive.Object);

        _binaryServiceFactory.Setup(f => f.GetConstructionBuilder(MegFileVersion.V2))
            .Returns(_constructingArchiveBuilder.Object);

        var megMetadata = new Mock<IMegFileMetadata>();
        megMetadata.SetupGet(m => m.Bytes).Returns([0, 1, 2]);
        megMetadata.SetupGet(m => m.Size).Returns(3);
        
        _megBinaryConverter.Setup(c => c.ModelToBinary(constructingArchive.Object.Archive))
            .Returns(megMetadata.Object);
        
        _binaryServiceFactory.Setup(f => f.GetConverter(MegFileVersion.V2))
            .Returns(_megBinaryConverter.Object);

        
        _fileSystem.AddFile("a.meg", null);
        Assert.ThrowsException<IOException>(() => _megFileService.CreateMegArchive(fileParams, builderEntries, false));

        _megFileService.CreateMegArchive(fileParams, builderEntries, true);

        Assert.IsTrue(_fileSystem.FileExists("a.meg"));
        var data = _fileSystem.File.ReadAllBytes("a.meg");

        CollectionAssert.AreEqual(new byte[] { }, data);
    }
}