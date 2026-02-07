// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Linq;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

internal sealed class ConstructingMegArchive : MegDataEntryHolderBase<VirtualMegDataEntryReference>, IConstructingMegArchive
{
    public IMegArchive Archive { get; }

    public MegFileVersion MegVersion { get; }

    public bool Encrypted { get; }
    
    public uint ExpectedFileSize { get; }

    internal ConstructingMegArchive(
        IList<VirtualMegDataEntryReference> virtualEntries, 
        MegFileVersion megVersion,
        uint expectedFileSize,
        bool encrypted) 
        : base(virtualEntries)
    {
        var dataEntries = Entries.Select(f => f.DataEntry).ToList();
        Archive = new MegArchive(dataEntries);
        MegVersion = megVersion;
        ExpectedFileSize = expectedFileSize;
        Encrypted = encrypted;
    }
}