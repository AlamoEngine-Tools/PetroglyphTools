// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
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
    public IVirtualMegArchive BuildFrom(IEnumerable<MegDataEntryReference> fileEntries, bool replaceExisting)
    {
        if (fileEntries == null) 
            throw new ArgumentNullException(nameof(fileEntries));

        return BuildFrom(fileEntries, replaceExisting, true);
    }

    /// <inheritdoc/>
    public IVirtualMegArchive BuildFrom(IMegFile megFile, bool replaceExisting)
    {
        if (megFile == null) 
            throw new ArgumentNullException(nameof(megFile));

        var entryReferences = megFile.Archive
            .Select(entry => new MegDataEntryReference(new MegDataEntryLocationReference(megFile, entry)));
        return BuildFrom(entryReferences, replaceExisting, false);
    }

    /// <inheritdoc/>
    public IVirtualMegArchive BuildFrom(IList<IMegFile> megFiles, bool replaceExisting)
    {
        if (megFiles == null) 
            throw new ArgumentNullException(nameof(megFiles));

        var entryReferences = from megFile in megFiles from entry in megFile.Archive select new MegDataEntryReference(new MegDataEntryLocationReference(megFile, entry));
        return BuildFrom(entryReferences, replaceExisting, false);
    }

    private static IVirtualMegArchive BuildFrom(IEnumerable<MegDataEntryReference> fileEntries, bool replaceExisting, bool checkExists)
    {
        IList<MegDataEntryReference> sortedEntries;
        if (replaceExisting)
        {
            var s = new SortedList<Crc32, MegDataEntryReference>();
            foreach (var entry in fileEntries)
                s[entry.Crc32] = entry;
            sortedEntries = s.Values;
        }
        else
        {
            sortedEntries = Crc32Utilities.SortByCrc32(fileEntries);
        }

        if (checkExists)
        {
            foreach (var entry in sortedEntries)
                if (!entry.Location.Exists)
                    throw new FileNotInMegException(entry.Location);
        }
        
        return new VirtualMegArchive(sortedEntries);
    }
}