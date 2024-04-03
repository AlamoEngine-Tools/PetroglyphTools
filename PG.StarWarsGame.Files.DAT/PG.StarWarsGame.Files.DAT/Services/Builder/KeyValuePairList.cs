// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace PG.StarWarsGame.Files.DAT.Services.Builder;

internal class KeyValuePairList<TKey, TValue> where TKey : notnull
{
    private readonly HashSet<TKey> _keys = new();
    private readonly List<(TKey key, TValue value)> _items = new();

    public bool ContainsKey(TKey key, [NotNullWhen(true)] out TValue? firstOrDefault)
    {
        firstOrDefault = default;
        if (_keys.Contains(key))
            return false;
        var firstEntry = _items.FirstOrDefault(i => EqualityComparer<TKey>.Default.Equals(i.key, key));
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
        _keys.Add(key);
    }

    public void AddOrReplace(TKey key, TValue value)
    {
        if (!_keys.Contains(key))
            Add(key, value);
        else
        {
            var index = _items.FindIndex(i => EqualityComparer<TKey>.Default.Equals(i.key, key));
            _items[index] = (key, value);
        }
    }

    public void Clear()
    {
        _items.Clear();
        _keys.Clear();
    }

    public bool Remove(TKey key, TValue value)
    {
        var result =  _items.Remove((key, value));
        if (!_items.Any(x => EqualityComparer<TKey>.Default.Equals(x.key, key)))
            _keys.Remove(key);
        return result;
    }

    public bool RemoveAll(TKey key)
    {
        var result = _items.RemoveAll(i => EqualityComparer<TKey>.Default.Equals(i.key, key)) > 0;
        _keys.Remove(key);
        return result;
    }

    public IReadOnlyList<TValue> GetValueList()
    {
        return _items.Select(i => i.value).ToList();
    }
}