using System;
using System.IO;
using System.IO.Abstractions;

using Moq;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services;


public class MegDataStreamFactoryTest
{
    private readonly MockFileSystem _fileSystem = new();
    private readonly MegDataStreamFactory _streamFactory;

    public MegDataStreamFactoryTest()
    {
        var spMock = new Mock<IServiceProvider>();
        spMock.Setup(s => s.GetService(typeof(IFileSystem))).Returns(_fileSystem);
        _streamFactory = new MegDataStreamFactory(spMock.Object);
    }

    [Fact]
    public void Test_Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new MegDataStreamFactory(null!));
    }

    [Fact]
    public void Test_GetDataStream_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _streamFactory.GetDataStream((MegDataEntryOriginInfo)null!));
        Assert.Throws<ArgumentNullException>(() => _streamFactory.GetDataStream((MegDataEntryLocationReference)null!));
    }

    [Fact]
    public void Test_GetFileData_OriginInfo_Throws_FileNotFound()
    {
       var originInfo = new MegDataEntryOriginInfo("test.txt");
        Assert.Throws<FileNotFoundException>(() => _streamFactory.GetDataStream(originInfo));
    }

    [Fact]
    public void Test_GetFileData_OriginInfo_File()
    {
        _fileSystem.Initialize().WithFile("test.txt").Which(m => m.HasBytesContent([1,2,3]));

        var originInfo = new MegDataEntryOriginInfo("test.txt");
        var stream = _streamFactory.GetDataStream(originInfo);
        Assert.Equal(3, stream.Length);

        var resultStream = new MemoryStream(new byte[3]);
        stream.CopyTo(resultStream);
        Assert.Equal([1, 2, 3], resultStream.ToArray());
    }

    [Fact]
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
        Assert.Equal(2, stream.Length);

        var resultStream = new MemoryStream(new byte[2]);
        stream.CopyTo(resultStream);
        Assert.Equal([2, 3], resultStream.ToArray());
    }


    [Fact]
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

        Assert.Throws<FileNotInMegException>(() => _streamFactory.GetDataStream(location));
    }

    [Fact]
    public void Test_GetFileData_LocationReference_EmptyData_MegFileNotExists_Throws()
    {
        var entry = MegDataEntryTest.CreateEntry("file.txt", offset: 2, size: 0);

        var archive = new Mock<IMegArchive>();
        archive.Setup(a => a.Contains(entry)).Returns(true);

        var meg = new Mock<IMegFile>();
        meg.SetupGet(m => m.FilePath).Returns(_fileSystem.Path.GetFullPath("other.meg"));
        meg.SetupGet(m => m.Archive).Returns(archive.Object);

        var location = new MegDataEntryLocationReference(meg.Object, entry);

        Assert.Throws<FileNotFoundException>(() => _streamFactory.GetDataStream(location));
    }

    [Fact]
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
        Assert.Equal(0, stream.Length);
    }

    [Fact]
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
        Assert.Equal(2, stream.Length);

        var resultStream = new MemoryStream(new byte[2]);
        stream.CopyTo(resultStream);
        Assert.Equal([2, 3], resultStream.ToArray());
    }
}