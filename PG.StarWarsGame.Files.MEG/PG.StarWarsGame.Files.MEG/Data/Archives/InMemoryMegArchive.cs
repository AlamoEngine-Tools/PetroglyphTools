// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Utilities;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

/// <inheritdoc cref="IInMemoryMegArchive"/>
internal sealed class InMemoryMegArchive : MegDataEntryHolderBase<MegDataEntry>, IInMemoryMegArchive
{
    private readonly byte[] _megData;

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryMegArchive"/> class.
    /// </summary>
    /// <param name="entries">The CRC32-sorted entries of the archive.</param>
    /// <param name="megData">The raw bytes of the whole MEG archive.</param>
    internal InMemoryMegArchive(IList<MegDataEntry> entries, byte[] megData) : base(entries)
    {
        _megData = megData ?? throw new ArgumentNullException(nameof(megData));
    }

    /// <inheritdoc />
    public MegEntryStream GetData(MegDataEntry entry)
    {
        if (entry is null)
            throw new ArgumentNullException(nameof(entry));
        if (!Contains(entry))
            throw new ArgumentException($"The entry '{entry.Path}' is not contained in this MEG archive.", nameof(entry));

        if (entry.Location.Size == 0)
            return MegEntryStream.CreateEmptyStream(entry.Path);

        // Each call gets its own MemoryStream view over the shared buffer (no copy), so the returned
        // streams have independent positions and can be read concurrently without synchronization.
        var stream = new MemoryStream(_megData, writable: false);
        return new MegEntryStream(entry.Path, stream, entry.Location.Offset, entry.Location.Size);
    }
}
