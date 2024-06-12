// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;

namespace PG.Commons.DataTypes;

/// <summary>
/// Compares two <see cref="IHasCrc32"/> by their CRC32 checksum.
/// </summary>
public sealed class CrcBasedEqualityComparer<T> : IEqualityComparer<T> where T : IHasCrc32
{
    /// <summary>
    /// Gets a shared instance of the <see cref="CrcBasedEqualityComparer{T}"/> class.
    /// </summary>
    public static readonly CrcBasedEqualityComparer<T> Instance = new();


    /// <inheritdoc />
    public bool Equals(T x, T y)
    {
        return x.Crc32.Equals(y.Crc32);
    }

    /// <inheritdoc />
    public int GetHashCode(T obj)
    {
        return obj.Crc32.GetHashCode();
    }
}