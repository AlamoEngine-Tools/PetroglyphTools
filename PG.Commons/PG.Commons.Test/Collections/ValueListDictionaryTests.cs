using PG.Commons.Collections;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;
using AnakinRaW.CommonUtilities.Collections;
using Xunit;

namespace PG.Commons.Test.Collections;

public class ValueListDictionaryTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_Default_InitializesEmptyDictionary()
    {
        var dictionary = new ValueListDictionary<string, int>();

        Assert.NotNull(dictionary);
        Assert.Empty(dictionary.Keys);
        Assert.Empty(dictionary.Values);
        Assert.Equal(0, dictionary.Count);
        Assert.Equal(0, dictionary.KeyCount);
    }

    [Fact]
    public void Constructor_WithNullComparer_UsesDefaultComparer()
    {
        var dictionary = new ValueListDictionary<string, int>(null) { { "Key1", 1 } };

        Assert.True(dictionary.ContainsKey("Key1"));
        Assert.False(dictionary.ContainsKey("key1"));
    }

    [Fact]
    public void Constructor_WithCustomComparer_UsesProvidedComparer()
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        var dictionary = new ValueListDictionary<string, int>(comparer)
        {
            { "KEY1", 1 },
            { "key1", 2 }
        };

        Assert.True(dictionary.ContainsKey("key1"));
        Assert.True(dictionary.ContainsKey("KEY1"));
        Assert.True(dictionary.ContainsKey("Key1"));
        Assert.Equal(2, dictionary.GetValues("kEy1").Count);
        Assert.Equal(2, dictionary.Count);
        Assert.Equal(1, dictionary.KeyCount);
    }

    #endregion

    #region Count and KeyCount Tests

    [Fact]
    public void Count_ReturnsZero_WhenEmpty()
    {
        var dictionary = new ValueListDictionary<string, int>();
        Assert.Equal(0, dictionary.Count);
    }

    [Fact]
    public void Count_ReturnsTotalNumberOfValues()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 20 },
            { "Key2", 30 }
        };

        Assert.Equal(3, dictionary.Count);
    }

    [Fact]
    public void KeyCount_ReturnsZero_WhenEmpty()
    {
        var dictionary = new ValueListDictionary<string, int>();
        Assert.Equal(0, dictionary.KeyCount);
    }

    [Fact]
    public void KeyCount_ReturnsNumberOfDistinctKeys()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 20 },
            { "Key2", 30 },
            { "Key3", 40 }
        };

        Assert.Equal(3, dictionary.KeyCount);
        Assert.Equal(4, dictionary.Count);
    }

    #endregion

    #region Add Tests

    [Fact]
    public void Add_ReturnsFalse_WhenKeyIsNew()
    {
        var dictionary = new ValueListDictionary<string, int>();

        var result = dictionary.Add("Key1", 10);

        Assert.False(result);
        Assert.Equal(1, dictionary.Count);
        Assert.Equal(1, dictionary.KeyCount);
    }

    [Fact]
    public void Add_ReturnsTrue_WhenKeyExists()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        var result = dictionary.Add("Key1", 20);

        Assert.True(result);
        Assert.Equal(2, dictionary.Count);
        Assert.Equal(1, dictionary.KeyCount);
    }

    [Fact]
    public void Add_ThrowsArgumentNullException_WhenKeyIsNull()
    {
        var dictionary = new ValueListDictionary<string, int>();
        Assert.Throws<ArgumentNullException>(() => dictionary.Add(null!, 10));
    }

    [Fact]
    public void Add_AllowsNullValue_ForReferenceTypes()
    {
        var dictionary = new ValueListDictionary<string, string> { { "Key1", null! } };

        Assert.Equal(1, dictionary.Count);
        Assert.Null(dictionary.GetFirstValue("Key1"));
    }

    [Fact]
    public void Add_PreservesInsertionOrder()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "C", 3 },
            { "A", 1 },
            { "B", 2 }
        };

        var keys = dictionary.Keys.ToList();
        Assert.Equal(["C", "A", "B"], keys);
    }

    [Fact]
    public void Add_PreservesValueOrderPerKey()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 1 },
            { "Key1", 2 },
            { "Key1", 3 }
        };

        var values = dictionary.GetValues("Key1");
        Assert.Equal([1, 2, 3], values);
    }

    [Fact]
    public void Add_HandlesLargeNumberOfEntries()
    {
        var dictionary = new ValueListDictionary<int, int>();
        const int largeCount = 100000;

        for (var i = 0; i < largeCount; i++)
            dictionary.Add(i, i);

        Assert.Equal(largeCount, dictionary.Count);
        Assert.Equal(largeCount, dictionary.KeyCount);
    }

    [Fact]
    public void Add_HandlesLargeNumberOfValuesPerKey()
    {
        var dictionary = new ValueListDictionary<string, int>();
        const int largeCount = 10000;

        for (var i = 0; i < largeCount; i++)
            dictionary.Add("Key1", i);

        Assert.Equal(largeCount, dictionary.Count);
        Assert.Equal(1, dictionary.KeyCount);
        Assert.Equal(largeCount, dictionary.GetValues("Key1").Count);
    }

    #endregion

    #region Clear Tests

    [Fact]
    public void Clear_OnEmptyDictionary_DoesNothing()
    {
        var dictionary = new ValueListDictionary<string, int>();

        dictionary.Clear();

        Assert.Equal(0, dictionary.Count);
        Assert.Equal(0, dictionary.KeyCount);
    }

    [Fact]
    public void Clear_RemovesAllKeysAndValues()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "a", 1 },
            { "a", 2 },
            { "b", 3 }
        };

        dictionary.Clear();

        Assert.Equal(0, dictionary.Count);
        Assert.Equal(0, dictionary.KeyCount);
        Assert.Empty(dictionary.Keys);
        Assert.Empty(dictionary.Values);
        Assert.False(dictionary.ContainsKey("a"));
        Assert.False(dictionary.ContainsKey("b"));
    }

    [Fact]
    public void Clear_AllowsReuseAfterClearing()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 1 } };

        dictionary.Clear();
        dictionary.Add("Key2", 2);

        Assert.Equal(1, dictionary.Count);
        Assert.Equal(1, dictionary.KeyCount);
        Assert.True(dictionary.ContainsKey("Key2"));
        Assert.False(dictionary.ContainsKey("Key1"));
    }

    #endregion

    #region ContainsKey Tests

    [Fact]
    public void ContainsKey_ReturnsTrue_WhenKeyExists()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        Assert.True(dictionary.ContainsKey("Key1"));
    }

    [Fact]
    public void ContainsKey_ReturnsFalse_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<string, int>();

        Assert.False(dictionary.ContainsKey("Key1"));
    }

    [Fact]
    public void ContainsKey_ThrowsArgumentNullException_WhenKeyIsNull()
    {
        var dictionary = new ValueListDictionary<string, int>();
        Assert.Throws<ArgumentNullException>(() => dictionary.ContainsKey(null!));
    }

    #endregion

    #region GetValues Tests

    [Fact]
    public void GetValues_ReturnsAllValues_ForExistingKey()
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
    public void GetValues_ReturnsSingleValue_WhenKeyHasOneValue()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        var values = dictionary.GetValues("Key1");

        Assert.Single(values);
        Assert.Equal(10, values[0]);
    }

    [Fact]
    public void GetValues_ThrowsKeyNotFoundException_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<string, int>();

        var ex = Assert.Throws<KeyNotFoundException>(() => dictionary.GetValues("Key1"));
        Assert.Contains("Key1", ex.Message);
    }

    [Fact]
    public void GetValues_ThrowsArgumentNullException_WhenKeyIsNull()
    {
        var dictionary = new ValueListDictionary<string, int>();
        Assert.Throws<ArgumentNullException>(() => dictionary.GetValues(null!));
    }

    #endregion

    #region GetFirstValue Tests

    [Fact]
    public void GetFirstValue_ReturnsFirstValue_WhenKeyHasMultipleValues()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 20 },
            { "Key1", 30 }
        };

        Assert.Equal(10, dictionary.GetFirstValue("Key1"));
    }

    [Fact]
    public void GetFirstValue_ReturnsValue_WhenKeyHasSingleValue()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        Assert.Equal(10, dictionary.GetFirstValue("Key1"));
    }

    [Fact]
    public void GetFirstValue_ThrowsKeyNotFoundException_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<string, int>();

        var ex = Assert.Throws<KeyNotFoundException>(() => dictionary.GetFirstValue("Key1"));
        Assert.Contains("Key1", ex.Message);
    }

    #endregion

    #region GetLastValue Tests

    [Fact]
    public void GetLastValue_ReturnsLastValue_WhenKeyHasMultipleValues()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 20 },
            { "Key1", 30 }
        };

        Assert.Equal(30, dictionary.GetLastValue("Key1"));
    }

    [Fact]
    public void GetLastValue_ReturnsValue_WhenKeyHasSingleValue()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        Assert.Equal(10, dictionary.GetLastValue("Key1"));
    }

    [Fact]
    public void GetLastValue_ThrowsKeyNotFoundException_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<string, int>();

        var ex = Assert.Throws<KeyNotFoundException>(() => dictionary.GetLastValue("Key1"));
        Assert.Contains("Key1", ex.Message);
    }

    #endregion

    #region TryGetFirstValue Tests

    [Fact]
    public void TryGetFirstValue_ReturnsTrueAndFirstValue_WhenKeyExists()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 20 }
        };

        var result = dictionary.TryGetFirstValue("Key1", out var value);

        Assert.True(result);
        Assert.Equal(10, value);
    }

    [Fact]
    public void TryGetFirstValue_ReturnsFalseAndDefault_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<string, int>();

        var result = dictionary.TryGetFirstValue("Key1", out var value);

        Assert.False(result);
        Assert.Equal(default, value);
    }

    [Fact]
    public void TryGetFirstValue_ReturnsTrueAndNull_WhenValueIsNull()
    {
        var dictionary = new ValueListDictionary<string, string> { { "Key1", null! } };

        var result = dictionary.TryGetFirstValue("Key1", out var value);

        Assert.True(result);
        Assert.Null(value);
    }

    #endregion

    #region TryGetLastValue Tests

    [Fact]
    public void TryGetLastValue_ReturnsTrueAndLastValue_WhenKeyExists()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 20 }
        };

        var result = dictionary.TryGetLastValue("Key1", out var value);

        Assert.True(result);
        Assert.Equal(20, value);
    }

    [Fact]
    public void TryGetLastValue_ReturnsFalseAndDefault_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<string, int>();

        var result = dictionary.TryGetLastValue("Key1", out var value);

        Assert.False(result);
        Assert.Equal(default, value);
    }

    #endregion

    #region TryGetValues Tests

    [Fact]
    public void TryGetValues_ReturnsTrueAndValues_WhenKeyExists()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 20 }
        };

        var result = dictionary.TryGetValues("Key1", out var values);

        Assert.True(result);
        Assert.Equal([10, 20], values);
    }

    [Fact]
    public void TryGetValues_ReturnsFalseAndEmptyList_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<string, int>();

        var result = dictionary.TryGetValues("Key1", out var values);

        Assert.False(result);
        Assert.Empty(values);
    }

    #endregion

    #region Dictionary Enumerator Tests

    [Fact]
    public void GetEnumerator_ReturnsEmptyEnumerator_WhenDictionaryIsEmpty()
    {
        var dictionary = new ValueListDictionary<string, int>();

        using var enumerator = dictionary.GetEnumerator();

        Assert.False(enumerator.MoveNext());
    }

    [Fact]
    public void GetEnumerator_EnumeratesAllKeyValueGroups()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key2", 20 },
            { "Key2", 30 }
        };

        var pairs = new List<KeyValuePair<string, ReadOnlyFrugalList<int>>>();
        foreach (var pair in dictionary)
            pairs.Add(pair);

        Assert.Equal(2, pairs.Count);

        Assert.Equal("Key1", pairs[0].Key);
        Assert.Equal([10], pairs[0].Value);

        Assert.Equal("Key2", pairs[1].Key);
        Assert.Equal([20, 30], pairs[1].Value);
    }

    [Fact]
    public void GetEnumerator_PreservesKeyInsertionOrder()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "C", 3 },
            { "A", 1 },
            { "B", 2 }
        };

        var keys = dictionary.Select(kvp => kvp.Key).ToList();

        Assert.Equal(["C", "A", "B"], keys);
    }

    [Fact]
    public void Enumerator_Current_ReturnsCurrentElement()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        using var enumerator = dictionary.GetEnumerator();
        enumerator.MoveNext();

        Assert.Equal("Key1", enumerator.Current.Key);
        Assert.Equal([10], enumerator.Current.Value);
    }

    [Fact]
    public void Enumerator_Reset_ResetsToBeginning()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key2", 20 }
        };

        using var enumerator = dictionary.GetEnumerator();
        enumerator.MoveNext();
        enumerator.MoveNext();
        enumerator.Reset();

        Assert.True(enumerator.MoveNext());
        Assert.Equal("Key1", enumerator.Current.Key);
    }

    [Fact]
    public void Enumerator_Dispose_CanBeCalledMultipleTimes()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        var enumerator = dictionary.GetEnumerator();
        enumerator.Dispose();
        enumerator.Dispose(); // Should not throw
    }

    [Fact]
    public void Enumerator_NonGenericCurrent_ReturnsBoxedValue()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        var enumerator = ((IEnumerable)dictionary).GetEnumerator();
        enumerator.MoveNext();

        var current = (KeyValuePair<string, ReadOnlyFrugalList<int>>)enumerator.Current;
        Assert.Equal("Key1", current.Key);
    }

    [Fact]
    public void Enumerator_GenericInterface_ReturnsCorrectEnumerator()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        IEnumerable<KeyValuePair<string, ReadOnlyFrugalList<int>>> enumerable = dictionary;
        using var enumerator = enumerable.GetEnumerator();

        Assert.True(enumerator.MoveNext());
        Assert.Equal("Key1", enumerator.Current.Key);
    }

    #endregion

    #region KeyCollection Tests

    [Fact]
    public void Keys_ReturnsSameInstance()
    {
        var dictionary = new ValueListDictionary<string, int>();

        var keys1 = dictionary.Keys;
        var keys2 = dictionary.Keys;

        Assert.Same(keys1, keys2);
    }

    [Fact]
    public void Keys_ReflectsChangesToDictionary()
    {
        var dictionary = new ValueListDictionary<string, int>();
        var keys = dictionary.Keys;

        Assert.Equal(0, keys.Count);

        dictionary.Add("Key1", 10);
        Assert.Equal(1, keys.Count);

        dictionary.Add("Key2", 20);
        Assert.Equal(2, keys.Count);

        dictionary.Clear();
        Assert.Equal(0, keys.Count);
    }

    [Fact]
    public void Keys_Count_ReturnsNumberOfDistinctKeys()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 20 },
            { "Key2", 30 }
        };

        Assert.Equal(2, dictionary.Keys.Count);
    }

    [Fact]
    public void Keys_IsReadOnly_ReturnsTrue()
    {
        var dictionary = new ValueListDictionary<string, int>();

        Assert.True(dictionary.Keys.IsReadOnly);
    }

    [Fact]
    public void Keys_Contains_ReturnsTrueForExistingKey()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        Assert.True(dictionary.Keys.Contains("Key1"));
    }

    [Fact]
    public void Keys_Contains_ReturnsFalseForNonExistingKey()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        Assert.False(dictionary.Keys.Contains("Key2"));
    }

    [Fact]
    public void Keys_CopyTo_CopiesKeysToArray()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key2", 20 }
        };

        var array = new string[2];
        dictionary.Keys.CopyTo(array, 0);

        Assert.Equal(["Key1", "Key2"], array);
    }

    [Fact]
    public void Keys_CopyTo_CopiesKeysToArrayAtIndex()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key2", 20 }
        };

        var array = new string[4];
        dictionary.Keys.CopyTo(array, 2);

        Assert.Equal((string[])[null!, null!, "Key1", "Key2"], array);
    }

    [Fact]
    public void Keys_CopyTo_ThrowsArgumentNullException_WhenArrayIsNull()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        Assert.Throws<ArgumentNullException>(() => dictionary.Keys.CopyTo(null!, 0));
    }

    [Fact]
    public void Keys_CopyTo_ThrowsArgumentOutOfRangeException_WhenIndexIsNegative()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.Keys.CopyTo(new string[1], -1));
    }

    [Fact]
    public void Keys_CopyTo_ThrowsArgumentException_WhenArrayIsTooSmall()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key2", 20 }
        };

        Assert.Throws<ArgumentException>(() => dictionary.Keys.CopyTo(new string[1], 0));
    }

    [Fact]
    public void Keys_CopyTo_ThrowsArgumentException_WhenNotEnoughSpaceFromIndex()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key2", 20 }
        };

        Assert.Throws<ArgumentException>(() => dictionary.Keys.CopyTo(new string[3], 2));
    }

    [Fact]
    public void Keys_GetEnumerator_EnumeratesKeysInInsertionOrder()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "C", 3 },
            { "A", 1 },
            { "B", 2 }
        };

        var keys = dictionary.Keys.ToList();

        Assert.Equal(["C", "A", "B"], keys);
    }

    [Fact]
    public void Keys_GetEnumerator_NonGeneric_EnumeratesKeys()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        var keys = new List<string>();
        foreach (var key in (IEnumerable)dictionary.Keys)
            keys.Add((string)key);

        Assert.Equal(["Key1"], keys);
    }

    [Fact]
    public void Keys_Add_ThrowsNotSupportedException()
    {
        var dictionary = new ValueListDictionary<string, int>();
        ICollection<string> keys = dictionary.Keys;

        Assert.Throws<NotSupportedException>(() => keys.Add("Key1"));
    }

    [Fact]
    public void Keys_Clear_ThrowsNotSupportedException()
    {
        var dictionary = new ValueListDictionary<string, int>();
        ICollection<string> keys = dictionary.Keys;

        Assert.Throws<NotSupportedException>(() => keys.Clear());
    }

    [Fact]
    public void Keys_Remove_ThrowsNotSupportedException()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };
        ICollection<string> keys = dictionary.Keys;

        Assert.Throws<NotSupportedException>(() => keys.Remove("Key1"));
    }

    [Fact]
    public void Keys_InterfaceProperty_ReturnsSameCollectionAsConcreteProperty()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        IReadOnlyValueListDictionary<string, int> readOnlyDict = dictionary;

        Assert.Same(dictionary.Keys, readOnlyDict.Keys);
    }

    #endregion

    #region ValueCollection Tests

    [Fact]
    public void Values_ReturnsSameInstance()
    {
        var dictionary = new ValueListDictionary<string, int>();

        var values1 = dictionary.Values;
        var values2 = dictionary.Values;

        Assert.Same(values1, values2);
    }

    [Fact]
    public void Values_ReflectsChangesToDictionary()
    {
        var dictionary = new ValueListDictionary<string, int>();
        var values = dictionary.Values;

        Assert.Equal(0, values.Count);

        dictionary.Add("Key1", 10);
        Assert.Equal(1, values.Count);

        dictionary.Add("Key1", 20);
        Assert.Equal(2, values.Count);

        dictionary.Clear();
        Assert.Equal(0, values.Count);
    }

    [Fact]
    public void Values_Count_ReturnsTotalNumberOfValues()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 20 },
            { "Key2", 30 }
        };

        Assert.Equal(3, dictionary.Values.Count);
    }

    [Fact]
    public void Values_IsReadOnly_ReturnsTrue()
    {
        var dictionary = new ValueListDictionary<string, int>();

        Assert.True(dictionary.Values.IsReadOnly);
    }

    [Fact]
    public void Values_Contains_ReturnsTrueForExistingValue()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 20 }
        };

        Assert.True(dictionary.Values.Contains(10));
        Assert.True(dictionary.Values.Contains(20));
    }

    [Fact]
    public void Values_Contains_ReturnsFalseForNonExistingValue()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        Assert.False(dictionary.Values.Contains(99));
    }

    [Fact]
    public void Values_Contains_ReturnsTrueForNullValue()
    {
        var dictionary = new ValueListDictionary<string, string>
        {
            { "Key1", null! },
            { "Key1", "test" }
        };

        Assert.True(dictionary.Values.Contains(null!));
    }

    [Fact]
    public void Values_Contains_ReturnsFalseForNullWhenNotPresent()
    {
        var dictionary = new ValueListDictionary<string, string> { { "Key1", "test" } };

        Assert.False(dictionary.Values.Contains(null!));
    }

    [Fact]
    public void Values_CopyTo_CopiesValuesToArray()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key2", 20 }
        };

        var array = new int[2];
        dictionary.Values.CopyTo(array, 0);

        Assert.Equal([10, 20], array);
    }

    [Fact]
    public void Values_CopyTo_CopiesValuesInCorrectOrder()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 20 },
            { "Key2", 30 }
        };

        var array = new int[3];
        dictionary.Values.CopyTo(array, 0);

        Assert.Equal([10, 20, 30], array);
    }

    [Fact]
    public void Values_CopyTo_CopiesValuesToArrayAtIndex()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key2", 20 }
        };

        var array = new int[4];
        dictionary.Values.CopyTo(array, 2);

        Assert.Equal([0, 0, 10, 20], array);
    }

    [Fact]
    public void Values_CopyTo_ThrowsArgumentNullException_WhenArrayIsNull()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        Assert.Throws<ArgumentNullException>(() => dictionary.Values.CopyTo(null!, 0));
    }

    [Fact]
    public void Values_CopyTo_ThrowsArgumentOutOfRangeException_WhenIndexIsNegative()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.Values.CopyTo(new int[1], -1));
    }

    [Fact]
    public void Values_CopyTo_ThrowsArgumentException_WhenArrayIsTooSmall()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key2", 20 }
        };

        Assert.Throws<ArgumentException>(() => dictionary.Values.CopyTo(new int[1], 0));
    }

    [Fact]
    public void Values_GetEnumerator_EnumeratesAllValuesGroupedByKey()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 20 },
            { "Key2", 30 }
        };

        var values = dictionary.Values.ToList();

        Assert.Equal([10, 20, 30], values);
    }

    [Fact]
    public void Values_GetEnumerator_ReturnsEmptyForEmptyDictionary()
    {
        var dictionary = new ValueListDictionary<string, int>();

        var values = dictionary.Values.ToList();

        Assert.Empty(values);
    }

    [Fact]
    public void Values_GetEnumerator_NonGeneric_EnumeratesValues()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key2", 20 }
        };

        var values = new List<int>();
        foreach (var value in (IEnumerable)dictionary.Values)
            values.Add((int)value);

        Assert.Equal([10, 20], values);
    }

    [Fact]
    public void Values_Add_ThrowsNotSupportedException()
    {
        var dictionary = new ValueListDictionary<string, int>();
        ICollection<int> values = dictionary.Values;

        Assert.Throws<NotSupportedException>(() => values.Add(10));
    }

    [Fact]
    public void Values_Clear_ThrowsNotSupportedException()
    {
        var dictionary = new ValueListDictionary<string, int>();
        ICollection<int> values = dictionary.Values;

        Assert.Throws<NotSupportedException>(() => values.Clear());
    }

    [Fact]
    public void Values_Remove_ThrowsNotSupportedException()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };
        ICollection<int> values = dictionary.Values;

        Assert.Throws<NotSupportedException>(() => values.Remove(10));
    }

    [Fact]
    public void Values_InterfaceProperty_ReturnsSameCollectionAsConcreteProperty()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        IReadOnlyValueListDictionary<string, int> readOnlyDict = dictionary;

        Assert.Same(dictionary.Values, readOnlyDict.Values);
    }

    #endregion

    #region ValueCollection.Enumerator Tests

    [Fact]
    public void ValueEnumerator_Current_ReturnsCurrentValue()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        var enumerator = dictionary.Values.GetEnumerator();
        enumerator.MoveNext();

        Assert.Equal(10, enumerator.Current);
    }

    [Fact]
    public void ValueEnumerator_MoveNext_ReturnsFalseWhenEmpty()
    {
        var dictionary = new ValueListDictionary<string, int>();

        var enumerator = dictionary.Values.GetEnumerator();

        Assert.False(enumerator.MoveNext());
    }

    [Fact]
    public void ValueEnumerator_MoveNext_IteratesAllValues()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key1", 20 },
            { "Key2", 30 }
        };

        var enumerator = dictionary.Values.GetEnumerator();
        var values = new List<int>();

        while (enumerator.MoveNext())
            values.Add(enumerator.Current);

        Assert.Equal([10, 20, 30], values);
    }

    [Fact]
    public void ValueEnumerator_MoveNext_ReturnsFalseAfterLastElement()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        var enumerator = dictionary.Values.GetEnumerator();
        enumerator.MoveNext();

        Assert.False(enumerator.MoveNext());
        Assert.False(enumerator.MoveNext());
    }

    [Fact]
    public void ValueEnumerator_Reset_ResetsToBeginning()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key2", 20 }
        };

        var enumerator = dictionary.Values.GetEnumerator();
        enumerator.MoveNext();
        enumerator.MoveNext();
        enumerator.Reset();

        Assert.True(enumerator.MoveNext());
        Assert.Equal(10, enumerator.Current);
    }

    [Fact]
    public void ValueEnumerator_Dispose_CanBeCalledMultipleTimes()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        var enumerator = dictionary.Values.GetEnumerator();
        enumerator.Dispose();
        enumerator.Dispose(); // Should not throw
    }

    [Fact]
    public void ValueEnumerator_NonGenericCurrent_ReturnsBoxedValue()
    {
        var dictionary = new ValueListDictionary<string, int> { { "Key1", 10 } };

        IEnumerator enumerator = dictionary.Values.GetEnumerator();
        enumerator.MoveNext();

        Assert.Equal(10, (int)enumerator.Current!);
    }

    #endregion

    #region Interface Implementation Tests

    [Fact]
    public void IReadOnlyValueListDictionary_Keys_ReturnsICollection()
    {
        IReadOnlyValueListDictionary<string, int> dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 }
        };

        var keys = dictionary.Keys;

        Assert.Single(keys);
        Assert.Contains("Key1", keys);
    }

    [Fact]
    public void IReadOnlyValueListDictionary_Values_ReturnsICollection()
    {
        IReadOnlyValueListDictionary<string, int> dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 }
        };

        var values = dictionary.Values;

        Assert.Single(values);
        Assert.Contains(10, values);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Dictionary_WithValueTypeKey_WorksCorrectly()
    {
        var dictionary = new ValueListDictionary<int, string>
        {
            { 1, "one" },
            { 2, "two" },
            { 1, "uno" }
        };

        Assert.Equal(2, dictionary.KeyCount);
        Assert.Equal(3, dictionary.Count);
        Assert.Equal(["one", "uno"], dictionary.GetValues(1));
    }

    [Fact]
    public void Dictionary_WithReferenceTypeValue_AllowsDuplicateValues()
    {
        var dictionary = new ValueListDictionary<string, string>
        {
            { "Key1", "value" },
            { "Key1", "value" }
        };

        Assert.Equal(2, dictionary.Count);
        Assert.Equal(["value", "value"], dictionary.GetValues("Key1"));
    }

    [Fact]
    public void Dictionary_EmptyAfterClear_CanBeReused()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "Key1", 10 },
            { "Key2", 20 }
        };

        var keys = dictionary.Keys;
        var values = dictionary.Values;

        dictionary.Clear();

        Assert.Equal(0, keys.Count);
        Assert.Equal(0, values.Count);

        dictionary.Add("Key3", 30);

        Assert.Equal(1, keys.Count);
        Assert.Equal(1, values.Count);
        Assert.Contains("Key3", keys);
        Assert.Contains(30, values);
    }

    [Fact]
    public void Dictionary_MultipleIterationsProduceSameResults()
    {
        var dictionary = new ValueListDictionary<string, int>
        {
            { "A", 1 },
            { "B", 2 },
            { "A", 3 }
        };

        var firstIteration = dictionary.ToList();
        var secondIteration = dictionary.ToList();

        Assert.Equal(firstIteration.Count, secondIteration.Count);
        for (var i = 0; i < firstIteration.Count; i++)
        {
            Assert.Equal(firstIteration[i].Key, secondIteration[i].Key);
            Assert.Equal(firstIteration[i].Value, secondIteration[i].Value);
        }
    }

    #endregion
}