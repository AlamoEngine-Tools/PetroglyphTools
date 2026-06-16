// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Utilities;

namespace PG.StarWarsGame.Files.MEG.Data.Archives;

/// <summary>
/// An <see cref="IMegDataSource"/> that holds a whole MEG in memory and serves entry data from a byte buffer.
/// </summary>
internal sealed class InMemoryMeg : IMegDataSource
{
    private readonly byte[] _megData;

    /// <inheritdoc />
    public IMegArchive Archive { get; }

    internal string Name { get; } = $"<in-memory MEG {Guid.NewGuid()}>";

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryMeg"/> class with the specified archive and data.
    /// </summary>
    /// <param name="archive">The table of contents of the MEG.</param>
    /// <param name="megData">The raw bytes of the whole MEG archive.</param>
    internal InMemoryMeg(IMegArchive archive, byte[] megData)
    {
        Archive = archive ?? throw new ArgumentNullException(nameof(archive));
        _megData = megData ?? throw new ArgumentNullException(nameof(megData));
    }

    /// <inheritdoc />
    public MegEntryStream GetData(MegDataEntry entry)
    {
        if (entry is null)
            throw new ArgumentNullException(nameof(entry));
        if (!Archive.Contains(entry))
            throw new EntryNotInMegException(this, entry);
        if (entry.Encrypted)
            throw new NotImplementedException("Encrypted archives are currently not supported");

        if (entry.Location.Size == 0)
            return MegEntryStream.CreateEmptyStream(entry.Path);

        var stream = new MemoryStream(_megData, writable: false);
        return new MegEntryStream(entry.Path, stream, entry.Location.Offset, entry.Location.Size);
    }
}
