// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <summary>
/// Service to build virtual MEG archives.
/// </summary>
public interface IVirtualMegArchiveBuilder
{
    /// <summary>
    /// Builds a virtual MEG archive from a collection of MEG data references. The archive does not contain duplicates.
    /// </summary>
    /// <remarks>
    /// The resulting archive is correctly sorted as specified.
    /// </remarks>
    /// <param name="fileEntries">The collection of data references.</param>
    /// <param name="replaceExisting">When <see langowrd="true"/>, entries with the same CRC32 checksum get replaced.</param>
    /// <returns>The virtual MEG archive.</returns>
    /// <exception cref="EntryNotInMegException">When a <see cref="MegDataEntryReference"/> does not point to a real location.</exception>
    IVirtualMegArchive BuildFrom(IEnumerable<MegDataEntryReference> fileEntries, bool replaceExisting);

    /// <summary>
    /// Converts an <see cref="IMegDataSource"/> into a virtual, MEG archive.
    /// The archive does not contain duplicates.
    /// </summary>
    /// <param name="meg">The MEG to convert.</param>
    /// <returns>The virtual MEG archive.</returns>
    IVirtualMegArchive BuildFrom(IMegDataSource meg);

    /// <summary>
    /// Merges a collection of <see cref="IMegDataSource"/> into a virtual MEG archive.
    /// The archive does not contain duplicates.
    /// </summary>
    /// <param name="megs">The MEGs to merge into a virtual MEG archive.</param>
    /// <param name="replaceExisting">When <see langowrd="true"/>, entries from different MEGs with the same CRC32 checksum get replaced.
    /// Duplicate entries within the same MEG are ignored, so that only the first entry is recognized.</param>
    /// <returns>The virtual MEG archive.</returns>
    IVirtualMegArchive BuildFrom(IEnumerable<IMegDataSource> megs, bool replaceExisting);
}
