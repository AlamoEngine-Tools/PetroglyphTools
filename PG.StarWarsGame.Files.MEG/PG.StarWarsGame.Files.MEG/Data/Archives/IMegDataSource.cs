// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Utilities;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

/// <summary>
/// Represents a MEG archive along with the necessary means to access its data.
/// </summary>
public interface IMegDataSource
{
    /// <summary>
    /// Gets the archive metadata: the table of contents describing the data entries contained in this MEG.
    /// </summary>
    IMegArchive Archive { get; }

    /// <summary>
    /// Gets a read-only stream over the data of the specified <paramref name="entry"/>.
    /// </summary>
    /// <param name="entry">The data entry to read.</param>
    /// <returns>A read-only stream containing the entry's data.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="entry"/> is <see langword="null"/>.</exception>
    /// <exception cref="EntryNotInMegException"><paramref name="entry"/> is not contained in this MEG.</exception>
    MegEntryStream GetData(MegDataEntry entry);
}
