// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Linq;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

/// <inheritdoc cref="IConstructingMegArchive"/>
internal sealed class ConstructingMegArchive : MegDataEntryHolderBase<VirtualMegDataEntryReference>, IConstructingMegArchive
{
    /// <inheritdoc/>
    public IMegArchive Archive { get; }

    /// <inheritdoc/>
    public MegFileVersion MegVersion { get; }

    public bool Encrypted { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConstructingMegArchive"/> class.
    /// </summary>
    /// <param name="virtualEntries">The data entries of this archive.</param>
    /// <param name="megVersion">The MEG version of this archive.</param>
    /// <param name="encrypted">Encryption information of this archive.</param>
    internal ConstructingMegArchive(IList<VirtualMegDataEntryReference> virtualEntries, MegFileVersion megVersion, bool encrypted) : base(virtualEntries)
    {
        var dataEntries = Entries.Select(f => f.DataEntry).ToList();
        Archive = new MegArchive(dataEntries);
        MegVersion = megVersion;
        Encrypted = encrypted;
    }
}