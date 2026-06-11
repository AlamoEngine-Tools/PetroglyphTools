// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Utilities;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

/// <summary>
/// An <see cref="IMegArchive"/> that is fully held in memory, including the data of all its entries.
/// </summary>
/// <remarks>
/// In contrast to a MEG file on disk, an in-memory archive is self-contained: it is not backed by a file
/// and does not retain the stream it was loaded from. Entry data is served directly from memory.
/// </remarks>
public interface IInMemoryMegArchive : IMegArchive
{
    /// <summary>
    /// Gets a read-only stream over the in-memory data of the specified <paramref name="entry"/>.
    /// </summary>
    /// <remarks>
    /// Each call returns an independent stream; the returned stream may be read concurrently with streams
    /// returned by other calls.
    /// </remarks>
    /// <param name="entry">The entry to read the data of.</param>
    /// <returns>A read-only stream containing the entry's data.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="entry"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="entry"/> is not contained in this archive.</exception>
    MegEntryStream GetData(MegDataEntry entry);
}
