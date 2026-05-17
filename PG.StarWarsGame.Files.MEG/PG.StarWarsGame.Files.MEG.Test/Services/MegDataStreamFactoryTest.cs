using System;
using System.IO;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

public class MegDataStreamFactoryTest : CommonMegTestBase
{ 
    private readonly MegDataStreamFactory _streamFactory;

    public MegDataStreamFactoryTest()
    {
        _streamFactory = new MegDataStreamFactory(ServiceProvider);
    }

    [Fact]
    public void Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new MegDataStreamFactory(null!));
    }

    [Fact]
    public void GetDataStream_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _streamFactory.GetStream((MegDataEntryOriginInfo)null!));
        Assert.Throws<ArgumentNullException>(() => _streamFactory.GetStream((MegDataEntryLocationReference)null!));
    }

    [Fact]
    public void GetFileData_OriginInfo_Throws_FileNotFound()
    {
       var originInfo = new MegDataEntryOriginInfo(FileSystem.FileInfo.New("test.txt"));
        Assert.Throws<FileNotFoundException>(() => _streamFactory.GetStream(originInfo));
    }

    [Fact]
    public void GetFileData_OriginInfo_File()
    {
        FileSystem.Initialize().WithFile("test.txt").Which(m => m.HasBytesContent([1,2,3]));

        var originInfo = new MegDataEntryOriginInfo(FileSystem.FileInfo.New("test.txt"));
        var stream = _streamFactory.GetStream(originInfo);
        Assert.Equal(3, stream.Length);

        var resultStream = new MemoryStream(new byte[3]);
        stream.CopyTo(resultStream);
        Assert.Equal([1, 2, 3], resultStream.ToArray());
    }

    [Fact]
    public void GetFileData_OriginInfo_LocationReference()
    {
        FileSystem.Initialize().WithFile("a.meg").Which(m => m.HasBytesContent([1, 2, 3, 4, 5]));

        var entry = MegDataEntryTest.CreateEntry("file.txt", offset: 1, size: 2);

        var archive = new MegArchive([entry]);

        var meg = new MegFile(archive, new MegFileInformation("a.meg", MegFileVersion.V1), ServiceProvider);

        var originInfo = new MegDataEntryOriginInfo(new MegDataEntryLocationReference(meg, entry));

        var stream = _streamFactory.GetStream(originInfo);
        Assert.Equal(2, stream.Length);

        var resultStream = new MemoryStream(new byte[2]);
        stream.CopyTo(resultStream);
        Assert.Equal([2, 3], resultStream.ToArray());
    }


    [Fact]
    public void GetFileData_LocationReference_Throws_FileNotInMeg()
    {
        FileSystem.Initialize().WithFile("a.meg");
        var entry = MegDataEntryTest.CreateEntry("file.txt");

        var archive = new MegArchive([]);

        var meg = new MegFile(archive, new MegFileInformation("a.meg", MegFileVersion.V1),
            ServiceProvider);

        var location = new MegDataEntryLocationReference(meg, entry);

        Assert.Throws<EntryNotInMegException>(() => _streamFactory.GetStream(location));
    }

    [Fact]
    public void GetFileData_LocationReference_EmptyData_MegFileNotExists_Throws()
    {
        FileSystem.Initialize().WithFile("a.meg");

        var entry = MegDataEntryTest.CreateEntry("file.txt", offset: 2, size: 0);

        var archive = new MegArchive([entry]);

        var meg = new MegFile(archive, new MegFileInformation("a.meg", MegFileVersion.V1),
            ServiceProvider);

        FileSystem.File.Delete("a.meg");

        var location = new MegDataEntryLocationReference(meg, entry);

        Assert.Throws<FileNotFoundException>(() => _streamFactory.GetStream(location));
    }

    [Fact]
    public void GetFileData_LocationReference_EmptyDataFile()
    {
        FileSystem.Initialize().WithFile("a.meg");
        var entry = MegDataEntryTest.CreateEntry("file.txt", offset: 2, size: 0);

        var archive = new MegArchive([entry]);

        var meg = new MegFile(archive, new MegFileInformation("a.meg", MegFileVersion.V1),
            ServiceProvider);

        var location = new MegDataEntryLocationReference(meg, entry);

        var stream = _streamFactory.GetStream(location);
        Assert.Equal(0, stream.Length);
    }

    [Fact]
    public void GetFileData_OriginInfo_Bytes()
    {
        var bytes = new byte[] { 10, 20, 30, 40, 50 };
        var originInfo = new MegDataEntryOriginInfo(bytes);

        using var resultStream = _streamFactory.GetStream(originInfo);
        Assert.Equal(5, resultStream.Length);

        var sink = new MemoryStream();
        resultStream.CopyTo(sink);
        Assert.Equal(bytes, sink.ToArray());
    }

    [Fact]
    public void GetFileData_OriginInfo_Bytes_Replayable()
    {
        var bytes = new byte[] { 10, 20, 30, 40, 50 };
        var originInfo = new MegDataEntryOriginInfo(bytes);

        using (var first = _streamFactory.GetStream(originInfo))
            first.CopyTo(new MemoryStream());

        using var second = _streamFactory.GetStream(originInfo);
        var sink = new MemoryStream();
        second.CopyTo(sink);
        Assert.Equal(bytes, sink.ToArray());
    }

    [Fact]
    public void GetFileData_OriginInfo_Bytes_Empty()
    {
        var originInfo = new MegDataEntryOriginInfo(Array.Empty<byte>());

        using var resultStream = _streamFactory.GetStream(originInfo);
        Assert.Equal(0, resultStream.Length);
    }

    [Fact]
    public void GetFileData_LocationReference_File()
    {
        FileSystem.Initialize().WithFile("a.meg").Which(m => m.HasBytesContent([1, 2, 3, 4, 5]));

        var entry = MegDataEntryTest.CreateEntry("file.txt", offset: 1, size: 2);

        var archive = new MegArchive([entry]);

        var meg = new MegFile(archive, new MegFileInformation("a.meg", MegFileVersion.V1),
            ServiceProvider);

        var location = new MegDataEntryLocationReference(meg, entry);

        var stream = _streamFactory.GetStream(location);
        Assert.Equal(2, stream.Length);

        var resultStream = new MemoryStream(new byte[2]);
        stream.CopyTo(resultStream);
        Assert.Equal([2, 3], resultStream.ToArray());
    }
}