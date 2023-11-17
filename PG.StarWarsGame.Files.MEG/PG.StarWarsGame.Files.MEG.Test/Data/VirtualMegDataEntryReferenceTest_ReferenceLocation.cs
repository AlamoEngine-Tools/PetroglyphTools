using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Test.Data;

[TestClass]
public class VirtualMegDataEntryReferenceTest_ReferenceLocation : MegDataEntryBaseTest<MegDataEntryOriginInfo>
{
    // We need a instance global variable, cause IMegFile equality is only done by reference.
    private readonly IMegFile _megFile = new Mock<IMegFile>().Object;

    protected override MegDataEntryBase<MegDataEntryOriginInfo> CreateEntry(string path, Crc32 crc, MegDataEntryOriginInfo location)
    {
        return new VirtualMegDataEntryReference(
            new MegDataEntry(path, crc, new MegDataEntryLocation(), false), location);
    }

    protected override MegDataEntryOriginInfo CreateLocation(int seed)
    {
        return new MegDataEntryOriginInfo(new MegDataEntryReferenceLocation(_megFile,
            new MegDataEntry(seed.ToString(), DefaultCrc, new MegDataEntryLocation((uint)seed, (uint)seed), false)));
    }
}