// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Commons.Collections;

/// <summary>
/// Represents a generic collection that maps keys to list of values, while maintaining the order of key insertion.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of the value-list in the dictionary.</typeparam>
public interface IValueListDictionary<TKey, TValue> : IReadOnlyValueListDictionary<TKey, TValue> where TKey : notnull
{
    /// <summary>
    /// Adds a value to the dictionary under the specified key.
    /// </summary>
    /// <remarks>
    /// Multiple values can be added under the same key. Values are stored in insertion order.
    /// </remarks>
    /// <param name="key">The key under which to add the value.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>
    /// <see langword="true"/> if the key already existed and the value was added 
    /// to an existing key; <see langword="false"/> if a new key was created.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    bool Add(TKey key, TValue value);

    /// <summary>
    /// Removes all keys and values from the <see cref="IValueListDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="IReadOnlyValueListDictionary{TKey, TValue}.Count"/> and 
    /// <see cref="IReadOnlyValueListDictionary{TKey, TValue}.KeyCount"/> are set to zero.
    /// </remarks>
    void Clear();
}