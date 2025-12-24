// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AnakinRaW.CommonUtilities.Collections;

namespace PG.Commons.Collections;

/// <summary>
/// Represents a read-only generic collection that maps keys to list of values.
/// </summary>
/// <remarks>
/// <para>
/// Unlike a standard <see cref="IReadOnlyDictionary{TKey, TValue}"/>, this dictionary
/// allows multiple values to be associated with a single key.
/// </para>
/// <para>
/// When enumerating, each key appears exactly once with all its associated values
/// as a <see cref="ReadOnlyFrugalList{T}"/>.
/// </para>
/// </remarks>
/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
public interface IReadOnlyValueListDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, ReadOnlyFrugalList<TValue>>> where TKey : notnull
{
    /// <summary>
    /// Gets a collection containing all values in the dictionary.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Values are returned grouped by key, in the order the keys were first inserted.
    /// Within each key group, values appear in the order they were added.
    /// Modifications to the collection are not reflected in the dictionary.
    /// </para>
    /// </remarks>
    ICollection<TValue> Values { get; }

    /// <summary>
    /// Gets an <see cref="ICollection{T}"/> containing the keys in the dictionary.
    /// </summary>
    /// <remarks>
    /// Modifications to the collection are not reflected in the dictionary.
    /// <br/>
    /// The order of the keys in the returned <see cref="ICollection{T}"/> is unspecified.
    /// </remarks>
    ICollection<TKey> Keys { get; }

    /// <summary>
    /// Gets the total number of values across all keys in the dictionary.
    /// </summary>
    /// <remarks>
    /// This is the sum of all values for all keys, not the number of distinct keys.
    /// Use <see cref="KeyCount"/> to get the number of distinct keys.
    /// </remarks>
    int Count { get; }

    /// <summary>
    /// Gets the number of distinct keys in the dictionary.
    /// </summary>
    int KeyCount { get; }

    /// <summary>
    /// Determines whether the dictionary contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the dictionary.</param>
    /// <returns><see langword="true"/> if the dictionary contains the key; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    bool ContainsKey(TKey key);

    /// <summary>
    /// Get a list of values stored with the specified key.
    /// </summary>
    /// <param name="key">The key to get the list of values for.</param>
    /// <returns>The list of values of the specified <paramref name="key"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">The key does not exist in the dictionary.</exception>
    ReadOnlyFrugalList<TValue> GetValues(TKey key);

    /// <summary>
    /// Gets the last element with the specified key.
    /// </summary>
    /// <param name="key">The key of the element to get.</param>
    /// <returns>The last element with the specified key.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">The key does not exist in the dictionary.</exception>
    TValue GetLastValue(TKey key);

    /// <summary>
    /// Gets the first element with the specified key.
    /// </summary>
    /// <param name="key">The key of the element to get.</param>
    /// <returns>The first element with the specified key.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">The key does not exist in the dictionary.</exception>
    TValue GetFirstValue(TKey key);

    /// <summary>
    /// Gets the first value associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose value to get.</param>
    /// <param name="value">
    /// When this method returns, the first value associated with the specified key, if the key is found;
    /// otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true"/> if the dictionary contains a value with the specified key; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    bool TryGetFirstValue(TKey key, [NotNullWhen(true)] out TValue value);

    /// <summary>
    /// Gets the last value associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose value to get.</param>
    /// <param name="value">
    /// When this method returns, the last value associated with the specified key, if the key is found;
    /// otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true"/> if the dictionary contains a value with the specified key; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    bool TryGetLastValue(TKey key, [NotNullWhen(true)] out TValue value);

    /// <summary>
    /// Gets the list of values associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose value to get.</param>
    /// <param name="values">
    /// When this method returns, a list of values associated with the specified key, if the key is found;
    /// otherwise, an empty list. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true"/> if the dictionary contains at least one value with the specified key; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    bool TryGetValues(TKey key, out ReadOnlyFrugalList<TValue> values);
}