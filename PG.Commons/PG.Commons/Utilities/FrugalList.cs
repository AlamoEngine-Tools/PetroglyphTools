using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PG.Commons.Utilities;

// Design Notes for FrugalList<T>:
// 1. This mutable structure is internal, to limit public support for it. Mutable structs expose dangerous side effects such as:
//      var a := FrugalList<int> { 0, 0 }
//      var b := a          // Copy by value!
//      a[0] = 1            // Modification of first item does not get reflected to copies.
//      a[1] = 1            // Modification to all remaining items gets reflected to copies.
//      print(b[0])         // prints 0
//      print(b[1])         // prints 0
//
//
// 2. Also this list does not implement the non-generic IList interface for a single reason: 
// The IList.CopyTo(Array, int) does not work without re-implementing major parts of the .NET type system.
// E.g.
//      int[] is compatible to uint[] is compatible to byte[] (see. ECMA335 I.8.7.1 array-element-compatible-with)
//
// What .NET provides:
//
//      - Array.Copy() [needed for copying the _tailList]
//              respects ECMA335 rules.
//
//      - Array.SetValue() [need for copying the _firstItem]
//              does not fully respect ECMA335 ([...] 2. V and W have the same reduced type)
// Also,
//      - C# Reflection (T.IsAssignableFrom(Type)) does not provide means to correctly check for array compatibility. 
//
// TL:DR
// Imagine we have a FrugalList<int>:
//      ((IList)FrugalList<int>).CopyTo(array, 0) where
//              array is object[]
//          or 
//              array is uint[]
// This does not work out-of the without adding huge complexity. 
/// <summary>
/// A memory-optimized strongly typed list which avoids unnecessary memory allocations if none or one item is present.
/// </summary>
/// <remarks>
/// Note that this List is a <see langword="struct"/> and thus is copied by-value and not by-reference.
/// There are <b>side effects</b> involving modifications to the first item!
/// <br/>
/// Usage advise: To avoid side effects either box this structure (e.g, to <see cref="IList{T}"/> (this allocates ) or pass this structure as by-<see langword="ref"/>.
/// </remarks>
/// <typeparam name="T">The type of elements in the list.</typeparam>
internal struct FrugalList<T> : IList<T>
{
    private static readonly EqualityComparer<T> ItemComparer = EqualityComparer<T>.Default;
    private static readonly EmptyList EmptyDummyList = EmptyList.Instance;

    private T _firstItem = default!;
    private List<T>? _tailList;

    /// <inheritdoc />
    public readonly int Count => _tailList is null ? 0 : 1 + _tailList.Count;

    /// <inheritdoc />
    public readonly bool IsReadOnly => false;

    /// <inheritdoc />
    public T this[int index]
    {
        readonly get
        {
            if ((uint)index >= (uint)Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index should be non-negative and less than Count.");
            return index == 0 ? _firstItem : _tailList![index - 1];
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

    /// <summary>
    /// Initializes a new instance of the <see cref="FrugalList{T}"/> structure that contains elements copied from the specified list.
    /// </summary>
    /// <param name="collection">The list whose elements are copied to the new list.</param>
    /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
    public FrugalList(IEnumerable<T> collection)
    {
        if (collection == null) 
            throw new ArgumentNullException(nameof(collection));
        foreach (var item in collection) 
            Add(item);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrugalList{T}"/> structure that copies all elements from the given list.
    /// </summary>
    /// <param name="list">The list whose elements are copied to the new list.</param>
    /// <remarks>
    /// Modifications to <paramref name="list"/> will not be reflected to this instance.
    /// </remarks>
    public FrugalList(in FrugalList<T> list)
    {
        _firstItem = list._firstItem;
        if (list._tailList is null)
            return;
        _tailList = list._tailList is EmptyList ? EmptyDummyList : new List<T>(list._tailList);
    }

    /// <summary>
    /// Produces a read-only representation of the current state from this list.
    /// </summary>
    /// <remarks>
    /// Modifications to this instance will not be reflected to the newly create readonly list.
    /// </remarks>
    /// <returns>The read-only list.</returns>
    public readonly ReadOnlyFrugalList<T> AsReadOnly()
    {
        return new ReadOnlyFrugalList<T>(in this);
    }

    /// <inheritdoc />
    public void Add(T item)
    {
        if (_tailList == null)
        {
            _firstItem = item;
            _tailList = EmptyDummyList;
        }
        else
        {
            if (_tailList is EmptyList)
                _tailList = new List<T>(2);
            _tailList.Add(item);
        }
    }

    /// <inheritdoc cref="IList.Clear"/>
    public void Clear()
    {
        _firstItem = default!;
        if (_tailList is not EmptyList) 
            _tailList?.Clear();
        _tailList = null;
    }

    /// <inheritdoc />
    public readonly bool Contains(T item)
    {
        var count = Count;
        if (count > 0 && ItemComparer.Equals(_firstItem, item))
            return true;
        return count > 1 && _tailList!.Contains(item);
    }

    /// <inheritdoc />
    public readonly void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null)
            throw new ArgumentNullException(nameof(array));

        // Formally (arrayIndex < array.GetLowerBound(0)) but since only SZArrays are allowed this is also OK.
        if (arrayIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));

        var count = Count;

        if (count > array.Length - arrayIndex)
            throw new ArgumentException(nameof(arrayIndex));

        if (count > 0) 
            array[arrayIndex++] = _firstItem;

        if (count <= 1)
            return;
        _tailList!.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc />
    public bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index < 0)
            return false;
        RemoveAt(index);
        return true;
    }

    /// <inheritdoc />
    public readonly int IndexOf(T item)
    {
        var count = Count;
        switch (count)
        {
            case > 0 when ItemComparer.Equals(_firstItem, item):
                return 0;
            case > 1:
            {
                var indexInList = _tailList!.IndexOf(item);
                if (indexInList >= 0)
                    return 1 + indexInList;
                break;
            }
        }
        return -1;
    }

    /// <inheritdoc />
    public void Insert(int index, T item)
    {
        var count = Count;
        if ((uint)index > (uint)count)
            throw new ArgumentOutOfRangeException(nameof(index));
        if (index == 0)
        {
            switch (count)
            {
                case 0:
                    _tailList = EmptyDummyList;
                    break;
                case 1:
                    _tailList = new List<T>(2) { _firstItem };
                    break;
                default:
                    _tailList!.Insert(0, _firstItem);
                    break;
            }

            _firstItem = item;
        }
        else
        {
            if (_tailList == EmptyDummyList)
                _tailList = new List<T>(2);
            _tailList!.Insert(index - 1, item);
        }
    }

    /// <inheritdoc cref="IList.RemoveAt"/>
    public void RemoveAt(int index)
    {
        var count = Count;
        if ((uint)index >= (uint)count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index should be non-negative and less than Count.");
        if (index == 0)
        {
            if (count == 1)
            {
                _firstItem = default!;
                _tailList = null;
            }
            else
            {
                _firstItem = _tailList![0];
                if (count == 2)
                    _tailList = EmptyDummyList;
                else
                    _tailList.RemoveAt(0);
            }
        }
        else if (count == 2)
        {
            _tailList = EmptyDummyList;
        }
        else
        {
            _tailList!.RemoveAt(index - 1);
        }
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        return new FrugalEnumerator(ref this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private struct FrugalEnumerator : IEnumerator<T?>
    {
        private readonly FrugalList<T> _list;

        private int _position;
        private T _current;

        public readonly T Current => _current;

        readonly object? IEnumerator.Current => _current;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FrugalEnumerator(ref FrugalList<T> list)
        {
            _list = list;
            _position = 0;
            _current = default!;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (_position < _list.Count)
            {
                _current = _list[_position];
                ++_position;
                return true;
            }
            _position = _list.Count + 1;
            _current = default!;
            return false;
        }

        public void Reset()
        {
            _position = 0;
            _current = default!;
        }

        public void Dispose()
        {
        }
    }

    /// <summary>
    /// Private type exists so that we can perform type checking on that type rather than reference checking.
    /// </summary>
    private sealed class EmptyList : List<T>
    {
        public static readonly EmptyList Instance = new();

        private EmptyList() : base(0)
        {
        }
    }
}