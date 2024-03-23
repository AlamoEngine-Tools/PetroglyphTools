using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PG.Commons.Binary;

/// <summary>
/// A read-only list of binary items. 
/// </summary>
/// <typeparam name="T"></typeparam>
public class BinaryTable<T> : BinaryBase, IBinaryTable<T> where T : IBinary
{
    /// <summary>
    /// Gets the underlying items of this table.
    /// </summary>
    protected readonly IReadOnlyList<T> Items;

    /// <inheritdoc cref="IReadOnlyList{T}.this"/>
    public T this[int i] => Items[i];

    T IReadOnlyList<T>.this[int i] => this[i];

    /// <inheritdoc cref="IReadOnlyCollection{T}.Count"/>
    public int Count => Items.Count;

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryTable{T}"/> that contains elements copied from the specified collection.
    /// </summary>
    /// <param name="items">The collection whose elements are copied to the new table.</param>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    public BinaryTable(IList<T> items)
    {
        if (items is null)
            throw new ArgumentNullException(nameof(items));
        Items = items.ToList();
    }

    /// <inheritdoc/>
    protected override int GetSizeCore()
    {
        return Items.Count switch
        {
            0 => 0,
            1 => Items[0].Size,
            _ => Items.Sum(item => item.Size)
        };
    }

    /// <inheritdoc/>
    protected override byte[] ToBytesCore()
    {
        if (Size == 0)
            return Array.Empty<byte>();
        var bytes = new List<byte>(Size);
        foreach (var item in Items)
            bytes.AddRange(item.Bytes);
        return bytes.ToArray();
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}