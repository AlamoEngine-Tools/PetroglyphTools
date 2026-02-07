using System.Collections.Generic;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Test.Data.Archives;

public class VirtualMegArchiveTest : MegDataEntryHolderBaseTest<MegDataEntryReference, IVirtualMegArchive>
{
    protected override IVirtualMegArchive CreateArchive(IList<MegDataEntryReference> entries)
    {
        return new VirtualMegArchive(entries);
    }

    protected override MegDataEntryReference CreateEntry(string path, Crc32 crc = default)
    {
        FileSystem.File.Create("file.meg").Dispose();
        var entry = MegDataEntryTest.CreateEntry(path, crc);
        var megFile = new MegFile(new MegArchive([entry]), new MegFileInformation("file.meg", MegFileVersion.V1), ServiceProvider);
        return new MegDataEntryReference(new MegDataEntryLocationReference(megFile, entry));
    }
}