using System;
using System.Collections.Generic;
using System.Linq;
using AnakinRaW.CommonUtilities.Collections;
using PG.Commons.Collections;
using Xunit;

namespace PG.Commons.Test.Collections;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

// Class Key + Struct Value

public class ValueListDictionaryTests_ClassKey_StructValue : ValueListDictionaryTestsBase<string, int>
{
    protected override string CreateKey(int seed) => $"Key{seed}";
    protected override int CreateValue(int seed) => seed * 10;
    protected override string CreateKeyNotInDictionary() => "NotFound";
    protected override int CreateValueNotInDictionary() => 999;

    protected override IEqualityComparer<string> GetCaseInsensitiveComparer() => StringComparer.OrdinalIgnoreCase;
    protected override string CreateAlternateKey(string original) => original.ToLower();
}

// Class Key + Class Value
public class ValueListDictionaryTests_ClassKey_ClassValue : ValueListDictionaryTestsBase<string, string>
{
    protected override string CreateKey(int seed) => $"Key{seed}";
    protected override string CreateValue(int seed) => $"Value{seed}";
    protected override string CreateKeyNotInDictionary() => "NotFound";
    protected override string CreateValueNotInDictionary() => "NOT_FOUND";

    protected override IEqualityComparer<string> GetCaseInsensitiveComparer() => StringComparer.OrdinalIgnoreCase;

    protected override string CreateAlternateKey(string original) => original.ToUpper();
}

// Struct Key + Struct Value
public class ValueListDictionaryTests_StructKey_StructValue : ValueListDictionaryTestsBase<int, double>
{
    protected override int CreateKey(int seed) => seed;
    protected override double CreateValue(int seed) => seed * 1.5;
    protected override int CreateKeyNotInDictionary() => 999;
    protected override double CreateValueNotInDictionary() => 999.9;
}

// Struct Key + Class Value
public class ValueListDictionaryTests_StructKey_ClassValue : ValueListDictionaryTestsBase<int, string>
{
    protected override int CreateKey(int seed) => seed;
    protected override string CreateValue(int seed) => $"Value{seed}";
    protected override int CreateKeyNotInDictionary() => 999;
    protected override string CreateValueNotInDictionary() => "NOT_FOUND";
}

/// <summary>
/// Abstract base class for testing ValueListDictionary with different key/value type combinations.
/// </summary>
public abstract class ValueListDictionaryTestsBase<TKey, TValue> where TKey : notnull
{
    protected abstract TKey CreateKey(int seed);
    protected abstract TValue CreateValue(int seed);
    protected abstract TKey CreateKeyNotInDictionary();
    protected abstract TValue CreateValueNotInDictionary();

    protected virtual IEqualityComparer<TKey>? GetCaseInsensitiveComparer() => null;
    protected virtual TKey CreateAlternateKey(TKey original) => original; // Default: same key

    protected virtual bool SupportsCaseInsensitiveComparer => GetCaseInsensitiveComparer() != null;
    protected virtual bool SupportsNullValues => !typeof(TValue).IsValueType;

    #region Constructor Tests

    [Fact]
    public void Constructor_Default_InitializesEmptyDictionary()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();

        Assert.NotNull(dictionary);
        Assert.Empty(dictionary.Keys);
        Assert.Empty(dictionary.Values);
        Assert.Equal(0, dictionary.Count);
        Assert.Equal(0, dictionary.KeyCount);
    }

    [Fact]
    public void Constructor_WithNullComparer_UsesDefaultComparer()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>(null)
        {
            { CreateKey(1), CreateValue(1) }
        };

        Assert.True(dictionary.ContainsKey(CreateKey(1)));
        Assert.False(dictionary.ContainsKey(CreateKey(2)));
    }

    [Fact]
    public void Constructor_WithCustomComparer_UsesProvidedComparer()
    {
        if (!SupportsCaseInsensitiveComparer)
            return;

        var comparer = GetCaseInsensitiveComparer();
        var dictionary = new ValueListDictionary<TKey, TValue>(comparer);

        var key = CreateKey(1);
        var alternateKey = CreateAlternateKey(key);

        dictionary.Add(key, CreateValue(1));
        dictionary.Add(alternateKey, CreateValue(2));

        Assert.Equal(2, dictionary.Count);
        Assert.Equal(1, dictionary.KeyCount);
        Assert.Equal(2, dictionary.GetValues(key).Count);
    }

    #endregion

    #region Count and KeyCount Tests

    [Fact]
    public void Count_ReturnsZero_WhenEmpty()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();
        Assert.Equal(0, dictionary.Count);
    }

    [Fact]
    public void Count_ReturnsTotalNumberOfValues()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { CreateKey(1), CreateValue(1) },
            { CreateKey(1), CreateValue(2) },
            { CreateKey(2), CreateValue(3) }
        };

        Assert.Equal(3, dictionary.Count);
    }

    [Fact]
    public void KeyCount_ReturnsZero_WhenEmpty()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();
        Assert.Equal(0, dictionary.KeyCount);
    }

    [Fact]
    public void KeyCount_ReturnsNumberOfDistinctKeys()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { CreateKey(1), CreateValue(1) },
            { CreateKey(1), CreateValue(2) },
            { CreateKey(2), CreateValue(3) },
            { CreateKey(3), CreateValue(4) }
        };

        Assert.Equal(3, dictionary.KeyCount);
        Assert.Equal(4, dictionary.Count);
    }

    #endregion

    #region Add Tests

    [Fact]
    public void Add_ReturnsFalse_WhenKeyIsNew()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();

        var result = dictionary.Add(CreateKey(1), CreateValue(1));

        Assert.False(result);
        Assert.Equal(1, dictionary.Count);
        Assert.Equal(1, dictionary.KeyCount);
    }

    [Fact]
    public void Add_ReturnsTrue_WhenKeyExists()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();
        var key = CreateKey(1);

        dictionary.Add(key, CreateValue(1));
        var result = dictionary.Add(key, CreateValue(2));

        Assert.True(result);
        Assert.Equal(2, dictionary.Count);
        Assert.Equal(1, dictionary.KeyCount);
    }

    [Fact]
    public void Add_AllowsNullValue_ForReferenceTypes()
    {
        if (!SupportsNullValues)
            return;

        var dictionary = new ValueListDictionary<TKey, TValue> { { CreateKey(1), default! } };

        Assert.Equal(1, dictionary.Count);
        Assert.Equal(default, dictionary.GetFirstValue(CreateKey(1)));
    }

    [Fact]
    public void Add_PreservesInsertionOrder()
    {
        var key1 = CreateKey(1);
        var key2 = CreateKey(2);
        var key3 = CreateKey(3);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { key3, CreateValue(3) },
            { key1, CreateValue(1) },
            { key2, CreateValue(2) }
        };

        var keys = dictionary.Keys.ToList();
        Assert.Equal(new[] { key3, key1, key2 }, keys);
    }

    [Fact]
    public void Add_PreservesValueOrderPerKey()
    {
        var key = CreateKey(1);
        var value1 = CreateValue(1);
        var value2 = CreateValue(2);
        var value3 = CreateValue(3);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { key, value1 },
            { key, value2 },
            { key, value3 }
        };

        var values = dictionary.GetValues(key);
        Assert.Equal([value1, value2, value3], values);
    }

    [Fact]
    public void Add_HandlesLargeNumberOfEntries()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();
        var key = CreateKey(1);
        const int largeCount = 1000;

        for (var i = 0; i < largeCount; i++)
            dictionary.Add(key, CreateValue(i));

        Assert.Equal(largeCount, dictionary.Count);
        Assert.Equal(1, dictionary.KeyCount);
    }

    #endregion

    #region Clear Tests

    [Fact]
    public void Clear_OnEmptyDictionary_DoesNothing()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();

        dictionary.Clear();

        Assert.Equal(0, dictionary.Count);
        Assert.Equal(0, dictionary.KeyCount);
    }

    [Fact]
    public void Clear_RemovesAllKeysAndValues()
    {
        var key1 = CreateKey(1);
        var key2 = CreateKey(2);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { key1, CreateValue(1) },
            { key1, CreateValue(2) },
            { key2, CreateValue(3) }
        };

        dictionary.Clear();

        Assert.Equal(0, dictionary.Count);
        Assert.Equal(0, dictionary.KeyCount);
        Assert.Empty(dictionary.Keys);
        Assert.Empty(dictionary.Values);
        Assert.False(dictionary.ContainsKey(key1));
        Assert.False(dictionary.ContainsKey(key2));
    }

    [Fact]
    public void Clear_AllowsReuseAfterClearing()
    {
        var key1 = CreateKey(1);
        var key2 = CreateKey(2);

        var dictionary = new ValueListDictionary<TKey, TValue> { { key1, CreateValue(1) } };

        dictionary.Clear();
        dictionary.Add(key2, CreateValue(2));

        Assert.Equal(1, dictionary.Count);
        Assert.Equal(1, dictionary.KeyCount);
        Assert.True(dictionary.ContainsKey(key2));
        Assert.False(dictionary.ContainsKey(key1));
    }

    #endregion

    #region ContainsKey Tests

    [Fact]
    public void ContainsKey_ReturnsTrue_WhenKeyExists()
    {
        var key = CreateKey(1);
        var dictionary = new ValueListDictionary<TKey, TValue> { { key, CreateValue(1) } };

        Assert.True(dictionary.ContainsKey(key));
    }

    [Fact]
    public void ContainsKey_ReturnsFalse_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();

        Assert.False(dictionary.ContainsKey(CreateKey(1)));
    }

    #endregion

    #region GetValues Tests

    [Fact]
    public void GetValues_ReturnsAllValues_ForExistingKey()
    {
        var key = CreateKey(1);
        var value1 = CreateValue(1);
        var value2 = CreateValue(2);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { key, value1 },
            { key, value2 }
        };

        var values = dictionary.GetValues(key);

        Assert.Equal(2, values.Count);
        Assert.Equal([value1, value2], values);
    }

    [Fact]
    public void GetValues_ReturnsSingleValue_WhenKeyHasOneValue()
    {
        var key = CreateKey(1);
        var value = CreateValue(1);

        var dictionary = new ValueListDictionary<TKey, TValue> { { key, value } };

        var values = dictionary.GetValues(key);

        Assert.Single(values);
        Assert.Equal(value, values[0]);
    }

    [Fact]
    public void GetValues_ThrowsKeyNotFoundException_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();
        var keyNotFound = CreateKeyNotInDictionary();

        var ex = Assert.Throws<KeyNotFoundException>(() => dictionary.GetValues(keyNotFound));
        Assert.Contains(keyNotFound.ToString()!, ex.Message);
    }

    #endregion

    #region GetFirstValue / GetLastValue Tests

    [Fact]
    public void GetFirstValue_ReturnsFirstValue_WhenKeyHasMultipleValues()
    {
        var key = CreateKey(1);
        var value1 = CreateValue(1);
        var value2 = CreateValue(2);
        var value3 = CreateValue(3);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { key, value1 },
            { key, value2 },
            { key, value3 }
        };

        Assert.Equal(value1, dictionary.GetFirstValue(key));
    }

    [Fact]
    public void GetFirstValue_ReturnsValue_WhenKeyHasSingleValue()
    {
        var key = CreateKey(1);
        var value = CreateValue(1);

        var dictionary = new ValueListDictionary<TKey, TValue> { { key, value } };

        Assert.Equal(value, dictionary.GetFirstValue(key));
    }

    [Fact]
    public void GetFirstValue_ThrowsKeyNotFoundException_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();

        Assert.Throws<KeyNotFoundException>(() => dictionary.GetFirstValue(CreateKeyNotInDictionary()));
    }

    [Fact]
    public void GetLastValue_ReturnsLastValue_WhenKeyHasMultipleValues()
    {
        var key = CreateKey(1);
        var value1 = CreateValue(1);
        var value2 = CreateValue(2);
        var value3 = CreateValue(3);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { key, value1 },
            { key, value2 },
            { key, value3 }
        };

        Assert.Equal(value3, dictionary.GetLastValue(key));
    }

    [Fact]
    public void GetLastValue_ReturnsValue_WhenKeyHasSingleValue()
    {
        var key = CreateKey(1);
        var value = CreateValue(1);

        var dictionary = new ValueListDictionary<TKey, TValue> { { key, value } };

        Assert.Equal(value, dictionary.GetLastValue(key));
    }

    [Fact]
    public void GetLastValue_ThrowsKeyNotFoundException_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();

        Assert.Throws<KeyNotFoundException>(() => dictionary.GetLastValue(CreateKeyNotInDictionary()));
    }

    #endregion

    #region TryGetFirstValue / TryGetLastValue Tests

    [Fact]
    public void TryGetFirstValue_ReturnsTrueAndFirstValue_WhenKeyExists()
    {
        var key = CreateKey(1);
        var value1 = CreateValue(1);
        var value2 = CreateValue(2);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { key, value1 },
            { key, value2 }
        };

        var result = dictionary.TryGetFirstValue(key, out var value);

        Assert.True(result);
        Assert.Equal(value1, value);
    }

    [Fact]
    public void TryGetFirstValue_ReturnsFalseAndDefault_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();

        var result = dictionary.TryGetFirstValue(CreateKeyNotInDictionary(), out var value);

        Assert.False(result);
        Assert.Equal(default, value);
    }

    [Fact]
    public void TryGetLastValue_ReturnsTrueAndLastValue_WhenKeyExists()
    {
        var key = CreateKey(1);
        var value1 = CreateValue(1);
        var value2 = CreateValue(2);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { key, value1 },
            { key, value2 }
        };

        var result = dictionary.TryGetLastValue(key, out var value);

        Assert.True(result);
        Assert.Equal(value2, value);
    }

    [Fact]
    public void TryGetLastValue_ReturnsFalseAndDefault_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();

        var result = dictionary.TryGetLastValue(CreateKeyNotInDictionary(), out var value);

        Assert.False(result);
        Assert.Equal(default, value);
    }

    #endregion

    #region TryGetValues Tests

    [Fact]
    public void TryGetValues_ReturnsTrueAndValues_WhenKeyExists()
    {
        var key = CreateKey(1);
        var value1 = CreateValue(1);
        var value2 = CreateValue(2);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { key, value1 },
            { key, value2 }
        };

        var result = dictionary.TryGetValues(key, out var values);

        Assert.True(result);
        Assert.Equal([value1, value2], values);
    }

    [Fact]
    public void TryGetValues_ReturnsFalseAndEmptyList_WhenKeyDoesNotExist()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();

        var result = dictionary.TryGetValues(CreateKeyNotInDictionary(), out var values);

        Assert.False(result);
        Assert.Empty(values);
    }

    #endregion

    #region Enumerator Tests

    [Fact]
    public void GetEnumerator_ReturnsEmptyEnumerator_WhenDictionaryIsEmpty()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();

        using var enumerator = dictionary.GetEnumerator();

        Assert.False(enumerator.MoveNext());
    }

    [Fact]
    public void GetEnumerator_EnumeratesAllKeyValueGroups()
    {
        var key1 = CreateKey(1);
        var key2 = CreateKey(2);
        var value1 = CreateValue(1);
        var value2 = CreateValue(2);
        var value3 = CreateValue(3);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { key1, value1 },
            { key2, value2 },
            { key2, value3 }
        };

        var pairs = new List<KeyValuePair<TKey, ReadOnlyFrugalList<TValue>>>();
        foreach (var pair in dictionary)
            pairs.Add(pair);

        Assert.Equal(2, pairs.Count);

        Assert.Equal(key1, pairs[0].Key);
        Assert.Equal([value1], pairs[0].Value);

        Assert.Equal(key2, pairs[1].Key);
        Assert.Equal([value2, value3], pairs[1].Value);
    }

    [Fact]
    public void GetEnumerator_PreservesKeyInsertionOrder()
    {
        var key1 = CreateKey(1);
        var key2 = CreateKey(2);
        var key3 = CreateKey(3);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { key3, CreateValue(3) },
            { key1, CreateValue(1) },
            { key2, CreateValue(2) }
        };

        var keys = dictionary.Select(kvp => kvp.Key).ToList();

        Assert.Equal(new[] { key3, key1, key2 }, keys);
    }

    [Fact]
    public void Enumerator_Reset_ResetsToBeginning()
    {
        var key1 = CreateKey(1);
        var key2 = CreateKey(2);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { key1, CreateValue(1) },
            { key2, CreateValue(2) }
        };

        using var enumerator = dictionary.GetEnumerator();
        enumerator.MoveNext();
        enumerator.MoveNext();
        enumerator.Reset();

        Assert.True(enumerator.MoveNext());
        Assert.Equal(key1, enumerator.Current.Key);
    }

    #endregion

    #region KeyCollection Tests

    [Fact]
    public void Keys_ReturnsSameInstance()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();

        var keys1 = dictionary.Keys;
        var keys2 = dictionary.Keys;

        Assert.Same(keys1, keys2);
    }

    [Fact]
    public void Keys_ReflectsChangesToDictionary()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();
        var keys = dictionary.Keys;

#pragma warning disable xUnit2013
        Assert.Equal(0, keys.Count);

        dictionary.Add(CreateKey(1), CreateValue(1));
        Assert.Equal(1, keys.Count);

        dictionary.Add(CreateKey(2), CreateValue(2));
        Assert.Equal(2, keys.Count);

        dictionary.Clear();
        Assert.Equal(0, keys.Count);
#pragma warning restore xUnit2013
    }

    [Fact]
    public void Keys_Contains_ReturnsTrueForExistingKey()
    {
        var key = CreateKey(1);
        var dictionary = new ValueListDictionary<TKey, TValue> { { key, CreateValue(1) } };

#pragma warning disable xUnit2017
        Assert.True(dictionary.Keys.Contains(key));
#pragma warning restore xUnit2017
    }

    [Fact]
    public void Keys_Contains_ReturnsFalseForNonExistingKey()
    {
        var dictionary = new ValueListDictionary<TKey, TValue> { { CreateKey(1), CreateValue(1) } };

#pragma warning disable xUnit2017
        Assert.False(dictionary.Keys.Contains(CreateKey(2)));
#pragma warning restore xUnit2017
    }

    [Fact]
    public void Keys_CopyTo_CopiesKeysToArray()
    {
        var key1 = CreateKey(1);
        var key2 = CreateKey(2);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { key1, CreateValue(1) },
            { key2, CreateValue(2) }
        };

        var array = new TKey[2];
        dictionary.Keys.CopyTo(array, 0);

        Assert.Equal([key1, key2], array);
    }

    [Fact]
    public void Keys_CopyTo_CopiesKeysToArrayAtIndex()
    {
        var key1 = CreateKey(1);
        var key2 = CreateKey(2);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { key1, CreateValue(1) },
            { key2, CreateValue(2) }
        };

        var array = new TKey[4];
        dictionary.Keys.CopyTo(array, 2);

        Assert.Equal(default, array[0]);
        Assert.Equal(default, array[1]);
        Assert.Equal(key1, array[2]);
        Assert.Equal(key2, array[3]);
    }

    [Fact]
    public void Keys_CopyTo_ThrowsArgumentNullException_WhenArrayIsNull()
    {
        var dictionary = new ValueListDictionary<TKey, TValue> { { CreateKey(1), CreateValue(1) } };

        Assert.Throws<ArgumentNullException>(() => dictionary.Keys.CopyTo(null!, 0));
    }

    [Fact]
    public void Keys_CopyTo_ThrowsArgumentOutOfRangeException_WhenIndexIsNegative()
    {
        var dictionary = new ValueListDictionary<TKey, TValue> { { CreateKey(1), CreateValue(1) } };

        Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.Keys.CopyTo(new TKey[1], -1));
    }

    [Fact]
    public void Keys_CopyTo_ThrowsArgumentException_WhenArrayIsTooSmall()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { CreateKey(1), CreateValue(1) },
            { CreateKey(2), CreateValue(2) }
        };

        Assert.Throws<ArgumentException>(() => dictionary.Keys.CopyTo(new TKey[1], 0));
    }

    [Fact]
    public void Keys_GetEnumerator_EnumeratesKeysInInsertionOrder()
    {
        var key1 = CreateKey(1);
        var key2 = CreateKey(2);
        var key3 = CreateKey(3);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { key3, CreateValue(3) },
            { key1, CreateValue(1) },
            { key2, CreateValue(2) }
        };

        var keys = dictionary.Keys.ToList();

        Assert.Equal(new[] { key3, key1, key2 }, keys);
    }

    [Fact]
    public void Keys_Add_ThrowsNotSupportedException()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();
        ICollection<TKey> keys = dictionary.Keys;

        Assert.Throws<NotSupportedException>(() => keys.Add(CreateKey(1)));
    }

    [Fact]
    public void Keys_Clear_ThrowsNotSupportedException()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();
        ICollection<TKey> keys = dictionary.Keys;

        Assert.Throws<NotSupportedException>(() => keys.Clear());
    }

    [Fact]
    public void Keys_Remove_ThrowsNotSupportedException()
    {
        var dictionary = new ValueListDictionary<TKey, TValue> { { CreateKey(1), CreateValue(1) } };
        ICollection<TKey> keys = dictionary.Keys;

        Assert.Throws<NotSupportedException>(() => keys.Remove(CreateKey(1)));
    }

    #endregion

    #region ValueCollection Tests

    [Fact]
    public void Values_ReturnsSameInstance()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();

        var values1 = dictionary.Values;
        var values2 = dictionary.Values;

        Assert.Same(values1, values2);
    }

    [Fact]
    public void Values_ReflectsChangesToDictionary()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();
        var values = dictionary.Values;

#pragma warning disable xUnit2013
        Assert.Equal(0, values.Count);

        dictionary.Add(CreateKey(1), CreateValue(1));
        Assert.Equal(1, values.Count);

        dictionary.Add(CreateKey(1), CreateValue(2));
        Assert.Equal(2, values.Count);

        dictionary.Clear();
        Assert.Equal(0, values.Count);
#pragma warning restore xUnit2013
    }

    [Fact]
    public void Values_Contains_ReturnsTrueForExistingValue()
    {
        var value1 = CreateValue(1);
        var value2 = CreateValue(2);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { CreateKey(1), value1 },
            { CreateKey(1), value2 }
        };

#pragma warning disable xUnit2017
        Assert.True(dictionary.Values.Contains(value1));
        Assert.True(dictionary.Values.Contains(value2));
#pragma warning restore xUnit2017
    }

    [Fact]
    public void Values_Contains_ReturnsFalseForNonExistingValue()
    {
        var dictionary = new ValueListDictionary<TKey, TValue> { { CreateKey(1), CreateValue(1) } };
#pragma warning disable xUnit2017
        Assert.False(dictionary.Values.Contains(CreateValueNotInDictionary()));
#pragma warning restore xUnit2017
    }

    [Fact]
    public void Values_CopyTo_CopiesValuesToArray()
    {
        var value1 = CreateValue(1);
        var value2 = CreateValue(2);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { CreateKey(1), value1 },
            { CreateKey(2), value2 }
        };

        var array = new TValue[2];
        dictionary.Values.CopyTo(array, 0);

        Assert.Equal([value1, value2], array);
    }

    [Fact]
    public void Values_CopyTo_CopiesValuesInCorrectOrder()
    {
        var value1 = CreateValue(1);
        var value2 = CreateValue(2);
        var value3 = CreateValue(3);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { CreateKey(1), value1 },
            { CreateKey(1), value2 },
            { CreateKey(2), value3 }
        };

        var array = new TValue[3];
        dictionary.Values.CopyTo(array, 0);

        Assert.Equal([value1, value2, value3], array);
    }

    [Fact]
    public void Values_CopyTo_CopiesValuesToArrayAtIndex()
    {
        var value1 = CreateValue(1);
        var value2 = CreateValue(2);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { CreateKey(1), value1 },
            { CreateKey(2), value2 }
        };

        var array = new TValue[4];
        dictionary.Values.CopyTo(array, 2);

        Assert.Equal(default, array[0]);
        Assert.Equal(default, array[1]);
        Assert.Equal(value1, array[2]);
        Assert.Equal(value2, array[3]);
    }

    [Fact]
    public void Values_CopyTo_ThrowsArgumentNullException_WhenArrayIsNull()
    {
        var dictionary = new ValueListDictionary<TKey, TValue> { { CreateKey(1), CreateValue(1) } };

        Assert.Throws<ArgumentNullException>(() => dictionary.Values.CopyTo(null!, 0));
    }

    [Fact]
    public void Values_CopyTo_ThrowsArgumentOutOfRangeException_WhenIndexIsNegative()
    {
        var dictionary = new ValueListDictionary<TKey, TValue> { { CreateKey(1), CreateValue(1) } };

        Assert.Throws<ArgumentOutOfRangeException>(() => dictionary.Values.CopyTo(new TValue[1], -1));
    }

    [Fact]
    public void Values_CopyTo_ThrowsArgumentException_WhenArrayIsTooSmall()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { CreateKey(1), CreateValue(1) },
            { CreateKey(2), CreateValue(2) }
        };

        Assert.Throws<ArgumentException>(() => dictionary.Values.CopyTo(new TValue[1], 0));
    }

    [Fact]
    public void Values_GetEnumerator_EnumeratesAllValuesGroupedByKey()
    {
        var value1 = CreateValue(1);
        var value2 = CreateValue(2);
        var value3 = CreateValue(3);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { CreateKey(1), value1 },
            { CreateKey(1), value2 },
            { CreateKey(2), value3 }
        };

        var values = dictionary.Values.ToList();

        Assert.Equal(new[] { value1, value2, value3 }, values);
    }

    [Fact]
    public void Values_Add_ThrowsNotSupportedException()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();
        ICollection<TValue> values = dictionary.Values;

        Assert.Throws<NotSupportedException>(() => values.Add(CreateValue(1)));
    }

    [Fact]
    public void Values_Clear_ThrowsNotSupportedException()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();
        ICollection<TValue> values = dictionary.Values;

        Assert.Throws<NotSupportedException>(() => values.Clear());
    }

    [Fact]
    public void Values_Remove_ThrowsNotSupportedException()
    {
        var dictionary = new ValueListDictionary<TKey, TValue> { { CreateKey(1), CreateValue(1) } };
        ICollection<TValue> values = dictionary.Values;

        Assert.Throws<NotSupportedException>(() => values.Remove(CreateValue(1)));
    }

    #endregion

    #region ValueCollection.Enumerator Tests

    [Fact]
    public void ValueEnumerator_MoveNext_ReturnsFalseWhenEmpty()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>();

        using var enumerator = dictionary.Values.GetEnumerator();

        Assert.False(enumerator.MoveNext());
    }

    [Fact]
    public void ValueEnumerator_MoveNext_IteratesAllValues()
    {
        var value1 = CreateValue(1);
        var value2 = CreateValue(2);
        var value3 = CreateValue(3);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { CreateKey(1), value1 },
            { CreateKey(1), value2 },
            { CreateKey(2), value3 }
        };

        using var enumerator = dictionary.Values.GetEnumerator();
        var values = new List<TValue>();

        while (enumerator.MoveNext())
            values.Add(enumerator.Current);

        Assert.Equal(new[] { value1, value2, value3 }, values);
    }

    [Fact]
    public void ValueEnumerator_Reset_ResetsToBeginning()
    {
        var value1 = CreateValue(1);
        var value2 = CreateValue(2);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { CreateKey(1), value1 },
            { CreateKey(2), value2 }
        };

        using var enumerator = dictionary.Values.GetEnumerator();
        enumerator.MoveNext();
        enumerator.MoveNext();
        enumerator.Reset();

        Assert.True(enumerator.MoveNext());
        Assert.Equal(value1, enumerator.Current);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Dictionary_EmptyAfterClear_CanBeReused()
    {
        var key1 = CreateKey(1);
        var key2 = CreateKey(2);
        var key3 = CreateKey(3);

        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { key1, CreateValue(1) },
            { key2, CreateValue(2) }
        };

        var keys = dictionary.Keys;
        var values = dictionary.Values;

        dictionary.Clear();

#pragma warning disable xUnit2013
        Assert.Equal(0, keys.Count);
        Assert.Equal(0, values.Count);

        dictionary.Add(key3, CreateValue(3));

        Assert.Equal(1, keys.Count);
        Assert.Equal(1, values.Count);
        Assert.Contains(key3, keys);
#pragma warning restore xUnit2013
    }

    [Fact]
    public void Dictionary_MultipleIterationsProduceSameResults()
    {
        var dictionary = new ValueListDictionary<TKey, TValue>
        {
            { CreateKey(1), CreateValue(1) },
            { CreateKey(2), CreateValue(2) },
            { CreateKey(1), CreateValue(3) }
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