using System;
using System.Collections;
using System.Collections.Generic;

namespace PG.StarWarsGame.Files.MEG.Utilities;

internal class ReadOnlyUnsignedList<T> : IReadOnlyUnsignedList<T>
{
    private readonly IUnsignedList<T> _unsignedList;

    public int Count => _unsignedList.Count;

    public T this[uint index] => _unsignedList[index];


    public ReadOnlyUnsignedList(IUnsignedList<T> unsignedList)
    {
        _unsignedList = unsignedList ?? throw new ArgumentNullException(nameof(unsignedList));
    }
    public IEnumerator<T> GetEnumerator() => _unsignedList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}