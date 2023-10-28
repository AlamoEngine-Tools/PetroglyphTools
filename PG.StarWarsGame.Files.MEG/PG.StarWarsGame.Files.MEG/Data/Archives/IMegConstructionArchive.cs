// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

internal interface IMegConstructionArchive : IMegDataEntryHolder<ConstructingMegDataEntry>
{
    /// <summary>
    /// Gets the binary-convertible archive of this instance.
    /// </summary>
    IMegArchive Archive { get; }
}