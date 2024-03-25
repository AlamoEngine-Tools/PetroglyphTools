// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.DataTypes;
using PG.Commons.Hashing;
using PG.Commons.Utilities;

namespace PG.StarWarsGame.Files.DAT.Data;

/// <summary>
///     A simple representation of a key-value pair that can be stored in a DAT file.
/// </summary>
public readonly struct DatStringEntry : IHasCrc32, IEquatable<DatStringEntry>
{
    /// The entry's key. Does not have to be unique, but may never be null empty or whitespace.
    public string Key { get; }
    
    /// <summary>
    /// 
    /// </summary>
    public string OriginalKey { get; }

    /// The entry's value. May be null.
    public string Value { get; }

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
    public DatStringEntry(string key, Crc32 keyChecksum, string value) : this(key, keyChecksum, value, key)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="keyChecksum"></param>
    /// <param name="value"></param>
    /// <param name="originalKey"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public DatStringEntry(string key, Crc32 keyChecksum, string value, string originalKey)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));
        if (value == null) 
            throw new ArgumentNullException(nameof(value));

        StringUtilities.ValidateIsAsciiOnly(key.AsSpan());

        Key = key;
        OriginalKey = originalKey ?? throw new ArgumentNullException(nameof(originalKey));
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Crc32 = keyChecksum;
    }

    /// <inheritdoc/>
    public bool Equals(DatStringEntry other)
    {
        return Key == other.Key && Value == other.Value && Crc32.Equals(other.Crc32);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is DatStringEntry other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Key, Value, Crc32);
    }
}