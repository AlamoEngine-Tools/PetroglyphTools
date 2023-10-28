// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Services;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

/// <summary>
/// In-memory MEG archive whose contents may be distributed across multiple physical .MEG files.
/// <br/>
/// The PG games usually build such a model internally themselves in order to have a single Master MEG archive.
/// </summary>
/// <remarks>
/// Use the <see cref="IVirtualMegArchiveBuilder"/> service for creating this archive.
/// </remarks>
public interface IVirtualMegArchive : IMegDataEntryHolder<MegFileDataEntry>
{
}