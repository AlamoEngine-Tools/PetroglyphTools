using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

public partial class MegFileServiceTest
{
    [TestMethod]
    public void Test_CreateMegArchive_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _megFileService.CreateMegArchive(null!, MegFileVersion.V1, null, new List<MegFileDataEntryBuilderInfo>()));
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            using var fs = _fileSystem.File.OpenWrite("path");
            _megFileService.CreateMegArchive(fs, MegFileVersion.V3, null, null!);
        }); 
    }

    [TestMethod]
    public void Test_CreateMegArchive_DoesNotCreateDirectories_Throws()
    {
        const string megFileName = "/test/a.meg";
        var metadataBytes = new byte[] { 0, 1, 2 };

        Assert.IsFalse(_fileSystem.Directory.Exists("/test"));

        Assert.ThrowsException<DirectoryNotFoundException>(() => CreateMegArchive(megFileName, metadataBytes, new List<VirtualMegDataEntryReference>()));
    }

    [TestMethod]
    public void Test_CreateMegArchive_WithData()
    {
        const string megFileName = "/a.meg";

        var metadataBytes = new byte[] { 0, 1, 2 };

        const string entryPath = "test.txt";
        var entryBytes = new byte[] { 9, 8, 7 };
        var originInfo = new MegDataEntryOriginInfo(entryPath);

        var items = new List<VirtualMegDataEntryReference>
        {
            new(MegDataEntryTest.CreateEntry(entryPath, default, (uint)metadataBytes.Length, (uint)entryBytes.Length), originInfo)
        };

        _streamFactory.Setup(sf => sf.GetDataStream(originInfo))
            .Returns(new MemoryStream(entryBytes));

        CreateMegArchive(megFileName, metadataBytes, items);

        Assert.IsTrue(_fileSystem.FileExists(megFileName));
        var data = _fileSystem.File.ReadAllBytes(megFileName);

        CollectionAssert.AreEqual(metadataBytes.Concat(entryBytes).ToList(), data);
    }

    [TestMethod]
    [DataRow(3u, 0u)]
    [DataRow(2u, 3u)]
    public void Test_CreateMegArchive_InvalidEntrySizeAndOffsets_Throws(uint size, uint offset)
    {
        const string megFileName = "/a.meg";

        var metadataBytes = new byte[] { 0, 1, 2 };

        const string entryPath = "test.txt";
        var entryBytes = new byte[] { 9, 8, 7 };
        var originInfo = new MegDataEntryOriginInfo(entryPath);

        var items = new List<VirtualMegDataEntryReference>
        {
            new(MegDataEntryTest.CreateEntry(entryPath, default, offset, size), originInfo)
        };

        _streamFactory.Setup(sf => sf.GetDataStream(originInfo))
            .Returns(new MemoryStream(entryBytes));

        Assert.ThrowsException<InvalidOperationException>(() => CreateMegArchive(megFileName, metadataBytes, items));
    }

    [TestMethod]
    public void Test_CreateMegArchive_MegFileExceeds4GB_Throws()
    {
        const string megFileName = "/a.meg";

        var metadataBytes = new byte[] { 0, 1, 2 };

        const string entryPath = "test.txt";
        var entrySize = uint.MaxValue;
        var originInfo = new MegDataEntryOriginInfo(entryPath);

        var items = new List<VirtualMegDataEntryReference>
        {
            new(MegDataEntryTest.CreateEntry(entryPath, default, (uint)metadataBytes.Length, entrySize), originInfo)
        };

        var fakeStream = new Mock<Stream>();
        fakeStream.SetupGet(s => s.CanRead).Returns(true);
        fakeStream.SetupGet(s => s.Length).Returns(entrySize);
        _streamFactory.Setup(sf => sf.GetDataStream(originInfo))
            .Returns(fakeStream.Object);

        Assert.ThrowsException<NotSupportedException>(() => CreateMegArchive(megFileName, metadataBytes, items));
    }

    private void CreateMegArchive(string megFileName, byte[] metadataBytes, IList<VirtualMegDataEntryReference> items)
    {
        var builderEntries = new List<MegFileDataEntryBuilderInfo>();

        var constructingArchive = new Mock<IConstructingMegArchive>();
        constructingArchive.SetupGet(c => c.MegVersion).Returns(MegFileVersion.V2);
        constructingArchive.Setup(ca => ca.GetEnumerator()).Returns(items.GetEnumerator());
        
        _constructingArchiveBuilder.Setup(c => c.BuildConstructingMegArchive(builderEntries))
            .Returns(constructingArchive.Object);

        _binaryServiceFactory.Setup(f => f.GetConstructionBuilder(MegFileVersion.V2))
            .Returns(_constructingArchiveBuilder.Object);

        var megMetadata = new Mock<IMegFileMetadata>();
        megMetadata.SetupGet(m => m.Bytes).Returns(metadataBytes);
        megMetadata.SetupGet(m => m.Size).Returns(metadataBytes.Length);
        
        _megBinaryConverter.Setup(c => c.ModelToBinary(constructingArchive.Object.Archive))
            .Returns(megMetadata.Object);
        
        _binaryServiceFactory.Setup(f => f.GetConverter(MegFileVersion.V2))
            .Returns(_megBinaryConverter.Object);

        using var fs = _fileSystem.File.OpenWrite(megFileName);
        _megFileService.CreateMegArchive(fs, MegFileVersion.V2, null, builderEntries);
    }
}