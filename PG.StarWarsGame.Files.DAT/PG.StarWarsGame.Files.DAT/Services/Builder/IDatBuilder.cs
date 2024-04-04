// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.Commons.Services.Builder;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.DAT.Services.Builder;

/// <summary>
/// Service to create DAT files ensuring validation of keys.
/// </summary>
public interface IDatBuilder : IFileBuilder<IReadOnlyList<DatStringEntry>, DatFileInformation>
{
    /// <summary>
    /// Gets the key sort order of the DATs created by the <see cref="IDatBuilder"/>.
    /// </summary>
    DatFileType TargetKeySortOrder { get; }

    /// <summary>
    /// Gets a value indicating how the <see cref="IDatBuilder"/> treats an already existing key.
    /// </summary>
    BuilderOverrideKind KeyOverwriteBehavior { get; }

    /// <summary>
    /// Gets a list of all entries in the order of how they have been added to the <see cref="IDatBuilder"/>.
    /// </summary>
    IReadOnlyList<DatStringEntry> Entries { get; }

    /// <summary>
    /// Gets a list of all entries sorted by their CRC32 checksum.
    /// </summary>
    IReadOnlyList<DatStringEntry> SortedEntries { get; }

    /// <summary>
    /// Gets the key validator for this <see cref="IDatBuilder"/>.
    /// </summary>
    IDatKeyValidator KeyValidator { get; }

    /// <summary>
    /// Adds a key-value pair to the <see cref="IDatBuilder"/> and returns a status information to indicate whether the entry was successfully added. 
    /// </summary>
    /// <remarks>
    /// The actual added key might be different to <paramref name="key"/> due to re-encoding the key value.
    /// </remarks>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>The result of this operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> or <paramref name="value"/> is <see langword="null"/>.</exception>
    AddEntryResult AddEntry(string key, string value);

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="IDatBuilder"/>.
    /// </summary>
    /// <param name="entry">The entry to remove from the <see cref="IDatBuilder"/>.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="entry"/> was successfully removed from the <see cref="IDatBuilder"/>; otherwise, <see langword="false"/>.
    /// This method also returns <see langword="false"/> if <paramref name="entry"/> is not found in the original <see cref="IDatBuilder"/>.
    /// </returns>
    bool Remove(DatStringEntry entry);

    /// <summary>
    /// Removes all entries with the specified key.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns><see langword="true"/> if <paramref name="key"/> was successfully removed from the <see cref="IDatBuilder"/>; otherwise, <see langword="false"/>.
    /// This method also returns <see langword="false"/> if <paramref name="key"/> is not found in the original <see cref="IDatBuilder"/>.</returns>
    bool RemoveAllKeys(string key);

    /// <summary>
    /// Removes all entries from the <see cref="IDatBuilder"/>.
    /// </summary>
    void Clear();

    /// <summary>
    /// Checks whether the specified key is valid for this <see cref="IDatBuilder"/>.
    /// </summary>
    /// <param name="key">The key to validate</param>
    /// <returns><see langword="true"/> if the passed file information are valid; otherwise, <see langword="false"/>.</returns>
    bool IsKeyValid(string key);

    /// <summary>
    /// Create a DAT model from this instance.
    /// </summary>
    /// <returns>
    /// A DAT model containing all entries of the <see cref="IDatBuilder"/>.
    /// </returns>
    IDatModel BuildModel();
}