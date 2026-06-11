// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Services;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

public class MegFileServiceLoadArchiveTest : CommonMegTestBase
{
    private readonly IMegFileService _megFileService;

    public MegFileServiceLoadArchiveTest()
    {
        _megFileService = ServiceProvider.GetRequiredService<IMegFileService>();
    }

    [Fact]
    public void LoadArchive_NullStream_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _megFileService.LoadArchive(null!));
    }

    [Fact]
    public void LoadArchive_ParsesEntries()
    {
        using var stream = new MemoryStream(MegTestConstants.ContentMegFileV1);

        var archive = _megFileService.LoadArchive(stream);

        Assert.Equal(2, archive.Count);
        Assert.Equal("DATA\\XML\\CAMPAIGNFILES.XML", archive[0].Path);
        Assert.Equal("DATA\\XML\\GAMEOBJECTFILES.XML", archive[1].Path);
    }

    [Fact]
    public void LoadArchive_GetData_ReturnsEntryContent()
    {
        using var stream = new MemoryStream(MegTestConstants.ContentMegFileV1);

        var archive = _megFileService.LoadArchive(stream);

        Assert.Equal(MegTestConstants.CampaignFilesContent, ReadAll(archive.GetData(archive[0])));
        Assert.Equal(MegTestConstants.GameObjectFilesContent, ReadAll(archive.GetData(archive[1])));
    }

    [Fact]
    public void LoadArchive_IsSelfContained_SourceStreamMayBeDisposed()
    {
        var archive = LoadFromDisposedStream();

        // The source stream is already disposed; the data is still served from the in-memory copy.
        Assert.Equal(MegTestConstants.CampaignFilesContent, ReadAll(archive.GetData(archive[0])));
    }

    private IInMemoryMegArchive LoadFromDisposedStream()
    {
        using var stream = new MemoryStream(MegTestConstants.ContentMegFileV1);
        return _megFileService.LoadArchive(stream);
    }

    [Fact]
    public void LoadArchive_NonSeekableStream_Works()
    {
        using var nonSeekable = new NonSeekableReadStream(MegTestConstants.ContentMegFileV1);

        var archive = _megFileService.LoadArchive(nonSeekable);

        Assert.Equal(2, archive.Count);
        Assert.Equal(MegTestConstants.GameObjectFilesContent, ReadAll(archive.GetData(archive[1])));
    }

    [Fact]
    public void GetData_NullOrForeignEntry_Throws()
    {
        using var stream = new MemoryStream(MegTestConstants.ContentMegFileV1);
        var archive = _megFileService.LoadArchive(stream);

        var foreign = MegDataEntryTest.CreateEntry("not/in/archive.xml");

        Assert.Throws<ArgumentNullException>(() => archive.GetData(null!));
        Assert.Throws<ArgumentException>(() => archive.GetData(foreign));
    }

    [Fact]
    public void GetData_ReturnsIndependentStreams()
    {
        using var stream = new MemoryStream(MegTestConstants.ContentMegFileV1);
        var archive = _megFileService.LoadArchive(stream);

        using var s1 = archive.GetData(archive[0]);
        using var s2 = archive.GetData(archive[0]);

        s1.ReadByte();

        // Reading from one stream must not move the position of the other.
        Assert.Equal(1, s1.Position);
        Assert.Equal(0, s2.Position);
        Assert.Equal(MegTestConstants.CampaignFilesContent.Length, s1.Length);
    }

    private static byte[] ReadAll(Stream stream)
    {
        using (stream)
        {
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }

    private sealed class NonSeekableReadStream(byte[] data) : Stream
    {
        private readonly MemoryStream _inner = new(data, writable: false);

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _inner.Dispose();
            base.Dispose(disposing);
        }
    }
}
