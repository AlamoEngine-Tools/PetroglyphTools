// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using AnakinRaW.CommonUtilities.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Data;

/// <summary>
/// A list of key-value string entries which are used by Petroglyph games to store in-game text.
/// </summary>
public interface IDatModel : IReadOnlyList<DatStringEntry>
{
    /// <summary>
    /// Gets a copy of all keys present in the <see cref="IDatModel"/>.
    /// </summary>
    ISet<string> Keys { get; }

    /// <summary>
    /// Gets a copy of all CRC32 checksum present in the <see cref="IDatModel"/>.
    /// </summary>
    ISet<Crc32> CrcKeys { get; }

    /// <summary>
    /// Gets a value indicating how keys are organized in the <see cref="IDatModel"/>.
    /// </summary>
    /// <remarks>
    /// Game credit models <b>may</b> also be sorted by pure chance.
    /// So this property does not provide a safe way to determine the semantics of this model.
    /// </remarks>
    public DatFileType KeySortOder { get; }

    /// <summary>
    /// Determines whether the <see cref="IDatModel"/> contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the <see cref="IDatModel"/>.</param>
    /// <returns><see langword="true"/> if the <see cref="IDatModel"/> contains an entry with the specified key; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    bool ContainsKey(string key);

    /// <summary>
    /// Gets the first value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key. If the specified key is not found, a get operation throws a <see cref="KeyNotFoundException"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">The key does not exist in the list.</exception>
    string GetValue(string key);

    /// <summary>
    /// Gets the first value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">
    /// When this method returns, contains the value associated with the specified key,
    /// if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter.
    /// This parameter is passed uninitialized.
    /// </param>
    /// <returns><see langword="true"/> if the <see cref="IDatModel"/> contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    bool TryGetValue(string key, [NotNullWhen(true)] out string? value);

    /// <summary>
    /// Determines whether the <see cref="IDatModel"/> contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the <see cref="IDatModel"/>.</param>
    /// <returns><see langword="true"/> if the <see cref="IDatModel"/> contains an entry with the specified key; otherwise, <see langword="false"/>.</returns>
    bool ContainsKey(Crc32 key);

    /// <summary>
    /// Gets a list of entries with the matching key. 
    /// </summary>
    /// <param name="key">The key to match.</param>
    /// <returns>List of matching data entries.</returns>
    ReadOnlyFrugalList<DatStringEntry> EntriesWithCrc(Crc32 key);

    /// <summary>
    /// Gets a list of entries with the matching key. 
    /// </summary>
    /// <param name="key">The key to match.</param>
    /// <returns>List of matching data entries.</returns>
    ReadOnlyFrugalList<DatStringEntry> EntriesWithKey(string key);

    /// <summary>
    /// Gets the first value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key. If the specified key is not found, a get operation throws a <see cref="KeyNotFoundException"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">The key does not exist in the list.</exception>
    string GetValue(Crc32 key);

    /// <summary>
    /// Gets the first value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">
    /// When this method returns, contains the value associated with the specified key,
    /// if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter.
    /// This parameter is passed uninitialized.
    /// </param>
    /// <returns><see langword="true"/> if the <see cref="IDatModel"/> contains an element with the specified key; otherwise, <see langword="false"/>.</returns>
    bool TryGetValue(Crc32 key, [NotNullWhen(true)] out string? value);
}