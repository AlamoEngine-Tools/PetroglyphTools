// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <inheritdoc/>
internal sealed class VirtualMegArchiveBuilder : IVirtualMegArchiveBuilder
{
    /// <inheritdoc/>
    public IVirtualMegArchive BuildFrom(IEnumerable<MegDataEntryReference> fileEntries, bool replaceExisting)
    {
        var sortedEntries = Crc32Utilities.SortByCrc32(fileEntries);

        if (replaceExisting)
        {
            var mapped = Crc32Utilities.ListToCrcIndexRangeTable(sortedEntries);
            var deduplicated = new List<MegDataEntryReference>(mapped.Count);
            foreach (var indexRangeMap in mapped)
            {
                var lastIndex = indexRangeMap.Value.Start + indexRangeMap.Value.Length - 1;
                Debug.Assert(lastIndex >= indexRangeMap.Value.Start);
                deduplicated.Add(sortedEntries[lastIndex]);
            }
            sortedEntries = deduplicated;
        }
        return new VirtualMegArchive(sortedEntries);
    }

    /// <inheritdoc/>
    public IVirtualMegArchive BuildFrom(IMegFile megFile, bool replaceExisting)
    {
        var entryReferences = megFile.Archive
            .Select(entry => new MegDataEntryReference(new MegDataEntryLocationReference(megFile, entry)));
        return BuildFrom(entryReferences, replaceExisting);
    }

    /// <inheritdoc/>
    public IVirtualMegArchive BuildFrom(IList<IMegFile> megFiles, bool replaceExisting)
    {
        var entryReferences = (from megFile in megFiles from entry in megFile.Archive select new MegDataEntryReference(new MegDataEntryLocationReference(megFile, entry)));
        return BuildFrom(entryReferences, replaceExisting);
    }
}