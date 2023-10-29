// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services;

internal interface IMegConstructionArchiveService
{
    IMegConstructionArchive Build(IEnumerable<MegFileDataEntryBuilderInfo> builderEntries, MegFileVersion fileVersion);
}