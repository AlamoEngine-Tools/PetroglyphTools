using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AnakinRaW.CommonUtilities.Collections;

namespace PG.Commons.Collections;

/// <summary>
/// Represents a generic read-only collection of key/value-list pairs.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the read-only dictionary.</typeparam>
/// <typeparam name="TValue">The type of the value-list in the read-only dictionary.</typeparam>
public interface IReadOnlyValueListDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : notnull
{
    /// <summary>
    /// Gets an enumerable collection that contains the values in the dictionary.
    /// </summary>
    /// <remarks>
    /// Modifications to the collection are not reflected in the dictionary.
    /// <br/>
    /// The order of the keys in the returned <see cref="ICollection{T}"/> is unspecified,
    /// but it is guaranteed that values of the same key get returned as a sequence in the order they were added.
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
    /// Gets the number of values contained in the dictionary.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the key at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the key to get.</param>
    /// <returns>The key at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the dictionary.</exception>
    TKey this[int index] { get; }

    /// <summary>
    /// Gets the value at the specified key index.
    /// </summary>
    /// <param name="index">The zero-based index of the key to get the value for.</param>
    /// <returns>The value at the specified key index.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the dictionary.</exception>
    TValue GetValueAtKeyIndex(int index);

    /// <summary>
    /// Determines whether the dictionary contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the dictionary.</param>
    /// <returns><see langword="true"/> if the dictionary contains the key; otherwise, <see langword="false"/>.</returns>
    bool ContainsKey(TKey key);

    /// <summary>
    /// Get the list of values stored at the specified key.
    /// </summary>
    /// <param name="key">The key to get the list of values for.</param>
    /// <returns>The list of values of the specified <paramref name="key"/>.</returns>
    ReadOnlyFrugalList<TValue> GetValues(TKey key);

    /// <summary>
    /// Gets the last element with the specified key.
    /// </summary>
    /// <param name="key">The key of the element to get.</param>
    /// <returns>The last element with the specified key.</returns>
    TValue GetLastValue(TKey key);

    /// <summary>
    /// Gets the first element with the specified key.
    /// </summary>
    /// <param name="key">The key of the element to get.</param>
    /// <returns>The first element with the specified key.</returns>
    TValue GetFirstValue(TKey key);

    /// <summary>
    /// Gets the first value associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose value to get.</param>
    /// <param name="value">
    /// When this method returns, the first value associated with the specified key, if the key is found;
    /// otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true"/> if the dictionary contains a value with the specified key; otherwise, <see langword="false"/>.</returns>
    bool TryGetFirstValue(TKey key, [NotNullWhen(true)] out TValue value);

    /// <summary>
    /// Gets the last value associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose value to get.</param>
    /// <param name="value">
    /// When this method returns, the last value associated with the specified key, if the key is found;
    /// otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true"/> if the dictionary contains a value with the specified key; otherwise, <see langword="false"/>.</returns>
    bool TryGetLastValue(TKey key, [NotNullWhen(true)] out TValue value);

    /// <summary>
    /// Gets the list of values associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose value to get.</param>
    /// <param name="values">
    /// When this method returns, a list of values associated with the specified key, if the key is found;
    /// otherwise, an empty list. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true"/> if the dictionary contains at least one value with the specified key; otherwise, <see langword="false"/>.</returns>
    bool TryGetValues(TKey key, out ReadOnlyFrugalList<TValue> values);
}