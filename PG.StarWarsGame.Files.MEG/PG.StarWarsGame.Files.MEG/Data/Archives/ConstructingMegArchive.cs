using System.Collections.Generic;
using System.Linq;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

/// <inheritdoc cref="IMegConstructionArchive"/>
internal sealed class ConstructingMegArchive : MegDataEntryHolderBase<ConstructingMegDataEntry>, IMegConstructionArchive
{
    public IMegArchive Archive { get; }

    internal ConstructingMegArchive(IList<ConstructingMegDataEntry> files) : base(files)
    {
        var dataEntries = Entries.Select(f => f.FileEntry).ToList();
        Archive = new MegArchive(dataEntries);
    }
}