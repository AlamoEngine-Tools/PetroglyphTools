// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;

namespace PG.Commons.Data;

/// <summary>
/// Represents a CRC32-based comparison method.
/// </summary>
/// <typeparam name="T">The type, constrained to <see cref="IHasCrc32"/>, of objects to compare.</typeparam>
public sealed class CrcBasedEqualityComparer<T> : IEqualityComparer<T> where T : IHasCrc32
{
    /// <summary>
    /// Returns a default equality comparer for the type specified by the generic argument.
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