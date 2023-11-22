// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

/// <summary>
/// Contains all data entries of a single, physical .MEG file.
/// </summary>
public interface IMegArchive : IMegDataEntryHolder<MegDataEntry>;