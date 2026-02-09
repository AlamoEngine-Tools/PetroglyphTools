// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;

namespace PG.Commons.Data;

/// <summary>
/// Provides a CRC32-based equality comparison for objects that implement <see cref="IHasCrc32"/>.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="CrcBasedEqualityComparer{T}"/> class provides an implementation of the 
/// <see cref="IEqualityComparer{T}"/> interface that compares objects based solely on their 
/// CRC32 checksums. Two objects are considered equal if their <see cref="IHasCrc32.Crc32"/> 
/// values are equal.
/// </para>
/// <para>
/// This comparer is particularly useful when working with collections that require fast lookups 
/// based on CRC32 values, such as dictionaries or hash sets.
/// </para>
/// <para>
/// Use the <see cref="Instance"/> property to retrieve a default instance of this comparer 
/// rather than creating new instances.
/// </para>
/// </remarks>
/// <typeparam name="T">The type of objects to compare. This type must implement <see cref="IHasCrc32"/>.</typeparam>
public sealed class CrcBasedEqualityComparer<T> : IEqualityComparer<T> where T : IHasCrc32
{
    /// <summary>
    /// Returns a default equality comparer for the type specified by the generic argument.
    /// </summary>
    public static readonly CrcBasedEqualityComparer<T> Instance = new();

    /// <summary>
    /// Determines whether the specified objects are equal based on their CRC32 checksums.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method returns <see langword="true"/> under the following conditions:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Both <paramref name="x"/> and <paramref name="y"/> are <see langword="null"/>.</description></item>
    /// <item><description><paramref name="x"/> and <paramref name="y"/> refer to the same object.</description></item>
    /// <item><description>The <see cref="IHasCrc32.Crc32"/> values of <paramref name="x"/> and <paramref name="y"/> are equal.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="x">The first object of type <typeparamref name="T"/> to compare.</param>
    /// <param name="y">The second object of type <typeparamref name="T"/> to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the specified objects are both <see langword="null"/>, 
    /// are the same reference, or have equal CRC32 checksums; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equals(T? x, T? y)
    {
        if (ReferenceEquals(x, y))
            return true;
        if (x is null || y is null)
            return false;
        return x.Crc32.Equals(y.Crc32);
    }

    /// <summary>
    /// Returns a hash code for the specified object based on its CRC32 checksum.
    /// </summary>
    /// <remarks>
    /// The hash code is computed using the hash code of the object's <see cref="IHasCrc32.Crc32"/> property.
    /// This ensures that objects with equal CRC32 values produce the same hash code.
    /// </remarks>
    /// <param name="obj">The object for which to get a hash code.</param>
    /// <returns>
    /// A hash code for the specified object, derived from its <see cref="IHasCrc32.Crc32"/> value.
    /// </returns>
    public int GetHashCode(T? obj)
    {
        return obj?.Crc32.GetHashCode() ?? 0;
    }
}