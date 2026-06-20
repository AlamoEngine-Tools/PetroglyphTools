// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

/// <summary>
/// Represents the list of data entries contained in a MEG archive.
/// The entries are sorted by their CRC32 checksum, which is calculated over the file name of the entry.
/// </summary>
public interface IMegArchive : IMegDataEntryHolder<MegDataEntry>;