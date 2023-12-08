using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
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


    //[TestMethod]
    public void Test_CreateMegArchive()
    {
        var fileParams = new MegFileHolderParam
        {
            FilePath = "a.meg",
            FileVersion = MegFileVersion.V2
        };
        var builderEntries = new List<MegFileDataEntryBuilderInfo>();

        
        var constructingArchive = new Mock<IConstructingMegArchive>();
        constructingArchive.SetupGet(c => c.MegVersion).Returns(MegFileVersion.V2);
        
        _constructingArchiveBuilder.Setup(c => c.BuildConstructingMegArchive(builderEntries))
            .Returns(constructingArchive.Object);

        _binaryServiceFactory.Setup(f => f.GetConstructionBuilder(MegFileVersion.V2))
            .Returns(_constructingArchiveBuilder.Object);


        var megMetadata = new Mock<IMegFileMetadata>();
        _megBinaryConverter.Setup(c => c.ModelToBinary(constructingArchive.Object.Archive))
            .Returns(megMetadata.Object);
        _binaryServiceFactory.Setup(f => f.GetConverter(MegFileVersion.V2))
            .Returns(_megBinaryConverter.Object);
        

        _megFileService.CreateMegArchive(fileParams, builderEntries, false);
    }
}