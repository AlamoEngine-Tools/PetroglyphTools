using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PG.StarWarsGame.Files.MEG.Utilities;

internal class UnsignedList<T> : IUnsignedList<T>
{
    private readonly List<T> _lowerIndices;
    private List<T>? _upperIndices;

    public int Count => _lowerIndices.Count + (_upperIndices?.Count ?? 0);
    
    public T this[uint index]
    {
        get
        {
            if (index < Count)
            {
                if (index < _lowerIndices.Count)
                    return _lowerIndices[(int)index];
                if (_upperIndices != null)
                    return _upperIndices[(int)(index - _lowerIndices.Count)];
            }
            throw new IndexOutOfRangeException();
        }
        set
        {
            if (index < Count)
            {
                if (index < _lowerIndices.Count)
                    _lowerIndices[(int)index] = value;
                else if (_upperIndices != null)
                    _upperIndices[(int)(index - _lowerIndices.Count)] = value;
            }
            else
                throw new IndexOutOfRangeException();
        }
    }

    public UnsignedList()
    {
        _lowerIndices = new List<T>();
        _upperIndices = null;
    }

    public UnsignedList(uint size)
    {
        if (size > int.MaxValue)
        {
            _lowerIndices = new List<T>(int.MaxValue);
            _upperIndices = new List<T>((int)(size - int.MaxValue));
        }
        else
        {
            _lowerIndices = new List<T>((int)size);
            _upperIndices = null;
        }
    }

    public UnsignedList(IEnumerable<T> items)
    {
        if (items == null) 
            throw new ArgumentNullException(nameof(items));

        if (items is IReadOnlyUnsignedList<T> unsignedList)
        {
            var count = unsignedList.Count;
            if (count == 0)
            {
                _lowerIndices = new List<T>(0);
                _upperIndices = null;
            }
            else if (count < int.MaxValue)
            {
                _lowerIndices = new List<T>(items);
                _upperIndices = null;
            }
            else
            {
                _lowerIndices = new List<T>(int.MaxValue);
                _upperIndices = new List<T>(count - int.MaxValue);

                var index = 0;
                foreach (var item in unsignedList)
                {
                    if (index < int.MaxValue)
                        _lowerIndices.Add(item);
                    else
                        _upperIndices.Add(item);

                    index++;
                }
            }
        }
        // In this case, Count cannot be greater than Int32.MaxValue
        else
        {
            _lowerIndices = new List<T>(items);
            _upperIndices = null;
        }
    }

    public void Clear()
    {
        _lowerIndices.Clear();
        _upperIndices?.Clear();
        _upperIndices = null;
    }

    public bool Contains(T item)
    {
        return _lowerIndices.Contains(item) || (_upperIndices != null && _upperIndices.Contains(item));
    }

    public void Add(T item)
    {
        if (Count < int.MaxValue)
        {
            _lowerIndices.Add(item);
        }
        else
        {
            EnsureUpperIndicesInitialized();
            _upperIndices!.Add(item);
        }
    }

    public bool Remove(T item)
    {
        var removed = _lowerIndices.Remove(item);
        if (!removed && _upperIndices != null)
            removed = _upperIndices.Remove(item);

        return removed;
    }

    public void RemoveAt(uint index)
    {
        if (index < Count)
        {
            if (index < _lowerIndices.Count)
                _lowerIndices.RemoveAt((int)index);
            else
                _upperIndices?.RemoveAt((int)(index - _lowerIndices.Count));
        }
        else
        {
            throw new IndexOutOfRangeException();
        }
    }

    public void Insert(uint index, T item)
    {
        if (index <= Count)
        {
            if (index <= _lowerIndices.Count)
                _lowerIndices.Insert((int)index, item);
            else
            {
                EnsureUpperIndicesInitialized();
                _upperIndices!.Insert((int)(index - _lowerIndices.Count), item);
            }
        }
        else
        {
            throw new IndexOutOfRangeException();
        }
    }

    public bool TryGetIndex(T item, out uint? index)
    {
        var lowerIndex = _lowerIndices.IndexOf(item);
        if (lowerIndex != -1)
        {
            index = (uint)lowerIndex;
            return true;
        }

        var upperIndex = _upperIndices?.IndexOf(item) ?? -1;
        if (upperIndex != -1)
        {
            index = (uint)(_lowerIndices.Count + upperIndex);
            return true;
        }

        index = null;
        return false;
    }

    private void EnsureUpperIndicesInitialized()
    {
        _upperIndices ??= new List<T>();
    }


    public IEnumerator<T> GetEnumerator()
    {
        if (_upperIndices is null || _upperIndices.Count == 0)
            return _lowerIndices.GetEnumerator();
        return _lowerIndices.Concat(_upperIndices).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}