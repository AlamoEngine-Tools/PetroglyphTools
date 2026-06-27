using System;
using System.IO;
using AnakinRaW.CommonUtilities.Testing;
using AnakinRaW.CommonUtilities.Testing.Extensions;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Test.Binary.Reader.V1;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

public abstract class MegDataSourceTestSuite : CommonMegTestBase
{
    protected abstract IMegDataSource CreateMegDataSource(byte[] megBytes);

    [Fact]
    public void GetData_ReturnsEntryContent()
    {
        var source = CreateMegDataSource(MegTestConstants.ContentMegFileV1);

        Assert.Equal(2, source.Archive.Count);
        Assert.Equal(MegTestConstants.CampaignFilesContent, ReadAll(source.GetData(source.Archive[0])));
        Assert.Equal(MegTestConstants.GameObjectFilesContent, ReadAll(source.GetData(source.Archive[1])));
    }

    [Fact]
    public void GetData_FromUnorderedMeg()
    {
        var bytes = TestingHelpers.GetEmbeddedResourceAsByteArray(typeof(MegFileBinaryReaderV1IntegrationTest), 
            "Files.v1_out_of_order.meg");
        var source = CreateMegDataSource(bytes);

        using var ms = new MemoryStream();
        using (var stream = source.GetData(source.Archive[0]))
            stream.CopyTo(ms);
        using (var stream = source.GetData(source.Archive[1]))
            stream.CopyTo(ms);

        Assert.Equal("456123"u8, ms.ToArray());
        Assert.True(source.Archive[0].Location.Offset > source.Archive[1].Location.Offset);
    }

    [Fact]
    public void GetData_NullEntry_Throws()
    {
        var source = CreateMegDataSource(MegTestConstants.ContentMegFileV1);
        Assert.Throws<ArgumentNullException>(() => source.GetData(null!));
    }

    [Fact]
    public void GetData_EntryNotInArchive_Throws()
    {
        var source = CreateMegDataSource(MegTestConstants.ContentMegFileV1);
        Assert.Throws<EntryNotInMegException>(() => source.GetData(MegDataEntryTest.CreateEntry("not/in/archive.xml")));
    }

    [Fact]
    public void GetData_EmptyEntry_ReturnsEmptyStream()
    {
        var source = CreateMegDataSource(MegTestConstants.EmptyEntryMegFileV1);

        var entry = Assert.Single(source.Archive);
        Assert.Equal(0u, entry.Location.Size);

        using var stream = source.GetData(entry);
        Assert.Equal(0, stream.Length);
    }

    [Fact]
    public void GetData_ReturnsIndependentStreams()
    {
        var source = CreateMegDataSource(MegTestConstants.ContentMegFileV1);

        using var s1 = source.GetData(source.Archive[0]);
        using var s2 = source.GetData(source.Archive[0]);

        s1.ReadByte();

        // Reading from one stream must not move the position of the other.
        Assert.Equal(1, s1.Position);
        Assert.Equal(0, s2.Position);
    }

    [Fact]
    public void Dispose_IsIdempotent()
    {
        var source = CreateMegDataSource(MegTestConstants.ContentMegFileV1);

        source.Dispose();
        Assert.DoesNotThrow(source.Dispose);
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
}
