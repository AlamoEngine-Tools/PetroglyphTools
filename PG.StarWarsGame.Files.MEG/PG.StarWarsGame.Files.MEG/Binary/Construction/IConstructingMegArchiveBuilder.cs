// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal interface IConstructingMegArchiveBuilder
{
    IConstructingMegArchive BuildConstructingMegArchive(IEnumerable<MegFileDataEntryBuilderInfo> builderEntries);
}