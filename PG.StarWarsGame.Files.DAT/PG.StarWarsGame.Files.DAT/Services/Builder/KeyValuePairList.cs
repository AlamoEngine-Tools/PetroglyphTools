// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace PG.StarWarsGame.Files.DAT.Services.Builder;

internal class KeyValuePairList<TKey, TValue> where TKey : notnull
{
    private readonly List<(TKey key, TValue value)> _items = new();

    public bool ContainsKey(TKey key, [NotNullWhen(true)] out TValue? firstOrDefault)
    {
        var firstEntry = _items.FirstOrDefault(i => EqualityComparer<TKey>.Default.Equals(i.key, key));
        firstOrDefault = default;
        if (firstEntry.Equals(default))
            return false;
        firstOrDefault = firstEntry.value!;
        return true;
    }

    public void Add(TKey key, TValue value)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));
        _items.Add((key, value));
    }

    public void AddOrReplace(TKey key, TValue value)
    {
        var index = _items.FindIndex(i => EqualityComparer<TKey>.Default.Equals(i.key, key));
        if (index == -1)
            Add(key, value);
        else
            _items[index] = (key, value);
    }

    public void Clear()
    {
        _items.Clear();
    }

    public bool Remove(TKey key, TValue value)
    {
        return _items.Remove((key, value));
    }

    public bool RemoveAll(TKey key)
    {
        return _items.RemoveAll(i => EqualityComparer<TKey>.Default.Equals(i.key, key)) > 0;
    }

    public IReadOnlyList<TValue> GetValueList()
    {
        return _items.Select(i => i.value).ToList();
    }
}