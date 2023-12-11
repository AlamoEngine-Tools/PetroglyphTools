// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Hashing;
using PG.Commons.Services;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.DAT.Binary;

namespace PG.StarWarsGame.Files.DAT.Data;

/// <summary>
///     A simple representation of a key-value pair that can be stored in a DAT file.
/// </summary>
public sealed class DatFileEntry : IEquatable<DatFileEntry>, IComparable<DatFileEntry>
{
    /// The entry's key. Does not have to be unique, but may never be null empty or whitespace.
    public string Key { get; }

    /// The entry's value. May be null.
    public string? Value { get; }

    /// <summary>
    ///     .ctor
    /// </summary>
    /// <param name="key">
    ///     The entry's <see cref="Key" />
    /// </param>
    /// <param name="value">
    ///     The entry's <see cref="Value" />
    /// </param>
    /// <exception cref="ArgumentNullException">If the key is null, empty or whitespace.</exception>
    public DatFileEntry(string key, string? value)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(key);

        Key = key;
        Value = value;
    }

    /// <inheritdoc />
    public bool Equals(DatFileEntry? other)
    {
        if (other is null)
        {
            return false;
        }

        return ChecksumService.Instance.GetChecksum(Key, DatFileConstants.TextKeyEncoding)
            .Equals(ChecksumService.Instance.GetChecksum(other.Key, DatFileConstants.TextKeyEncoding));
    }

    /// <inheritdoc />
    public int CompareTo(DatFileEntry? other)
    {
        if (other is null)
        {
            return 1;
        }

        return ChecksumService.Instance.GetChecksum(Key, DatFileConstants.TextKeyEncoding)
            .CompareTo(ChecksumService.Instance.GetChecksum(other.Key, DatFileConstants.TextKeyEncoding));
    }
}