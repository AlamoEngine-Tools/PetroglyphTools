using System.Collections.Generic;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Test.Data.Archives;

public class MegArchiveTest : MegDataEntryHolderBaseTest<MegDataEntry, IMegArchive>
{
    protected override IMegArchive CreateArchive(IList<MegDataEntry> entries)
    {
        return new MegArchive(entries);
    }

    protected override MegDataEntry CreateEntry(string path, Crc32 crc = default)
    {
        return MegDataEntryTest.CreateEntry(path, crc);
    }
}