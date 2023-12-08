using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.Testing.Hashing;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Construction;

[TestClass]
public class ConstructingMegArchiveBuilderBaseTest
{
    private Mock<IServiceProvider> _serviceProviderMock = null!;
    private MockFileSystem _fileSystem = null!;

    [TestInitialize]
    public void SetUp()
    {
        _fileSystem = new MockFileSystem();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(_fileSystem);
        sp.Setup(s => s.GetService(typeof(IChecksumService))).Returns(new HashCodeChecksumService());
        _serviceProviderMock = sp;
    }

    [TestMethod]
    public void Test__BuildConstructingMegArchive_ThrowsArgs()
    {
        var serviceMock = new Mock<ConstructingMegArchiveBuilderBase>(_serviceProviderMock.Object)
        {
            CallBase = true
        };

        Assert.ThrowsException<ArgumentNullException>(() => serviceMock.Object.BuildConstructingMegArchive(null!));
    }

    [TestMethod]
    public void Test__BuildConstructingMegArchive_FileNotFound_Throws()
    {
        var serviceMock = new Mock<ConstructingMegArchiveBuilderBase>(_serviceProviderMock.Object)
        {
            CallBase = true
        };

        var builderEntries = new List<MegFileDataEntryBuilderInfo>
        {
            new(new MegDataEntryOriginInfo("a"), "a"),
        };

        Assert.ThrowsException<FileNotFoundException>(() => serviceMock.Object.BuildConstructingMegArchive(builderEntries));
    }


    [TestMethod]
    public void Test__BuildConstructingMegArchive_Empty()
    {
        var serviceMock = new Mock<ConstructingMegArchiveBuilderBase>(_serviceProviderMock.Object)
        {
            CallBase = true
        };

        serviceMock.Protected().SetupGet<MegFileVersion>("FileVersion").Returns(MegFileVersion.V2);
        
        var archive = serviceMock.Object.BuildConstructingMegArchive(new List<MegFileDataEntryBuilderInfo>());
        
        Assert.AreEqual(0, archive.Count);
        Assert.AreEqual(MegFileVersion.V2, archive.MegVersion);
    }


    [TestMethod]
    public void Test__BuildConstructingMegArchive_Normal()
    {
        _fileSystem.AddFile("a", "test data");
        
        var serviceMock = new Mock<ConstructingMegArchiveBuilderBase>(_serviceProviderMock.Object)
        {
            CallBase = true
        };

        var builderEntries = new List<MegFileDataEntryBuilderInfo>
        {
            new(new MegDataEntryOriginInfo("a"), "A")
        };

        var archive = serviceMock.Object.BuildConstructingMegArchive(builderEntries);

        Assert.AreEqual(1, archive.Count);
        
        Assert.AreEqual(false, archive.Encrypted);
        Assert.AreEqual(MegFileVersion.V1, archive.MegVersion);

        Assert.AreEqual(9u, archive.Archive[0].Location.Size);
        Assert.AreEqual(3u, archive.Archive[0].Location.Offset);
        Assert.AreEqual("A", archive.Archive[0].FilePath);
        Assert.AreEqual(new Crc32("A".GetHashCode()), archive.Archive[0].Crc32);
        Assert.AreEqual(false, archive.Archive[0].Encrypted);
    }
}