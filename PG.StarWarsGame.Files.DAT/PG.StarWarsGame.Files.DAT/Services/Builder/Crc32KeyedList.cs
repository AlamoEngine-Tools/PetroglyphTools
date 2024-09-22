// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PG.Commons.DataTypes;
using PG.Commons.Hashing;

namespace PG.StarWarsGame.Files.DAT.Services.Builder;

/// <summary>
/// Specialized list that holds CRC32 hashable objects and allows fast [O(1)]
/// retrieval and modification to the first entry of a specific CRC32 checksum
/// </summary>
/// <typeparam name="TValue">The type of hashable elements in the list.</typeparam>
internal class Crc32KeyedList<TValue> where TValue : IHasCrc32
{
    private readonly Dictionary<Crc32, int> _keysWithIndexOfFirst = new();
    private readonly List<TValue> _items = new();
    private readonly BuilderOverrideKind _overrideBehavior;

    /// <summary>
    /// Initializes a new instance of the <see cref="Crc32KeyedList{TValue}"/> class and the specified override behavior.
    /// </summary>
    /// <param name="overrideBehavior">
    /// Specifies how the list shall behave if elements are get added
    /// where the checksum of the element is already present in the list.
    /// </param>
    public Crc32KeyedList(BuilderOverrideKind overrideBehavior)
    {
        _overrideBehavior = overrideBehavior;
    }

    public bool ContainsKey(Crc32 key, [MaybeNullWhen(false)] out TValue firstOrDefault)
    {
        firstOrDefault = default;
        if (!_keysWithIndexOfFirst.TryGetValue(key, out var index)) 
            return false;

        firstOrDefault = _items[index];
        return true;
    }

    public void AddOrReplace(Crc32 key, TValue value)
    {
        var containsKey = _keysWithIndexOfFirst.TryGetValue(key, out var index);
        if (containsKey)
        {
            if (_overrideBehavior == BuilderOverrideKind.NoOverwrite)
                throw new ArgumentException("This instance does not allow duplicates.", nameof(key));

            if (_overrideBehavior == BuilderOverrideKind.Overwrite)
            {
                _items[index] = value;
                return;
            }
        }

        _items.Add(value);

        if (!containsKey)
            _keysWithIndexOfFirst.Add(key, _items.Count - 1);
    }

    public void Clear()
    {
        _items.Clear();
        _keysWithIndexOfFirst.Clear();
    }

    public bool Remove(Crc32 key, TValue value)
    {
        var result =  _items.Remove(value);

        if (result)
        {
            var newIndex = _items.FindIndex(x => x.Crc32 == key);
            if (newIndex == -1)
                _keysWithIndexOfFirst.Remove(key);
            else
                _keysWithIndexOfFirst[key] = newIndex;
        }

        return result;
    }

    public bool RemoveAll(Crc32 key)
    {
        var result = _items.RemoveAll(i => i.Crc32 == key) > 0;
        _keysWithIndexOfFirst.Remove(key);
        return result;
    }

    public IReadOnlyList<TValue> GetValueList()
    {
        return _items.ToList();
    }
}