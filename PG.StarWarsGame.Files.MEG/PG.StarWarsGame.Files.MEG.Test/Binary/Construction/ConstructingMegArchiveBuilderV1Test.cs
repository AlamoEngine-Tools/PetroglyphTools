using System;
using System.Collections.Generic;
using System.Linq;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Binary.V1;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Construction;

public class ConstructingMegArchiveBuilderV1Test : ConstructingMegArchiveBuilderBaseTest
{
    private protected override ConstructingMegArchiveBuilderBase CreateBuilder()
    {
        return new ConstructingMegArchiveBuilderV1(ServiceProvider);
    }

    protected override int GetExpectedHeaderSize()
    {
        return MegHeader.SizeValue;
    }

    protected override MegFileVersion GetExpectedFileVersion()
    {
        return MegFileVersion.V1;
    }

    [Fact]
    public void Ctor_NullArg_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ConstructingMegArchiveBuilderV1(null!));
    }

    private protected override SmallMaxFileSizeConstructingService CreateSmallMaxFileSizeConstructingService(uint maxEntrySize, uint maxFileSize)
    {
        return new SmallMaxFileSizeConstructingServiceV1(maxEntrySize, maxFileSize, ServiceProvider);
    }

    protected override uint GetMaxEntrySizeForTooLargeTest(MegDataEntryBuilderInfo entry)
    {
        return entry.Size - 1;
    }

    private sealed class SmallMaxFileSizeConstructingServiceV1(uint maxEntrySize, uint maxFileSize, IServiceProvider services)
        : SmallMaxFileSizeConstructingService(maxEntrySize, maxFileSize, services)
    {
        protected override MegFileVersion FileVersion => MegFileVersion.V1;
    }

    protected override uint GetTotalMegSizeForTooLargeTest(IEnumerable<MegDataEntryBuilderInfo> entries)
    {
        var entriesList = entries.ToList();
        var fileNameTableSize =  entriesList.Sum(e => MegFileNameTableRecord.GetRecordSize(e.EntryPath));
        var fileTableSize = entriesList.Count * MegFileTableRecord.SizeValue;
        var dataSize = entriesList.Sum(e => e.Size);
        return (uint)(MegHeader.SizeValue + fileNameTableSize + fileTableSize + dataSize) - 1;
    }
}