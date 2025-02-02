// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.


// Currently we assume we don't support MEGs with data that is longer than signed int32. Thus, there is no need if these classes.

//namespace PG.StarWarsGame.Files.MEG.Utilities;

//internal interface IUnsignedList<T> : IReadOnlyUnsignedList<T>
//{
//    new T this[uint index] { get; set; }

//    void Clear();

//    void Add(T item);

//    bool Remove(T item);

//    void RemoveAt(uint index);

//    void Insert(uint index, T item);

//    bool Contains(T item);

//    bool TryGetIndex(T item, out uint? index);
//}

//internal interface IReadOnlyUnsignedList<out T> : IEnumerable<T>
//{
//    int Count { get; }

//    T this[uint index] { get; }
//}

//internal class UnsignedList<T> : IUnsignedList<T>
//{
//    private readonly List<T> _lowerIndices;
//    private List<T>? _upperIndices;

//    public int Count => _lowerIndices.Count + (_upperIndices?.Count ?? 0);

//    public T this[uint index]
//    {
//        get
//        {
//            if (index < Count)
//            {
//                if (index < _lowerIndices.Count)
//                    return _lowerIndices[(int)index];
//                if (_upperIndices != null)
//                    return _upperIndices[(int)(index - _lowerIndices.Count)];
//            }
//            throw new IndexOutOfRangeException();
//        }
//        set
//        {
//            if (index < Count)
//            {
//                if (index < _lowerIndices.Count)
//                    _lowerIndices[(int)index] = value;
//                else if (_upperIndices != null)
//                    _upperIndices[(int)(index - _lowerIndices.Count)] = value;
//            }
//            else
//                throw new IndexOutOfRangeException();
//        }
//    }

//    public UnsignedList()
//    {
//        _lowerIndices = new List<T>();
//        _upperIndices = null;
//    }

//    public UnsignedList(uint size)
//    {
//        if (size > int.MaxValue)
//        {
//            _lowerIndices = new List<T>(int.MaxValue);
//            _upperIndices = new List<T>((int)(size - int.MaxValue));
//        }
//        else
//        {
//            _lowerIndices = new List<T>((int)size);
//            _upperIndices = null;
//        }
//    }

//    public UnsignedList(IEnumerable<T> items)
//    {
//        if (items == null) 
//            throw new ArgumentNullException(nameof(items));

//        if (items is IReadOnlyUnsignedList<T> unsignedList)
//        {
//            var count = unsignedList.Count;
//            if (count == 0)
//            {
//                _lowerIndices = new List<T>(0);
//                _upperIndices = null;
//            }
//            else if (count < int.MaxValue)
//            {
//                _lowerIndices = new List<T>(items);
//                _upperIndices = null;
//            }
//            else
//            {
//                _lowerIndices = new List<T>(int.MaxValue);
//                _upperIndices = new List<T>(count - int.MaxValue);

//                var index = 0;
//                foreach (var item in unsignedList)
//                {
//                    if (index < int.MaxValue)
//                        _lowerIndices.Add(item);
//                    else
//                        _upperIndices.Add(item);

//                    index++;
//                }
//            }
//        }
//        // In this case, Count cannot be greater than Int32.MaxValue
//        else
//        {
//            _lowerIndices = new List<T>(items);
//            _upperIndices = null;
//        }
//    }

//    public void Clear()
//    {
//        _lowerIndices.Clear();
//        _upperIndices?.Clear();
//        _upperIndices = null;
//    }

//    public bool Contains(T item)
//    {
//        return _lowerIndices.Contains(item) || (_upperIndices != null && _upperIndices.Contains(item));
//    }

//    public void Add(T item)
//    {
//        if (Count < int.MaxValue)
//        {
//            _lowerIndices.Add(item);
//        }
//        else
//        {
//            EnsureUpperIndicesInitialized();
//            _upperIndices!.Add(item);
//        }
//    }

//    public bool Remove(T item)
//    {
//        var removed = _lowerIndices.Remove(item);
//        if (!removed && _upperIndices != null)
//            removed = _upperIndices.Remove(item);

//        return removed;
//    }

//    public void RemoveAt(uint index)
//    {
//        if (index < Count)
//        {
//            if (index < _lowerIndices.Count)
//                _lowerIndices.RemoveAt((int)index);
//            else
//                _upperIndices?.RemoveAt((int)(index - _lowerIndices.Count));
//        }
//        else
//        {
//            throw new IndexOutOfRangeException();
//        }
//    }

//    public void Insert(uint index, T item)
//    {
//        if (index <= Count)
//        {
//            if (index <= _lowerIndices.Count)
//                _lowerIndices.Insert((int)index, item);
//            else
//            {
//                EnsureUpperIndicesInitialized();
//                _upperIndices!.Insert((int)(index - _lowerIndices.Count), item);
//            }
//        }
//        else
//        {
//            throw new IndexOutOfRangeException();
//        }
//    }

//    public bool TryGetIndex(T item, out uint? index)
//    {
//        var lowerIndex = _lowerIndices.IndexOf(item);
//        if (lowerIndex != -1)
//        {
//            index = (uint)lowerIndex;
//            return true;
//        }

//        var upperIndex = _upperIndices?.IndexOf(item) ?? -1;
//        if (upperIndex != -1)
//        {
//            index = (uint)(_lowerIndices.Count + upperIndex);
//            return true;
//        }

//        index = null;
//        return false;
//    }

//    private void EnsureUpperIndicesInitialized()
//    {
//        _upperIndices ??= new List<T>();
//    }


//    public IEnumerator<T> GetEnumerator()
//    {
//        if (_upperIndices is null || _upperIndices.Count == 0)
//            return _lowerIndices.GetEnumerator();
//        return _lowerIndices.Concat(_upperIndices).GetEnumerator();
//    }

//    IEnumerator IEnumerable.GetEnumerator()
//    {
//        return GetEnumerator();
//    }
//}

//internal class ReadOnlyUnsignedList<T>(IUnsignedList<T> unsignedList) : IReadOnlyUnsignedList<T>
//{
//    private readonly IUnsignedList<T> _unsignedList = unsignedList ?? throw new ArgumentNullException(nameof(unsignedList));

//    public int Count => _unsignedList.Count;

//    public T this[uint index] => _unsignedList[index];


//    public IEnumerator<T> GetEnumerator() => _unsignedList.GetEnumerator();

//    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
//}