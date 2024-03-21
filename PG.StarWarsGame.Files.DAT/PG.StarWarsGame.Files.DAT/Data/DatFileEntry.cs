// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.DataTypes;
using PG.Commons.Hashing;
using AnakinRaW.CommonUtilities;

namespace PG.StarWarsGame.Files.DAT.Data;

/// <summary>
///     A simple representation of a key-value pair that can be stored in a DAT file.
/// </summary>
public sealed class DatFileEntry : IHasCrc32, IEquatable<DatFileEntry>, IComparable<DatFileEntry>
{
    /// The entry's key. Does not have to be unique, but may never be null empty or whitespace.
    public string Key { get; }

    /// The entry's value. May be null.
    public string? Value { get; }

    /// <summary>
    /// Get the CRC32 checksum of the entry's key.
    /// </summary>
    public Crc32 Crc32 { get; }

    /// <summary>
    ///     .ctor
    /// </summary>
    /// <param name="key">
    ///     The entry's <see cref="Key" />
    /// </param>
    /// <param name="keyChecksum">The CRC32 checksum of the key.</param>
    /// <param name="value">
    ///     The entry's <see cref="Value" />
    /// </param>
    /// <exception cref="ArgumentNullException">If the key is null, empty or whitespace.</exception>
    public DatFileEntry(string key, Crc32 keyChecksum, string? value)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(key);

        Key = key;
        Value = value;
        Crc32 = keyChecksum;
    }   

    /// <inheritdoc />
    public bool Equals(DatFileEntry? other)
    {
        return other is not null && Crc32.Equals(other.Crc32);
    }

    /// <inheritdoc />
    public int CompareTo(DatFileEntry? other)
    {
        return other is null ? 1 : Crc32.CompareTo(other.Crc32);
    }
}