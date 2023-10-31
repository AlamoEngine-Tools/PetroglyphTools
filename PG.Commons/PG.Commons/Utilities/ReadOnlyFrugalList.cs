using System.Collections;
using System.Collections.Generic;

namespace PG.Commons.Utilities;

/// <summary>
/// A read-only variant of the <see cref="FrugalList{T}"/>.
/// </summary>
/// <typeparam name="T">The type of elements in the list.</typeparam>
public readonly struct ReadOnlyFrugalList<T> : IReadOnlyList<T>
{
    /// <summary>
    /// Returns an empty <see cref="ReadOnlyFrugalList{T}"/> that has the specified type argument.
    /// </summary>
    public static readonly ReadOnlyFrugalList<T> Empty = default;

    private readonly FrugalList<T> _list;

    /// <inheritdoc />
    public int Count => _list.Count;

    /// <inheritdoc />
    public T this[int index] => _list[index];

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyFrugalList{T}"/> structure to one item.
    /// </summary>
    /// <param name="item">The item of the list.</param>
    public ReadOnlyFrugalList(T item)
    {
        _list = new FrugalList<T> { item };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyFrugalList{T}"/> structure with the given enumerable.
    /// </summary>
    /// <param name="items">The items of this list.</param>
    public ReadOnlyFrugalList(IEnumerable<T> items)
    {
        _list = new FrugalList<T>(items);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyFrugalList{T}"/> structure from a <see cref="FrugalList{T}"/>.
    /// </summary>
    /// <param name="items">The items of this list.</param>
    /// <remarks>
    /// Modifications to <paramref name="items"/> will not be reflected to this instance.
    /// </remarks>
    public ReadOnlyFrugalList(in FrugalList<T> items)
    {
        _list = items;
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}