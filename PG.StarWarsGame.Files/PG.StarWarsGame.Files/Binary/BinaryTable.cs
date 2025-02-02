// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PG.StarWarsGame.Files.Binary;

/// <summary>
/// Represents a read-only list of binary elements. 
/// </summary>
/// <typeparam name="T">The type of the table's elements.</typeparam>
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

    /// <inheritdoc />
    public override void GetBytes(Span<byte> bytes)
    {
        if (bytes.Length < Size)
            throw new ArgumentException("Destination is too short.", nameof(bytes));
        if (Size == 0)
            return;
        if (Items.Count == 1)
        {
            Items[0].GetBytes(bytes);
            return;
        }

        var offset = 0;
        foreach (var item in Items)
        {
            item.GetBytes(bytes.Slice(offset));
            offset += item.Size;
        }
    }

    /// <inheritdoc/>
    protected override int GetSizeCore()
    {
        return Items.Count switch
        {
            0 => 0,
            1 => Items[0].Size,
            _ => GetSizeSlow()
        };
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetSizeSlow()
    {
        var count = 0;
        foreach (var item in Items)
            count += item.Size;
        return count;
    }
}