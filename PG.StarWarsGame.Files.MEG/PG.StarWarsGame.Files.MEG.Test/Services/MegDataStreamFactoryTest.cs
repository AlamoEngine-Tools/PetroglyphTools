using System;
using System.IO;
using System.IO.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using Testably.Abstractions.Testing;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

[TestClass]
public class MegDataStreamFactoryTest
{
    private readonly MockFileSystem _fileSystem = new();
    private MegDataStreamFactory _streamFactory = null!;

    [TestInitialize]
    public void Init()
    {
        var spMock = new Mock<IServiceProvider>();
        spMock.Setup(s => s.GetService(typeof(IFileSystem))).Returns(_fileSystem);
        _streamFactory = new MegDataStreamFactory(spMock.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_Ctor_Throws()
    {
        _ = new MegDataStreamFactory(null!);
    }

    [TestMethod]
    public void Test_GetDataStream_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _streamFactory.GetDataStream((MegDataEntryOriginInfo)null!));
        Assert.ThrowsException<ArgumentNullException>(() => _streamFactory.GetDataStream((MegDataEntryLocationReference)null!));
    }

    [TestMethod]
    public void Test_GetFileData_OriginInfo_Throws_FileNotFound()
    {
       var originInfo = new MegDataEntryOriginInfo("test.txt");
        Assert.ThrowsException<FileNotFoundException>(() => _streamFactory.GetDataStream(originInfo));
    }

    [TestMethod]
    public void Test_GetFileData_OriginInfo_File()
    {
        _fileSystem.Initialize().WithFile("test.txt").Which(m => m.HasBytesContent([1,2,3]));

        var originInfo = new MegDataEntryOriginInfo("test.txt");
        var stream = _streamFactory.GetDataStream(originInfo);
        Assert.AreEqual(3, stream.Length);

        var resultStream = new MemoryStream(new byte[3]);
        stream.CopyTo(resultStream);
        CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, resultStream.ToArray());
    }

    [TestMethod]
    public void Test_GetFileData_OriginInfo_LocationReference()
    {
        _fileSystem.Initialize().WithFile("a.meg").Which(m => m.HasBytesContent([1, 2, 3, 4, 5]));

        var entry = MegDataEntryTest.CreateEntry("file.txt", offset: 1, size: 2);

        var archive = new Mock<IMegArchive>();
        archive.Setup(a => a.Contains(entry)).Returns(true);

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.FilePath).Returns(_fileSystem.Path.GetFullPath("a.meg"));
        meg.SetupGet(m => m.Archive).Returns(archive.Object);

        var originInfo = new MegDataEntryOriginInfo(new MegDataEntryLocationReference(meg.Object, entry));

        var stream = _streamFactory.GetDataStream(originInfo);
        Assert.AreEqual(2, stream.Length);

        var resultStream = new MemoryStream(new byte[2]);
        stream.CopyTo(resultStream);
        CollectionAssert.AreEqual(new byte[] { 2, 3 }, resultStream.ToArray());
    }


    [TestMethod]
    public void Test_GetFileData_LocationReference_Throws_FileNotInMeg()
    {
        _fileSystem.Initialize().WithFile("a.meg");
        var entry = MegDataEntryTest.CreateEntry("file.txt");

        var archive = new Mock<IMegArchive>();
        archive.Setup(a => a.Contains(entry)).Returns(false);

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.FilePath).Returns(_fileSystem.Path.GetFullPath("a.meg"));
        meg.SetupGet(m => m.Archive).Returns(archive.Object);

        var location = new MegDataEntryLocationReference(meg.Object, entry);

        Assert.ThrowsException<FileNotInMegException>(() => _streamFactory.GetDataStream(location));
    }

    [TestMethod]
    public void Test_GetFileData_LocationReference_EmptyData_MegFileNotExists_Throws()
    {
        var entry = MegDataEntryTest.CreateEntry("file.txt", offset: 2, size: 0);

        var archive = new Mock<IMegArchive>();
        archive.Setup(a => a.Contains(entry)).Returns(true);

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.FilePath).Returns(_fileSystem.Path.GetFullPath("other.meg"));
        meg.SetupGet(m => m.Archive).Returns(archive.Object);

        var location = new MegDataEntryLocationReference(meg.Object, entry);

        Assert.ThrowsException<FileNotFoundException>(() => _streamFactory.GetDataStream(location));
    }

    [TestMethod]
    public void Test_GetFileData_LocationReference_EmptyDataFile()
    {
        _fileSystem.Initialize().WithFile("a.meg");
        var entry = MegDataEntryTest.CreateEntry("file.txt", offset: 2, size: 0);

        var archive = new Mock<IMegArchive>();
        archive.Setup(a => a.Contains(entry)).Returns(true);

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.FilePath).Returns(_fileSystem.Path.GetFullPath("a.meg"));
        meg.SetupGet(m => m.Archive).Returns(archive.Object);

        var location = new MegDataEntryLocationReference(meg.Object, entry);

        var stream = _streamFactory.GetDataStream(location);
        Assert.AreEqual(0, stream.Length);
    }

    [TestMethod]
    public void Test_GetFileData_LocationReference_File()
    {
        _fileSystem.Initialize().WithFile("a.meg").Which(m => m.HasBytesContent([1, 2, 3, 4, 5]));

        var entry = MegDataEntryTest.CreateEntry("file.txt", offset: 1, size: 2);

        var archive = new Mock<IMegArchive>();
        archive.Setup(a => a.Contains(entry)).Returns(true);

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.FilePath).Returns(_fileSystem.Path.GetFullPath("a.meg"));
        meg.SetupGet(m => m.Archive).Returns(archive.Object);

        var location = new MegDataEntryLocationReference(meg.Object, entry);

        var stream = _streamFactory.GetDataStream(location);
        Assert.AreEqual(2, stream.Length);

        var resultStream = new MemoryStream(new byte[2]);
        stream.CopyTo(resultStream);
        CollectionAssert.AreEqual(new byte[] { 2, 3 }, resultStream.ToArray());
    }
}