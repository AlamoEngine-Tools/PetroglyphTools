using System.Collections.Generic;
using System.Linq;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

/// <inheritdoc cref="IMegConstructionArchive"/>
internal sealed class ConstructingMegArchive : MegDataEntryHolderBase<MegFileDataEntryReference>, IMegConstructionArchive
{
    public IMegArchive Archive { get; }

    public MegFileVersion MegVersion { get; }

    internal ConstructingMegArchive(IList<MegFileDataEntryReference> files, MegFileVersion megVersion) : base(files)
    {
        var dataEntries = Entries.Select(f => f.DataEntry).ToList();
        Archive = new MegArchive(dataEntries);
        MegVersion = megVersion;
    }
}