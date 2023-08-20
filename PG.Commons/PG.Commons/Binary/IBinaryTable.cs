// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;

namespace PG.Commons.Binary;

/// <summary>
/// A readonly list-like table used by Alamo binary models to hold metadata information
/// </summary>
/// <typeparam name="T">The content type of this table.</typeparam>
public interface IBinaryTable<out T> : IBinary, IEnumerable<T>
{
    /// <summary>
    /// Gets the element at the specified index.
    /// </summary>
    /// <remarks>
    /// The Alamo specifications usually allows tables with <see cref="uint"/> possible entries, however in .NET we are
    /// limited to <see cref="int"/> for indexing native list-like structures.  
    /// </remarks>
    /// <param name="i">The zero-based index of the element to get or set.</param>
    /// <returns>The element at the specified index.</returns>
    T this[int i] { get; }

    /// <summary>
    /// Gets the number of elements contained in this <see cref="IBinaryTable{T}"/>.
    /// </summary>
    /// <remarks>
    /// The Alamo specifications usually allows tables with <see cref="uint"/> possible entries, however in .NET we are
    /// limited to <see cref="int"/> for indexing native list-like structures.  
    /// </remarks>
    /// <returns>The number of elements contained in this <see cref="IBinaryTable{T}"/>. </returns>
    int Count { get; }
}