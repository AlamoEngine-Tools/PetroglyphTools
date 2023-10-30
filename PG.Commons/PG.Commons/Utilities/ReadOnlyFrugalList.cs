using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PG.Commons.Utilities;

/// <summary>
/// A read-only variant of the <see cref="FrugalList{T}"/>.
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
public readonly struct ReadOnlyFrugalList<T> : IReadOnlyList<T?>
{
    /// <summary>
    /// Returns an empty <see cref="ReadOnlyFrugalList{T}"/> that has the specified type argument.
    /// </summary>
    public static readonly ReadOnlyFrugalList<T> Empty = default;

    private readonly FrugalList<T?> _list;

    /// <inheritdoc />
    public int Count => _list.Count;

    /// <inheritdoc />
    public T? this[int index] => _list[index];

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyFrugalList{T}"/> structure to one item.
    /// </summary>
    /// <param name="item">The item of the list.</param>
    public ReadOnlyFrugalList(T? item)
    {
        _list = new FrugalList<T?> { item };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyFrugalList{T}"/> structure with the given enumerable.
    /// </summary>
    /// <param name="items">The items of this list.</param>
    public ReadOnlyFrugalList(IEnumerable<T?> items)
    {
        _list = new FrugalList<T?>(items);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyFrugalList{T}"/> structure from a <see cref="FrugalList{T}"/>.
    /// </summary>
    /// <param name="items">The items of this list.</param>
    /// <remarks>
    /// Modifications to <paramref name="items"/> will not be reflected to this instance.
    /// </remarks>
    public ReadOnlyFrugalList(in FrugalList<T?> items)
    {
        _list = items;
    }

    /// <inheritdoc />
    public IEnumerator<T?> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

/// <summary>
/// A memory-optimized strongly typed list which avoids unnecessary memory allocations if none or one item is present.
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
public struct FrugalList<T> : IList<T?>
{
    private static readonly List<T> _emptyDummyList = new(0);

    private T? _firstItem;
    private List<T?>? _tailList;

    /// <inheritdoc />
    public int Count => _tailList is null ? 0 : 1 + _tailList.Count;

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public T? this[int index]
    {
        get
        {
            if (index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index should be non-negative and less than Count.");
            if (index == 0)
                return _firstItem;
            return _tailList![index - 1];
        }
        set
        {
            if ((uint)index >= (uint)Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index should be non-negative and less than Count.");
            if (index == 0)
                _firstItem = value;
            else
                _tailList![index - 1] = value;
        }
    }


    public ReadOnlyFrugalList(IEnumerable<T?> items)
    {

    }

    public ReadOnlyFrugalList(in ReadOnlyFrugalList<T?> items)
    {
    }

    /// <summary>
    /// Produces a read-only representation of the current state from this list.
    /// </summary>
    /// <returns>The read-only list.</returns>
    public ReadOnlyFrugalList<T?> ToReadOnly()
    {
        return new ReadOnlyFrugalList<T?>(in this!);
    }
    
    public void Add(T? item)
    {
        throw new System.NotImplementedException();
    }

    public void Clear()
    {
        throw new System.NotImplementedException();
    }

    public bool Contains(T? item)
    {
        throw new System.NotImplementedException();
    }

    public void CopyTo(T?[] array, int arrayIndex)
    {
        throw new System.NotImplementedException();
    }

    public bool Remove(T? item)
    {
        throw new System.NotImplementedException();
    }

    public int IndexOf(T? item)
    {
        throw new System.NotImplementedException();
    }

    public void Insert(int index, T? item)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new System.NotImplementedException();
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        return new FrugalEnumerator(in this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private struct FrugalEnumerator : IEnumerator<T?>
    {
        private readonly FrugalList<T> _list;
        private readonly int _count;

        private int _position;
        private T? _current;

        public T? Current => _current;

        object? IEnumerator.Current => _current;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FrugalEnumerator(in FrugalList<T> list)
        {
            _list = list;
            _count = list.Count;
            _position = 0;
            _current = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (_position < _count)
            {
                _current = _list[_position];
                ++_position;
                return true;
            }
            _position = _count + 1;
            _current = default;
            return false;
        }

        public void Reset()
        {
            _position = 0;
            _current = default;
        }

        public void Dispose()
        {
        }
    }
}

internal static class CollectionUtilities
{
    private static int TryGetCountWithoutEnumerating<T>(IEnumerable<T?> enumerable)
    {
        switch (enumerable)
        {
            case null:
                throw new ArgumentNullException(nameof(enumerable));
            case ICollection<T> collT:
                return collT.Count;
            case ICollection collection:
                return collection.Count;
            case IReadOnlyCollection<T> roc:
                return roc.Count;
            case string str:
                return str.Length;
            default:
                return -1; // -1 means, count could not be determined
        }
    }
}