// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

/// <summary>
/// Intermediate MEG archive used for constructing new .MEG files.
/// </summary>
internal interface IConstructingMegArchive : IMegDataEntryHolder<VirtualMegDataEntryReference>
{
    IMegArchive Archive { get; }

    MegFileVersion MegVersion { get; }

    bool Encrypted { get; }
}