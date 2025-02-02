// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AnakinRaW.CommonUtilities.Collections;

namespace PG.Commons.Collections;

/// <inheritdoc />
/// <remarks>
/// A <see cref="ValueListDictionary{TKey,TValue}"/> can support multiple readers concurrently, as long as the collection is not modified.
/// Even so, enumerating through a collection is intrinsically not a thread-safe procedure.
/// In the rare case where an enumeration contends with write accesses, the collection must be locked during the entire enumeration. 
/// </remarks>
public class ValueListDictionary<TKey, TValue> : IValueListDictionary<TKey, TValue> where TKey : notnull
{
    private readonly List<TKey> _insertionTrackingList = new();
    private readonly Dictionary<TKey, TValue> _singleValueDictionary;
    private readonly Dictionary<TKey, List<TValue>> _multiValueDictionary;

    private readonly IEqualityComparer<TKey> _equalityComparer;

    /// <inheritdoc />
    public int Count => _insertionTrackingList.Count;

    /// <inheritdoc />
    public TKey this[int index] => _insertionTrackingList[index];

    /// <inheritdoc />
    public virtual ICollection<TKey> Keys => _singleValueDictionary.Keys.Concat(_multiValueDictionary.Keys).ToList();

    /// <inheritdoc />
    public ICollection<TValue> Values => this.Select(x => x.Value).ToList();

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueListDictionary{TKey,TValue}"/> class that is empty and uses the default equality comparer for the key type.
    /// </summary>
    public ValueListDictionary() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueListDictionary{TKey,TValue}"/> class that is empty and uses the specified <see cref="IEqualityComparer"/>.
    /// </summary>
    /// <param name="comparer">The <see cref="IEqualityComparer"/> implementation to use when comparing keys, or null to use the default <see cref="IEqualityComparer"/> for the type of the key.</param>
    public ValueListDictionary(IEqualityComparer<TKey>? comparer)
    {
        _equalityComparer = comparer ?? EqualityComparer<TKey>.Default;
        _singleValueDictionary = new Dictionary<TKey, TValue>(_equalityComparer);
        _multiValueDictionary = new Dictionary<TKey, List<TValue>>(_equalityComparer);
    }

    /// <inheritdoc />
    public TValue GetValueAtKeyIndex(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        var key = this[index];
        if (_singleValueDictionary.TryGetValue(key, out var value))
            return value;

        if (index == 0)
            return _multiValueDictionary[key].First();
        
        if (index == Count - 1)
            return _multiValueDictionary[key].Last();

        var keyCount = 0;
        foreach (var k in _insertionTrackingList.Take(index + 1))
        {
            if (_equalityComparer.Equals(key, k))
                keyCount++;
        }

        return _multiValueDictionary[key][keyCount - 1];
    }

    /// <inheritdoc />
    public bool ContainsKey(TKey key)
    {
        return _singleValueDictionary.ContainsKey(key) || _multiValueDictionary.ContainsKey(key);
    }

    /// <inheritdoc />
    public bool Add(TKey key, TValue value)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        _insertionTrackingList.Add(key);

        if (!_singleValueDictionary.ContainsKey(key))
        {
            if (!_multiValueDictionary.TryGetValue(key, out var list))
            {
                _singleValueDictionary.Add(key, value);
                return false;
            }

            list.Add(value);
            return true;
        }

        Debug.Assert(_multiValueDictionary.ContainsKey(key) == false);

        var firstValue = _singleValueDictionary[key];
        _singleValueDictionary.Remove(key);

        _multiValueDictionary.Add(key, [
            firstValue,
            value
        ]);

        return true;
    }

    /// <inheritdoc />
    public TValue GetLastValue(TKey key)
    {
        if (_singleValueDictionary.TryGetValue(key, out var value))
            return value;

        if (_multiValueDictionary.TryGetValue(key, out var valueList))
            return valueList.Last();

        throw new KeyNotFoundException($"The key '{key}' was not found.");
    }

    /// <inheritdoc />
    public TValue GetFirstValue(TKey key)
    {
        if (_singleValueDictionary.TryGetValue(key, out var value))
            return value;

        if (_multiValueDictionary.TryGetValue(key, out var valueList))
            return valueList.First();

        throw new KeyNotFoundException($"The key '{key}' was not found.");
    }

    /// <inheritdoc />
    public ReadOnlyFrugalList<TValue> GetValues(TKey key)
    {
        if (TryGetValues(key, out var values)) 
            return values;

        throw new KeyNotFoundException($"The key '{key}' was not found.");

    }

    /// <inheritdoc />
    public bool TryGetFirstValue(TKey key, [NotNullWhen(true)] out TValue value)
    {
        if (_singleValueDictionary.TryGetValue(key, out value!))
            return true;

        if (_multiValueDictionary.TryGetValue(key, out var valueList))
        {
            value = valueList.First()!;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public bool TryGetLastValue(TKey key, [NotNullWhen(true)] out TValue value)
    {
        if (_singleValueDictionary.TryGetValue(key, out value!))
            return true;

        if (_multiValueDictionary.TryGetValue(key, out var valueList))
        {
            value = valueList.Last()!;
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public bool TryGetValues(TKey key, out ReadOnlyFrugalList<TValue> values)
    {
        if (_singleValueDictionary.TryGetValue(key, out var value))
        {
            values = new ReadOnlyFrugalList<TValue>(value);
            return true;
        }

        if (_multiValueDictionary.TryGetValue(key, out var valueList))
        {
            values = new ReadOnlyFrugalList<TValue>(valueList);
            return true;
        }

        values = ReadOnlyFrugalList<TValue>.Empty;
        return false;
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Enumerates the values of a <see cref="ValueListDictionary{TKey,TValue}"/>.
    /// </summary>
    public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private Dictionary<TKey, TValue>.Enumerator _singleEnumerator;
        private Dictionary<TKey, List<TValue>>.Enumerator _multiEnumerator;
        private List<TValue>.Enumerator _currentListEnumerator = default;
        private bool _isMultiEnumeratorActive = false;

        internal Enumerator(ValueListDictionary<TKey, TValue> valueListDictionary)
        {
            _singleEnumerator = valueListDictionary._singleValueDictionary.GetEnumerator();
            _multiEnumerator = valueListDictionary._multiValueDictionary.GetEnumerator();
        }

        /// <inheritdoc />
        public KeyValuePair<TKey, TValue> Current =>
            _isMultiEnumeratorActive
                ? new KeyValuePair<TKey, TValue>(_multiEnumerator.Current.Key, _currentListEnumerator.Current)
                : _singleEnumerator.Current;

        object IEnumerator.Current => Current;

        /// <inheritdoc />
        public bool MoveNext()
        {
            if (_singleEnumerator.MoveNext())
                return true;

            if (_isMultiEnumeratorActive)
            {
                if (_currentListEnumerator.MoveNext())
                    return true;
                _isMultiEnumeratorActive = false;
            }

            if (_multiEnumerator.MoveNext())
            {
                _currentListEnumerator = _multiEnumerator.Current.Value.GetEnumerator();
                _isMultiEnumeratorActive = true;
                return _currentListEnumerator.MoveNext();
            }

            return false;
        }

        /// <inheritdoc />
        public void Reset()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _singleEnumerator.Dispose();
            _multiEnumerator.Dispose();
        }
    }
}