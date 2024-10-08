using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

/// <summary>
/// Represents a generic collection of key/value-list pairs.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of the value-list in the dictionary.</typeparam>
public interface IValueListDictionary<TKey, TValue> : IReadOnlyValueListDictionary<TKey, TValue> where TKey : notnull
{
    /// <summary>
    /// Adds an element with the provided key to the value-list of the dictionary.
    /// </summary>
    /// <param name="key">The object to use as the key of the element to add.</param>
    /// <param name="value">The object to use as the value of the element to add.</param>
    /// <returns><see langword="true"/> if values of the same <paramref name="key"/> already existed; otherwise, <see langword="false"/>.</returns>
    bool Add(TKey key, TValue value);
}

// NOT THREAD-SAFE!
public class ValueListDictionary<TKey, TValue> : IValueListDictionary<TKey, TValue> where TKey : notnull
{
    private readonly List<TKey> _insertionTrackingList = new();
    private readonly Dictionary<TKey, TValue> _singleValueDictionary;
    private readonly Dictionary<TKey, List<TValue>> _multiValueDictionary;

    private readonly IEqualityComparer<TKey> _equalityComparer;

    public int Count => _insertionTrackingList.Count;

    public TKey this[int index] => _insertionTrackingList[index];

    public ICollection<TKey> Keys => _singleValueDictionary.Keys.Concat(_multiValueDictionary.Keys).ToList();

    public ICollection<TValue> Values => this.Select(x => x.Value).ToList();

    public ValueListDictionary() : this(null)
    {
    }

    public ValueListDictionary(IEqualityComparer<TKey>? comparer)
    {
        _equalityComparer = comparer ?? EqualityComparer<TKey>.Default;
        _singleValueDictionary = new Dictionary<TKey, TValue>(_equalityComparer);
        _multiValueDictionary = new Dictionary<TKey, List<TValue>>(_equalityComparer);
    }

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

    public bool ContainsKey(TKey key)
    {
        return _singleValueDictionary.ContainsKey(key) || _multiValueDictionary.ContainsKey(key);
    }
    
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

    public TValue GetLastValue(TKey key)
    {
        if (_singleValueDictionary.TryGetValue(key, out var value))
            return value;

        if (_multiValueDictionary.TryGetValue(key, out var valueList))
            return valueList.Last();

        throw new KeyNotFoundException($"The key '{key}' was not found.");
    }

    public TValue GetFirstValue(TKey key)
    {
        if (_singleValueDictionary.TryGetValue(key, out var value))
            return value;

        if (_multiValueDictionary.TryGetValue(key, out var valueList))
            return valueList.First();

        throw new KeyNotFoundException($"The key '{key}' was not found.");
    }
    
    public ReadOnlyFrugalList<TValue> GetValues(TKey key)
    {
        if (TryGetValues(key, out var values)) 
            return values;

        throw new KeyNotFoundException($"The key '{key}' was not found.");

    }

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

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

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

        public KeyValuePair<TKey, TValue> Current =>
            _isMultiEnumeratorActive
                ? new KeyValuePair<TKey, TValue>(_multiEnumerator.Current.Key, _currentListEnumerator.Current)
                : _singleEnumerator.Current;

        object IEnumerator.Current => Current;

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

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            _singleEnumerator.Dispose();
            _multiEnumerator.Dispose();
        }
    }
}

public static class ValueListDictionaryExtensions
{
    public static IEnumerable<T> 
        AggregateValues<TKey, TValue, T>(
        this IReadOnlyValueListDictionary<TKey, TValue> valueListDictionary,
        AggregateStrategy aggregateStrategy,
        Predicate<T>? filter = null)
        where TKey : notnull
        where T : TValue
    {
        return valueListDictionary.AggregateValues(valueListDictionary.Keys, aggregateStrategy, filter);
    }

    public static IEnumerable<T> AggregateValues<TKey, TValue, T>(
        this IReadOnlyValueListDictionary<TKey, TValue> valueListDictionary,
        ICollection<TKey> keys,
        AggregateStrategy aggregateStrategy,
        Predicate<T>? filter = null)
        where TKey : notnull 
        where T : TValue
    {
        foreach (var key in keys)
        {
            if (!valueListDictionary.ContainsKey(key))
                continue;
            if (aggregateStrategy == AggregateStrategy.MultipleValuesPerKey)
            {
                foreach (var value in valueListDictionary.GetValues(key))
                {
                    if (value is not null)
                    {
                        var typedValue = (T)value;
                        if (filter is not null && filter(typedValue))
                            yield return typedValue;
                    }

                }
            }
            else
            {
                var value = aggregateStrategy == AggregateStrategy.FirstValuePerKey
                    ? valueListDictionary.GetFirstValue(key)
                    : valueListDictionary.GetLastValue(key);
                if (value is not null)
                {
                    var typedValue = (T)value;
                    if (filter is not null && filter(typedValue))
                        yield return typedValue;
                }
            }
        }
    }

    public static IEnumerable<T> AggregateValues<TKey, TValue, T>(
        this IReadOnlyValueListDictionary<TKey, TValue> valueListDictionary,
        Func<KeyValuePair<TKey, TValue>, T> selector,
        AggregateStrategy aggregateStrategy)
        where TKey : notnull
    {
        return valueListDictionary.AggregateValues(valueListDictionary.Keys, selector, aggregateStrategy);
    }

    public static IEnumerable<T> AggregateValues<TKey, TValue, T>(
        this IReadOnlyValueListDictionary<TKey, TValue> valueListDictionary,
        ICollection<TKey> keys,
        Func<KeyValuePair<TKey, TValue>, T> selector,
        AggregateStrategy aggregateStrategy)
        where TKey : notnull
    {
        foreach (var key in keys)
        {
            if (!valueListDictionary.ContainsKey(key))
                continue;
            if (aggregateStrategy == AggregateStrategy.MultipleValuesPerKey)
            {
                foreach (var value in valueListDictionary.GetValues(key))
                {
                    if (value is not null)
                        yield return selector(new KeyValuePair<TKey, TValue>(key, value));

                }
            }
            else
            {
                var value = aggregateStrategy == AggregateStrategy.FirstValuePerKey
                    ? valueListDictionary.GetFirstValue(key)
                    : valueListDictionary.GetLastValue(key);
                if (value is not null)
                {
                    yield return selector(new KeyValuePair<TKey, TValue>(key, value));
                }
            }
        }
    }

    public enum AggregateStrategy
    {
        FirstValuePerKey,
        LastValuePerKey,
        MultipleValuesPerKey,
    }
}