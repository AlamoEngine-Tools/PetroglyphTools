// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics;
using PG.Commons.DataTypes;
using PG.Commons.Hashing;
using PG.Commons.Utilities;

namespace PG.StarWarsGame.Files.DAT.Data;

/// <summary>
/// A simple representation of a key-value pair that can be stored in a DAT file.
/// </summary>
/// <remarks>
/// Equality is based on <see cref="Key"/>, <see cref="Crc32"/> and <see cref="Value"/>.
/// </remarks>
[DebuggerDisplay("{Key}:{Value}")]
public readonly struct DatStringEntry : IHasCrc32, IEquatable<DatStringEntry>
{
    /// <summary>
    /// Gets the entry's key. Does not have to be unique. 
    /// </summary>
    public string Key { get; }
    
    /// <summary>
    /// Gets the entry's original key which may include extended ASCII characters.
    /// </summary>
    public string OriginalKey { get; }

    /// <summary>
    /// Gets the entry's value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets the CRC32 checksum of the key.
    /// </summary>
    public Crc32 Crc32 { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatStringEntry"/> structure with a specified key, checksum and value.
    /// </summary>
    /// <param name="key"> The entry's key.</param>
    /// <param name="keyChecksum">The CRC32 checksum of the key.</param>
    /// <param name="value">The entry's value.</param>
    /// <exception cref="ArgumentException"><paramref name="key"/> contains non-ASCII characters.</exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="key"/> or <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    public DatStringEntry(string key, Crc32 keyChecksum, string value) : this(key, keyChecksum, value, key)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatStringEntry"/> structure with a specified key, checksum,
    /// value and the original extended ASCII key.
    /// </summary>
    /// <param name="key"> The entry's key.</param>
    /// <param name="keyChecksum">The CRC32 checksum of the key.</param>
    /// <param name="value">The entry's value.</param>
    /// <param name="originalKey">The entry's original key.</param>
    /// <exception cref="ArgumentException"><paramref name="key"/> contains non-ASCII characters.</exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="key"/> or <paramref name="value"/> or <paramref name="originalKey"/> is <see langword="null"/>.
    /// </exception>
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

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Key}:{Value}";
    }
}