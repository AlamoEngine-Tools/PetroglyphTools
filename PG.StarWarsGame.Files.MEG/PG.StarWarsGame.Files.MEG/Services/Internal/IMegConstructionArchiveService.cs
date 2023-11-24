// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services;

internal interface IMegConstructionArchiveService
{
    IMegConstructionArchive Build(IEnumerable<MegFileDataEntryBuilderInfo> builderEntries, MegFileVersion fileVersion);
}

internal class MegConstructionArchiveService : IMegConstructionArchiveService
{
    public IMegConstructionArchive Build(IEnumerable<MegFileDataEntryBuilderInfo> builderEntries, MegFileVersion fileVersion)
    {
        var sortedBuildEntries = builderEntries;

        
        var list = new List<VirtualMegDataEntryReference>();

        foreach (var builderInfoEntry in sortedBuildEntries)
        {
            var location = GetLocation(builderInfoEntry.OriginInfo);
            var entry = new MegDataEntry(builderInfoEntry.FilePath, builderInfoEntry.Crc32, location, builderInfoEntry.Encrypted);

            list.Add(new VirtualMegDataEntryReference(entry, builderInfoEntry.OriginInfo));
        }

        return new ConstructingMegArchive(list, fileVersion);
    }

    private MegDataEntryLocation GetLocation(MegDataEntryOriginInfo originInfo)
    {
        throw new NotImplementedException();
    }
}