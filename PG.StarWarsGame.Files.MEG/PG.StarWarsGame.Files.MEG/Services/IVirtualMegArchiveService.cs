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
    /// Builds a virtual MEG archive from a collection of MEG data references. 
    /// </summary>
    /// <remarks>
    /// The resulting archive is correctly sorted as specified.
    /// </remarks>
    /// <param name="fileEntries">The collection of data references.</param>
    /// <param name="replaceExisting"></param>
    /// <returns>The virtual MEG archive.</returns>
    IVirtualMegArchive BuildFrom(IEnumerable<MegDataEntryReference> fileEntries, bool replaceExisting);

    /// <summary>
    /// Converts an <see cref="IMegArchive"/> into a virtual, im-memory representation.
    /// </summary>
    /// <remarks>
    /// The resulting archive is sorted identical to the given <paramref name="archive"/>.
    /// </remarks>
    /// <param name="archive">The archive to convert</param>
    /// <param name="replaceExisting"></param>
    /// <returns>The virtual MEG archive.</returns>
    IVirtualMegArchive BuildFrom(IMegArchive archive, bool replaceExisting);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="archive"></param>
    /// <param name="replaceExisting"></param>
    /// <returns></returns>
    IVirtualMegArchive BuildFrom(IList<IMegArchive> archive, bool replaceExisting);
}