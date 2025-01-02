// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.Data;
using PG.Commons.Hashing;
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
    public IVirtualMegArchive BuildFrom(IMegFile megFile)
    {
        if (megFile == null)
            throw new ArgumentNullException(nameof(megFile));

        var entryReferences = megFile.Archive
            .Distinct(CrcBasedEqualityComparer<MegDataEntry>.Instance)
            .Select(entry => new MegDataEntryReference(new MegDataEntryLocationReference(megFile, entry)));

        return new VirtualMegArchive(Crc32Utilities.SortByCrc32(entryReferences));
    }

    /// <inheritdoc/>
    public IVirtualMegArchive BuildFrom(IEnumerable<MegDataEntryReference> fileEntries, bool replaceExisting)
    {
        if (fileEntries == null) 
            throw new ArgumentNullException(nameof(fileEntries));

        return BuildFrom(fileEntries, replaceExisting, true);
    }

    /// <inheritdoc/>
    public IVirtualMegArchive BuildFrom(IList<IMegFile> megFiles, bool replaceExisting)
    {
        if (megFiles == null) 
            throw new ArgumentNullException(nameof(megFiles));

        var entries = megFiles.SelectMany(f => f.Archive.Distinct(CrcBasedEqualityComparer<MegDataEntry>.Instance)
            .Select(entry => new MegDataEntryReference(new MegDataEntryLocationReference(f, entry))));

        return BuildFrom(entries, replaceExisting, false);
    }

    private static IVirtualMegArchive BuildFrom(IEnumerable<MegDataEntryReference> fileEntries, bool replaceExisting, bool checkExists)
    {
        var sortedEntries = new SortedList<Crc32, MegDataEntryReference>();

        foreach (var entry in fileEntries)
        {
            if (checkExists && !entry.Location.Exists)
                throw new FileNotInMegException(entry.Location);

            if (replaceExisting)
                sortedEntries[entry.Crc32] = entry;
            else
            {
                if (sortedEntries.ContainsKey(entry.Crc32))
                    continue;
                sortedEntries.Add(entry.Crc32, entry);
            }
        }

        return new VirtualMegArchive(sortedEntries.Values);
    }
}