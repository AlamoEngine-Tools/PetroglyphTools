using PG.Commons.Collections;
using System.Collections.Generic;
using System;
using System.Collections;
using Xunit;

namespace PG.Commons.Test.Collections;

public class ValueListDictionaryTests
{
    [Fact]
    public void Constructor_Should_Initialize_EmptyDictionary()
    {
        var dictionary = new ValueListDictionary<string, int>();

        Assert.NotNull(dictionary);
        Assert.Empty(dictionary.Keys);
        Assert.Empty(dictionary.Values);
        Assert.Equal(0, dictionary.Count);
    }

    [Fact]
    public void Constructor_WithComparer_Should_InitializeWithCustomComparer()
    {
        var comparer = StringComparer.OrdinalIgnoreCase;

        var dictionary = new ValueListDictionary<string, int>(comparer);

        Assert.NotNull(dictionary);
        Assert.Empty(dictionary.Keys);
        Assert.Empty(dictionary.Values);
        Assert.Equal(0, dictionary.Count);

        dictionary.Add("KEY1", 1);
        Assert.True(dictionary.ContainsKey("key1"));
        Assert.True(dictionary.ContainsKey("KEY1"));

        dictionary.Add("key1", 2);
        Assert.True(dictionary.ContainsKey("key1"));
        Assert.True(dictionary.ContainsKey("KEY1"));

        Assert.Equal(2, dictionary.GetValues("kEy1").Count);
        Assert.Equal(2, dictionary.Count);
    }

    [Fact]
    public void Add_Should_AddSingleValue_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<string, int>();

        var added = dictionary.Add("Key1", 10);

        Assert.False(added); // No prior values
        Assert.Single(dictionary.Keys);
        Assert.Single(dictionary.Values);
        Assert.Equal(1, dictionary.Count);
    }

    [Fact]
    public void Add_Should_AddValueToExistingKey_WhenKeyAlreadyExists()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        var added = dictionary.Add("Key1", 20);

        Assert.True(added); // Prior values exist
        Assert.Single(dictionary.Keys);
        Assert.Equal(2, dictionary.Count);
    }

    [Fact]
    public void Keys_Should_ReturnAllKeysInDictionary()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key2", 20 }
        };

        var keys = dictionary.Keys;

        Assert.Equal(2, keys.Count);
        Assert.Contains("Key1", keys);
        Assert.Contains("Key2", keys);
    }

    [Fact]
    public void Values_Should_ReturnAllValuesInDictionary()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key2", 20 }
        };

        var values = dictionary.Values;

        Assert.Equal(2, values.Count);
        Assert.Contains(10, values);
        Assert.Contains(20, values);
    }

    [Fact]
    public void Indexer_Should_ReturnKeyAtIndex()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key2", 20 },
            { "Key1", 100}
        };

        var keyAtIndex0 = dictionary[0];
        var keyAtIndex1 = dictionary[1];
        var keyAtIndex2 = dictionary[2];

        Assert.Equal("Key1", keyAtIndex0);
        Assert.Equal("Key2", keyAtIndex1);
        Assert.Equal("Key1", keyAtIndex2);
    }

    [Fact]
    public void Indexer_Should_ThrowException_WhenIndexIsOutOfRange()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        Assert.Throws<ArgumentOutOfRangeException>(() => dictionary[1]);
    }

    [Fact]
    public void GetValueAtKeyIndex_Should_ReturnValueAtIndex()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 100 },
            { "Key2", 20},
            { "Key1", 1000},
        };

        var valueAtIndex0 = dictionary.GetValueAtKeyIndex(0);
        var valueAtIndex1 = dictionary.GetValueAtKeyIndex(1);
        var valueAtIndex2 = dictionary.GetValueAtKeyIndex(2);
        var valueAtIndex3 = dictionary.GetValueAtKeyIndex(3);

        Assert.Equal(10, valueAtIndex0);
        Assert.Equal(100, valueAtIndex1);
        Assert.Equal(20, valueAtIndex2);
        Assert.Equal(1000, valueAtIndex3);
    }

    [Fact]
    public void GetValueAtKeyIndex_Should_ThrowException_WhenIndexIsOutOfRange()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.GetValueAtKeyIndex(1));
    }

    [Fact]
    public void ContainsKey_Should_ReturnTrue_WhenKeyExists()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        var result = dictionary.ContainsKey("Key1");

        Assert.True(result);
    }

    [Fact]
    public void ContainsKey_Should_ReturnFalse_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<string, int>();

        var result = dictionary.ContainsKey("Key1");

        Assert.False(result);
    }

    [Fact]
    public void GetValues_Should_ReturnListOfValues_ForExistingKey()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 20 }
        }; 
        
        var values = dictionary.GetValues("Key1");

        Assert.Equal(2, values.Count);
        Assert.Equal([10, 20], values);
    }

    [Fact]
    public void GetValues_Should_ThrowException_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<string, int>();

        Assert.Throws<KeyNotFoundException>(() => dictionary.GetValues("Key1"));
    }

    [Fact]
    public void GetFirstValue_Should_ReturnFirstValue_ForExistingKey()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 100 },
            { "Key2", 20 },
        };

        var firstValueKey1 = dictionary.GetFirstValue("Key1");
        var firstValueKey2 = dictionary.GetFirstValue("Key2");

        Assert.Equal(10, firstValueKey1);
        Assert.Equal(20, firstValueKey2);
    }

    [Fact]
    public void GetFirstValue_Should_ThrowException_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<string, int>();

        Assert.Throws<KeyNotFoundException>(() => dictionary.GetFirstValue("Key1"));
    }

    [Fact]
    public void GetLastValue_Should_ReturnLastValue_ForExistingKey()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 100 },
            { "Key2", 20 },
        };

        var lastValueKey1 = dictionary.GetLastValue("Key1");
        var lastValueKey2 = dictionary.GetLastValue("Key2");

        Assert.Equal(100, lastValueKey1);
        Assert.Equal(20, lastValueKey2);
    }

    [Fact]
    public void GetLastValue_Should_ThrowException_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<string, int>();

        Assert.Throws<KeyNotFoundException>(() => dictionary.GetLastValue("Key1"));
    }

    [Fact]
    public void TryGetFirstValue_Should_ReturnTrueAndFirstValue_WhenKeyExists()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 100 },
            { "Key2", 20 },
        };

        var resultKey1 = dictionary.TryGetFirstValue("Key1", out var firstValueKey1);
        var resultKey2 = dictionary.TryGetFirstValue("Key2", out var firstValueKey2);

        Assert.True(resultKey1);
        Assert.Equal(10, firstValueKey1);

        Assert.True(resultKey2);
        Assert.Equal(20, firstValueKey2);
    }

    [Fact]
    public void TryGetFirstValue_Should_ReturnFalse_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<string, int>();

        var result = dictionary.TryGetFirstValue("Key1", out var firstValue);

        Assert.False(result);
        Assert.Equal(default, firstValue);
    }

    [Fact]
    public void TryGetLastValue_Should_ReturnTrueAndLastValue_WhenKeyExists()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 100 },
            { "Key2", 20 },
        };

        var resultKey1 = dictionary.TryGetLastValue("Key1", out var lastValueKey1);
        var resultKey2 = dictionary.TryGetLastValue("Key2", out var lastValueKey2);

        Assert.True(resultKey1);
        Assert.Equal(100, lastValueKey1);

        Assert.True(resultKey2);
        Assert.Equal(20, lastValueKey2);
    }

    [Fact]
    public void TryGetLastValue_Should_ReturnFalse_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<string, int>();

        var result = dictionary.TryGetLastValue("Key1", out var lastValue);

        Assert.False(result);
        Assert.Equal(default, lastValue);
    }

    [Fact]
    public void TryGetValues_Should_ReturnTrueAndListOfValues_WhenKeyExists()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 100 },
            { "Key2", 20 },
        };

        var resultKey1 = dictionary.TryGetValues("Key1", out var valuesKey1);
        var resultKey2 = dictionary.TryGetValues("Key2", out var valuesKey2);

        Assert.True(resultKey1);
        Assert.Equal(2, valuesKey1.Count);
        Assert.Equal([10, 100], valuesKey1);

        Assert.True(resultKey2);
        Assert.Single(valuesKey2);
        Assert.Equal([20], valuesKey2);
    }

    [Fact]
    public void TryGetValues_Should_ReturnFalse_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<string, int>();

        var result = dictionary.TryGetValues("Key1", out var values);

        Assert.False(result);
        Assert.Empty(values); // Expect empty list
    }

    [Fact]
    public void Count_Should_ReturnTotalNumberOfValuesInDictionary()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 20 },
            { "Key2", 30 }
        };

        var count = dictionary.Count;

        Assert.Equal(3, count); // 2 values for Key1, 1 value for Key2
    }

    [Fact]
    public void Enumerator_Should_IterateThroughAllKeyValuePairs()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key2", 20 },
            { "Key2", 30 }
        };

        var keyValuePairs = new List<KeyValuePair<string, int>>();

        var objEnumerator = ((IEnumerable)dictionary).GetEnumerator();
        foreach (var pair in dictionary)
        {
            Assert.True(objEnumerator.MoveNext());
            Assert.Equal(pair, objEnumerator.Current);
            keyValuePairs.Add(pair);
        }

        Assert.Equal(3, keyValuePairs.Count);
        Assert.Contains(new KeyValuePair<string, int>("Key1", 10), keyValuePairs);
        Assert.Contains(new KeyValuePair<string, int>("Key2", 20), keyValuePairs);
        Assert.Contains(new KeyValuePair<string, int>("Key2", 30), keyValuePairs);
    }

    [Fact]
    public void Add_Should_ThrowException_WhenNullKeyIsProvided()
    {
        var dictionary = new ValueListDictionary<string, int>();
        Assert.Throws<ArgumentNullException>(() => dictionary.Add(null!, 10));
    }

    [Fact]
    public void GetValues_Should_ThrowException_WhenNullKeyIsProvided()
    {
        var dictionary = new ValueListDictionary<string, int>();
        Assert.Throws<ArgumentNullException>(() => dictionary.GetValues(null!));
    }

    [Fact]
    public void ContainsKey_Should_ThrowException_WhenNullKeyIsProvided()
    {
        var dictionary = new ValueListDictionary<string, int>();
        Assert.Throws<ArgumentNullException>(() => dictionary.ContainsKey(null!));
    }

    [Fact]
    public void Add_Should_HandleLargeNumberOfEntries()
    {
        var dictionary = new ValueListDictionary<int, int>();
        const int largeCount = 1000000;

        for (var i = 0; i < largeCount; i++) 
            dictionary.Add(i, i);

        Assert.Equal(largeCount, dictionary.Count);
    }

    [Fact]
    public void GetValues_Should_HandleMultipleValuesForSameKey()
    {
        var dictionary = new ValueListDictionary<string, int>();
        const int largeCount = 1000;

        for (var i = 0; i < largeCount; i++) 
            dictionary.Add("Key1", i);

        var values = dictionary.GetValues("Key1");
        Assert.Equal(largeCount, values.Count);
        Assert.Equal(0, values[0]);
        Assert.Equal(largeCount - 1, values[values.Count - 1]); // Last value check
    }
}