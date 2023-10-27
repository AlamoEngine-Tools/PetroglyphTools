// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services;

/// <summary>
/// Service to build virtual MEG archives.
/// </summary>
public interface IVirtualMegArchiveService
{
    /// <summary>
    /// Builds a virtual MEG archive from a collection of MEG data origin information. 
    /// </summary>
    /// <remarks>
    /// The resulting <see cref="IVirtualMegArchive.Files"/> is correctly sorted as specified.
    /// </remarks>
    /// <param name="dataOrigins">The collection of data origins.</param>
    /// <returns>The virtual MEG archive.</returns>
    IVirtualMegArchive Build(IEnumerable<MegDataEntryOriginInfo> dataOrigins);

    /// <summary>
    /// Converts an <see cref="IMegArchive"/> into a virtual, im-memory representation.
    /// </summary>
    /// <remarks>
    /// The resulting <see cref="IVirtualMegArchive.Files"/> is sorted identical to the given <paramref name="archive"/>.
    /// </remarks>
    /// <param name="archive">The archive to convert</param>
    /// <returns>The virtual MEG archive.</returns>
    IVirtualMegArchive ConvertFrom(IMegArchive archive);
}

internal interface IMegConstructionArchiveService
{
    IMegConstructionArchive Build(IEnumerable<MegFileDataEntryBuilderInfo> builderEntries);
}