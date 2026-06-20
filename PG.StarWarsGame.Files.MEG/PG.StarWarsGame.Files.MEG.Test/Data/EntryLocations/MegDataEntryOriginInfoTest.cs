using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using PG.Testing;
using System;
using System.IO;
using System.IO.Abstractions;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Data.EntryLocations;

public class MegDataEntryOriginInfoTest : PGTestBase
{
    [Fact]
    public void Ctor_InvalidArgs_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new MegDataEntryOriginInfo((IFileInfo)null!));
        Assert.Throws<ArgumentNullException>(() => new MegDataEntryOriginInfo((MegDataEntryLocationReference)null!));
        Assert.Throws<ArgumentNullException>(() => new MegDataEntryOriginInfo((byte[])null!));
    }

    [Fact]
    public void Ctor_FileInfo()
    {
        var fi = FileSystem.FileInfo.New("test.xml");
        var originInfo = new MegDataEntryOriginInfo(fi);

        Assert.Same(fi, originInfo.FileInfo);
        Assert.Null(originInfo.MegFileLocation);
        Assert.Null(originInfo.Bytes);

        Assert.True(originInfo.IsLocalFile);
        Assert.False(originInfo.IsEntryReference);
        Assert.False(originInfo.IsBytes);
    }

    [Fact]
    public void Ctor_ReferenceLocation()
    {
        using var _ = FileSystem.File.Create("test.meg");
        var meg = new MegFile(new MegArchive([]), new MegFileInformation("test.meg", MegVersion.V1),
            ServiceProvider);

        var location = new MegDataEntryLocationReference(meg, MegDataEntryTest.CreateEntry("path"));

        var originInfo = new MegDataEntryOriginInfo(location);

        Assert.Equal(location, originInfo.MegFileLocation);
        Assert.Null(originInfo.FileInfo);
        Assert.Null(originInfo.Bytes);

        Assert.True(originInfo.IsEntryReference);
        Assert.False(originInfo.IsLocalFile);
        Assert.False(originInfo.IsBytes);
    }

    #region CTOR_Byte[]

    [Fact]
    public void Ctor_Bytes()
    {
        var bytes = new byte[] { 1, 2, 3, 4 };
        var originInfo = new MegDataEntryOriginInfo(bytes);

        Assert.NotSame(bytes, originInfo.Bytes);
        Assert.Equal(bytes, originInfo.Bytes);
        Assert.Null(originInfo.FileInfo);
        Assert.Null(originInfo.MegFileLocation);

        Assert.True(originInfo.IsBytes);
        Assert.False(originInfo.IsLocalFile);
        Assert.False(originInfo.IsEntryReference);
    }

    [Fact]
    public void Ctor_Bytes_Empty_OK()
    {
        var bytes = Array.Empty<byte>();
        var originInfo = new MegDataEntryOriginInfo(bytes);

        Assert.True(originInfo.IsBytes);
        Assert.Empty(originInfo.Bytes!);
    }

    [Fact]
    public void Ctor_Bytes_DefensiveCopy()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var originInfo = new MegDataEntryOriginInfo(bytes);

        bytes[0] = 99;

        Assert.Equal([1, 2, 3], originInfo.Bytes);
    }

    #endregion

    #region CTOR_ReadOnlySpan<byte>

    [Fact]
    public void Ctor_Bytes_Span()
    {
        Span<byte> bytes = [1, 2, 3, 4];
        var originInfo = new MegDataEntryOriginInfo(bytes);

        Assert.Equal(bytes.ToArray(), originInfo.Bytes);
        Assert.Null(originInfo.FileInfo);
        Assert.Null(originInfo.MegFileLocation);

        Assert.True(originInfo.IsBytes);
        Assert.False(originInfo.IsLocalFile);
        Assert.False(originInfo.IsEntryReference);
    }

    [Fact]
    public void Ctor_Bytes_Span_Empty_OK()
    {
        var originInfo = new MegDataEntryOriginInfo(ReadOnlySpan<byte>.Empty);

        Assert.True(originInfo.IsBytes);
        Assert.Empty(originInfo.Bytes!);
    }

    [Fact]
    public void Ctor_Bytes_Span_DefensiveCopy()
    {
        Span<byte> bytes = [1, 2, 3];
        var originInfo = new MegDataEntryOriginInfo(bytes);

        bytes[0] = 99;

        Assert.Equal([1, 2, 3], originInfo.Bytes);
    }

    #endregion

    #region GetDataStream

    [Fact]
    public void GetDataStream_File_NotFound_Throws()
    {
        var originInfo = new MegDataEntryOriginInfo(FileSystem.FileInfo.New("test.txt"));
        Assert.Throws<FileNotFoundException>(originInfo.GetDataStream);
    }

    [Fact]
    public void GetDataStream_File()
    {
        FileSystem.Initialize().WithFile("test.txt").Which(m => m.HasBytesContent([1, 2, 3]));

        var originInfo = new MegDataEntryOriginInfo(FileSystem.FileInfo.New("test.txt"));
        var stream = originInfo.GetDataStream();
        Assert.Equal(3, stream.Length);

        var resultStream = new MemoryStream(new byte[3]);
        stream.CopyTo(resultStream);
        Assert.Equal([1, 2, 3], resultStream.ToArray());
    }

    [Fact]
    public void GetDataStream_LocationReference()
    {
        FileSystem.Initialize().WithFile("a.meg").Which(m => m.HasBytesContent([1, 2, 3, 4, 5]));

        var entry = MegDataEntryTest.CreateEntry("file.txt", offset: 1, size: 2);
        var meg = new MegFile(new MegArchive([entry]), new MegFileInformation("a.meg", MegVersion.V1), ServiceProvider);
        var originInfo = new MegDataEntryOriginInfo(new MegDataEntryLocationReference(meg, entry));

        var stream = originInfo.GetDataStream();
        Assert.Equal(2, stream.Length);

        var resultStream = new MemoryStream(new byte[2]);
        stream.CopyTo(resultStream);
        Assert.Equal([2, 3], resultStream.ToArray());
    }

    [Fact]
    public void GetDataStream_Bytes()
    {
        var bytes = new byte[] { 10, 20, 30, 40, 50 };
        var originInfo = new MegDataEntryOriginInfo(bytes);

        using var resultStream = originInfo.GetDataStream();
        Assert.Equal(5, resultStream.Length);

        var sink = new MemoryStream();
        resultStream.CopyTo(sink);
        Assert.Equal(bytes, sink.ToArray());
    }

    #endregion

    [Fact]
    public void EqualsHashCode()
    {
        using var _ = FileSystem.File.Create("test.meg");
        var meg = new MegFile(new MegArchive([]), new MegFileInformation("test.meg", MegVersion.V1),
            ServiceProvider);

        var location = new MegDataEntryLocationReference(meg, MegDataEntryTest.CreateEntry("path"));
        var otherLocation = new MegDataEntryLocationReference(meg, MegDataEntryTest.CreateEntry("path"));

        var originLoc = new MegDataEntryOriginInfo(location);
        var otherOriginLoc = new MegDataEntryOriginInfo(otherLocation);
        var originPath = new MegDataEntryOriginInfo(FileSystem.FileInfo.New("test.xml"));
        var otherOriginPath = new MegDataEntryOriginInfo(FileSystem.FileInfo.New("test.xml"));


        Assert.Equal(originLoc, originLoc);
        Assert.Equal(originLoc, (object)originLoc);
        Assert.Equal(originLoc, otherOriginLoc);

        Assert.Equal(originPath, originPath);
        Assert.Equal(originPath, (object)originPath);
        Assert.Equal(originPath, otherOriginPath);

        Assert.False(originLoc.Equals(null));
        Assert.NotEqual((object?)null, originLoc);

        Assert.False(originPath.Equals(null));
        Assert.NotEqual((object?)null, originPath);

        Assert.NotEqual(originPath, originLoc);
        Assert.NotEqual(originPath, (object)originLoc);

        Assert.NotEqual(originPath, new MegDataEntryOriginInfo(FileSystem.FileInfo.New("TEST.XML")));

        Assert.NotEqual(originLoc,
            new MegDataEntryOriginInfo(new MegDataEntryLocationReference(meg, MegDataEntryTest.CreateEntry("PATH"))));

        Assert.NotEqual(originLoc.GetHashCode(), originPath.GetHashCode());

        Assert.Equal(originLoc.GetHashCode(), otherOriginLoc.GetHashCode());
        Assert.Equal(originPath.GetHashCode(), otherOriginPath.GetHashCode());
    }

    [Fact]
    public void EqualsHashCode_Bytes()
    {
        var bytes = new byte[] { 1, 2, 3 };
        var origin = new MegDataEntryOriginInfo(bytes);

        // Self-equality.
        Assert.Equal(origin, origin);
        Assert.Equal(origin, (object)origin);
        Assert.False(origin.Equals(null));

        // Each ctor call clones the buffer, so two origins from the same input are NOT equal
        // (they own distinct internal buffers).
        var otherOrigin = new MegDataEntryOriginInfo(bytes);
        Assert.NotEqual(origin, otherOrigin);

        Assert.NotEqual(origin, new MegDataEntryOriginInfo(FileSystem.FileInfo.New("test.xml")));

        var spanOrigin = new MegDataEntryOriginInfo((ReadOnlySpan<byte>)bytes);
        Assert.Equal(spanOrigin, spanOrigin);
        Assert.Equal(spanOrigin, (object)spanOrigin);

        Assert.NotEqual(origin, otherOrigin);
    }
}
