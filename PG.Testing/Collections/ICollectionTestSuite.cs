using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PG.Testing.Collections;

// This test suite is taken from the .NET runtime repository (https://github.com/dotnet/runtime) and adapted to the VSTesting Framework.
// The .NET Foundation licenses this under the MIT license.
/// <summary>
/// Contains tests that ensure the correctness of any class that implements the generic
/// <see cref="ICollection{T}"/> interface
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class ICollectionTestSuite<T> : IEnumerableTestSuite<T>
{
    protected virtual Type ICollection_Generic_CopyTo_IndexLargerThanArrayCount_ThrowType => typeof(ArgumentException);

    protected virtual IEnumerable<T> InvalidValues => Array.Empty<T>();

    protected virtual bool DefaultValueAllowed => true;

    protected virtual bool AddRemoveClear_ThrowsNotSupported => false;

    protected virtual bool DuplicateValuesAllowed => true;

    protected virtual bool IsReadOnly => false;

    protected virtual bool IsReadOnly_ValidityValue => IsReadOnly;

    protected virtual bool DefaultValueWhenNotAllowed_Throws => true;

    /// <summary>
    /// Creates an instance of an <see cref="ICollection{T}"/> that can be used for testing.
    /// </summary>
    /// <returns>An instance of an <see cref="ICollection{T}"/> that can be used for testing.</returns>
    protected abstract ICollection<T> GenericICollectionFactory();

    /// <summary>
    /// Returns a set of ModifyEnumerable delegates that modify the enumerable passed to them.
    /// </summary>
    protected override IEnumerable<ModifyEnumerable> GetModifyEnumerables(ModifyOperation operations)
    {
        if (!AddRemoveClear_ThrowsNotSupported && operations.HasFlag(ModifyOperation.Add))
        {
            yield return enumerable =>
            {
                var casted = (ICollection<T>)enumerable;
                casted.Add(CreateT(2344));
                return true;
            };
        }
        if (!AddRemoveClear_ThrowsNotSupported && operations.HasFlag(ModifyOperation.Remove))
        {
            yield return enumerable =>
            {
                var casted = (ICollection<T>)enumerable;
                if (casted.Count > 0)
                {
                    casted.Remove(casted.ElementAt(0));
                    return true;
                }
                return false;
            };
        }
        if (!AddRemoveClear_ThrowsNotSupported && operations.HasFlag(ModifyOperation.Clear))
        {
            yield return enumerable =>
            {
                var casted = (ICollection<T>)enumerable;
                if (casted.Count > 0)
                {
                    casted.Clear();
                    return true;
                }
                return false;
            };
        }
    }

    protected override IEnumerable<T> GenericIEnumerableFactory(int count)
    {
        return GenericICollectionFactory(count);
    }

    /// <summary>
    /// Creates an instance of an <see cref="ICollection{T}"/> that can be used for testing.
    /// </summary>
    /// <param name="count">The number of unique items that the returned <see cref="ICollection{T}"/> contains.</param>
    /// <returns>An instance of an <see cref="ICollection{T}"/> that can be used for testing.</returns>
    protected virtual ICollection<T> GenericICollectionFactory(int count)
    {
        var collection = GenericICollectionFactory();
        AddToCollection(collection, count);
        return collection;
    }

    protected virtual void AddToCollection(ICollection<T> collection, int numberOfItemsToAdd)
    {
        var seed = 9600;
        var comparer = GetIEqualityComparer();
        while (collection.Count < numberOfItemsToAdd)
        {
            var toAdd = CreateT(seed++);
            while (collection.Contains(toAdd, comparer) || InvalidValues.Contains(toAdd, comparer))
                toAdd = CreateT(seed++);
            collection.Add(toAdd);
        }
    }

    #region IsReadOnly

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_IsReadOnly_Validity(int count)
    {
        var collection = GenericICollectionFactory(count);
        Assert.AreEqual(IsReadOnly_ValidityValue, collection.IsReadOnly);
    }

    #endregion

    #region Count

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Count_Validity(int count)
    {
        var collection = GenericICollectionFactory(count);
        Assert.AreEqual(count, collection.Count);
    }

    #endregion

    #region Add

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public virtual void ICollection_Generic_Add_DefaultValue(int count)
    {
        if (DefaultValueAllowed && !IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var collection = GenericICollectionFactory(count);
            collection.Add(default!);
            Assert.AreEqual(count + 1, collection.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Add_InvalidValueToMiddleOfCollection(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            foreach (var invalidValue in InvalidValues)
            {
                var collection = GenericICollectionFactory(count);
                collection.Add(invalidValue);
                for (var i = 0; i < count; i++)
                    collection.Add(CreateT(i));
                Assert.AreEqual(count * 2, collection.Count);
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Add_InvalidValueToBeginningOfCollection(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            foreach (var invalidValue in InvalidValues)
            {
                var collection = GenericICollectionFactory(0);
                collection.Add(invalidValue);
                for (var i = 0; i < count; i++)
                    collection.Add(CreateT(i));
                Assert.AreEqual(count, collection.Count);
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Add_InvalidValueToEndOfCollection(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            foreach (var invalidValue in InvalidValues)
            {
                var collection = GenericICollectionFactory(count);
                collection.Add(invalidValue);
                Assert.AreEqual(count, collection.Count);
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Add_DuplicateValue(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported && DuplicateValuesAllowed)
        {
            var collection = GenericICollectionFactory(count);
            var duplicateValue = CreateT(700);
            collection.Add(duplicateValue);
            collection.Add(duplicateValue);
            Assert.AreEqual(count + 2, collection.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Add_AfterCallingClear(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var collection = GenericICollectionFactory(count);
            collection.Clear();
            AddToCollection(collection, 5);
            Assert.AreEqual(5, collection.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Add_AfterRemovingAnyValue(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var seed = 840;
            var collection = GenericICollectionFactory(count);
            var items = collection.ToList();
            var toAdd = CreateT(seed++);
            while (collection.Contains(toAdd))
                toAdd = CreateT(seed++);
            collection.Add(toAdd);
            collection.Remove(toAdd);

            toAdd = CreateT(seed++);
            while (collection.Contains(toAdd))
                toAdd = CreateT(seed++);

            collection.Add(toAdd);
            items.Add(toAdd);
            CollectionAssertExtensions.EqualUnordered(items, collection);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Add_AfterRemovingAllItems(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var collection = GenericICollectionFactory(count);
            for (var i = 0; i < count; i++)
                collection.Remove(collection.ElementAt(0));
            collection.Add(CreateT(254));
            Assert.AreEqual(1, collection.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Add_ToReadOnlyFrugalList(int count)
    {
        if (IsReadOnly || AddRemoveClear_ThrowsNotSupported)
        {
            var collection = GenericICollectionFactory(count);
            Assert.ThrowsException<NotSupportedException>(() => collection.Add(CreateT(0)));
            Assert.AreEqual(count, collection.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Add_AfterRemoving(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var seed = 840;
            var collection = GenericICollectionFactory(count);
            var toAdd = CreateT(seed++);
            while (collection.Contains(toAdd))
                toAdd = CreateT(seed++);
            collection.Add(toAdd);
            collection.Remove(toAdd);
            collection.Add(toAdd);
        }
    }

    #endregion

    #region Clear

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Clear(int count)
    {
        var collection = GenericICollectionFactory(count);
        if (IsReadOnly || AddRemoveClear_ThrowsNotSupported)
        {
            Assert.ThrowsException<NotSupportedException>(() => collection.Clear());
            Assert.AreEqual(count, collection.Count);
        }
        else
        {
            collection.Clear();
            Assert.AreEqual(0, collection.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Clear_Repeatedly(int count)
    {
        var collection = GenericICollectionFactory(count);
        if (IsReadOnly || AddRemoveClear_ThrowsNotSupported)
        {
            Assert.ThrowsException<NotSupportedException>(() => collection.Clear());
            Assert.ThrowsException<NotSupportedException>(() => collection.Clear());
            Assert.ThrowsException<NotSupportedException>(() => collection.Clear());
            Assert.AreEqual(count, collection.Count);
        }
        else
        {
            collection.Clear();
            collection.Clear();
            collection.Clear();
            Assert.AreEqual(0, collection.Count);
        }
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void ICollection_Generic_Remove_ReferenceRemovedFromCollection(bool useRemove)
    {
        var isOnMono = Type.GetType("Mono.RuntimeStructs") != null;
        if (isOnMono || typeof(T).IsValueType || IsReadOnly || AddRemoveClear_ThrowsNotSupported)
            return;

        var collection = GenericICollectionFactory();

        var wr = PopulateAndRemove(collection, useRemove);
        Assert.IsTrue(SpinWait.SpinUntil(() =>
        {
            GC.Collect();
            return !wr.TryGetTarget(out _);
        }, 30_000));
        GC.KeepAlive(collection);

        [MethodImpl(MethodImplOptions.NoInlining)]
        WeakReference<object> PopulateAndRemove(ICollection<T> collection, bool useRemove)
        {
            AddToCollection(collection, 1);
            var value = collection.First();

            if (useRemove)
                Assert.IsTrue(collection.Remove(value));
            else
            {
                collection.Clear();
                Assert.AreEqual(0, collection.Count);
            }

            return new WeakReference<object>(value!);
        }
    }

    #endregion

    #region Contains

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Contains_ValidValueOnCollectionNotContainingThatValue(int count)
    {
        var collection = GenericICollectionFactory(count);
        var seed = 4315;
        var item = CreateT(seed++);
        while (collection.Contains(item))
            item = CreateT(seed++);
        Assert.IsFalse(collection.Contains(item));
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Contains_ValidValueOnCollectionContainingThatValue(int count)
    {
        var collection = GenericICollectionFactory(count);
        foreach (var item in collection)
            Assert.IsTrue(collection.Contains(item));
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Contains_DefaultValueOnCollectionNotContainingDefaultValue(int count)
    {
        var collection = GenericICollectionFactory(count);
        if (DefaultValueAllowed && default(T) is null) // it's true only for reference types and for Nullable<T>
        {
            Assert.IsFalse(collection.Contains(default!));
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public virtual void ICollection_Generic_Contains_DefaultValueOnCollectionContainingDefaultValue(int count)
    {
        if (DefaultValueAllowed && !IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var collection = GenericICollectionFactory(count);
            collection.Add(default!);
            Assert.IsTrue(collection.Contains(default!));
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Contains_ValidValueThatExistsTwiceInTheCollection(int count)
    {
        if (DuplicateValuesAllowed && !IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var collection = GenericICollectionFactory(count);
            var item = CreateT(12);
            collection.Add(item);
            collection.Add(item);
            Assert.AreEqual(count + 2, collection.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Contains_InvalidValue_ThrowsArgumentException(int count)
    {
        var collection = GenericICollectionFactory(count);
        foreach (var value in InvalidValues)
        {
            Assert.ThrowsException<ArgumentException>(() => collection.Contains(value));
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public virtual void ICollection_Generic_Contains_DefaultValueWhenNotAllowed(int count)
    {
        if (!DefaultValueAllowed && !IsReadOnly)
        {
            var collection = GenericICollectionFactory(count);
            if (DefaultValueWhenNotAllowed_Throws)
                Assert.ThrowsException<ArgumentNullException>(() => collection.Contains(default!));
            else
                Assert.IsFalse(collection.Contains(default!));
        }
    }

    #endregion

    #region CopyTo

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_CopyTo_NullArray_ThrowsArgumentNullException(int count)
    {
        var collection = GenericICollectionFactory(count);
        Assert.ThrowsException<ArgumentNullException>(() => collection.CopyTo(null!, 0));
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_CopyTo_NegativeIndex_ThrowsArgumentOutOfRangeException(int count)
    {
        var collection = GenericICollectionFactory(count);
        var array = new T[count];
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => collection.CopyTo(array, -1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => collection.CopyTo(array, int.MinValue));
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_CopyTo_IndexEqualToArrayCount_ThrowsArgumentException(int count)
    {
        var collection = GenericICollectionFactory(count);
        var array = new T[count];
        if (count > 0)
            Assert.ThrowsException<ArgumentException>(() => collection.CopyTo(array, count));
        else
            collection.CopyTo(array, count); // does nothing since the array is empty
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_CopyTo_IndexLargerThanArrayCount_ThrowsAnyArgumentException(int count)
    {
        var collection = GenericICollectionFactory(count);
        var array = new T[count];
        ExceptionUtilities.AssertThrowsException(ICollection_Generic_CopyTo_IndexLargerThanArrayCount_ThrowType, () => collection.CopyTo(array, count + 1));
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_CopyTo_NotEnoughSpaceInOffsettedArray_ThrowsArgumentException(int count)
    {
        if (count > 0) // Want the T array to have at least 1 element
        {
            var collection = GenericICollectionFactory(count);
            var array = new T[count];
            Assert.ThrowsException<ArgumentException>(() => collection.CopyTo(array, 1));
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_CopyTo_ExactlyEnoughSpaceInArray(int count)
    {
        var collection = GenericICollectionFactory(count);
        var array = new T[count];
        collection.CopyTo(array, 0);
        Assert.IsTrue(collection.SequenceEqual(array));
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_CopyTo_ArrayIsLargerThanCollection(int count)
    {
        var collection = GenericICollectionFactory(count);
        var array = new T[count * 3 / 2];
        collection.CopyTo(array, 0);
        Assert.IsTrue(collection.SequenceEqual(array.Take(count)));
    }

    #endregion

    #region Remove

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Remove_OnReadOnlyFrugalList_ThrowsNotSupportedException(int count)
    {
        if (IsReadOnly || AddRemoveClear_ThrowsNotSupported)
        {
            var collection = GenericICollectionFactory(count);
            Assert.ThrowsException<NotSupportedException>(() => collection.Remove(CreateT(34543)));
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Remove_DefaultValueNotContainedInCollection(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported && DefaultValueAllowed && !InvalidValues.Contains(default))
        {
            var collection = GenericICollectionFactory(count);
            var value = default(T);
            while (collection.Contains(value!))
            {
                collection.Remove(value!);
                count--;
            }
            Assert.IsFalse(collection.Remove(value!));
            Assert.AreEqual(count, collection.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Remove_NonDefaultValueNotContainedInCollection(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var seed = count * 251;
            var collection = GenericICollectionFactory(count);
            var value = CreateT(seed++);
            while (collection.Contains(value) || InvalidValues.Contains(value))
                value = CreateT(seed++);
            Assert.IsFalse(collection.Remove(value));
            Assert.AreEqual(count, collection.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public virtual void ICollection_Generic_Remove_DefaultValueContainedInCollection(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported && DefaultValueAllowed && !InvalidValues.Contains(default))
        {
            var collection = GenericICollectionFactory(count);
            var value = default(T);
            if (!collection.Contains(value!))
            {
                collection.Add(value!);
                count++;
            }
            Assert.IsTrue(collection.Remove(value!));
            Assert.AreEqual(count - 1, collection.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Remove_NonDefaultValueContainedInCollection(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var seed = count * 251;
            var collection = GenericICollectionFactory(count);
            var value = CreateT(seed++);
            if (!collection.Contains(value))
            {
                collection.Add(value);
                count++;
            }
            Assert.IsTrue(collection.Remove(value));
            Assert.AreEqual(count - 1, collection.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Remove_ValueThatExistsTwiceInCollection(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported && DuplicateValuesAllowed)
        {
            var seed = count * 90;
            var collection = GenericICollectionFactory(count);
            var value = CreateT(seed++);
            collection.Add(value);
            collection.Add(value);
            count += 2;
            Assert.IsTrue(collection.Remove(value));
            Assert.IsTrue(collection.Contains(value));
            Assert.AreEqual(count - 1, collection.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Remove_EveryValue(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var collection = GenericICollectionFactory(count);
            foreach (var value in collection.ToList()) 
                Assert.IsTrue(collection.Remove(value));
            Assert.IsFalse(collection.Any());
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Remove_InvalidValue_ThrowsArgumentException(int count)
    {
        var collection = GenericICollectionFactory(count);
        foreach (var value in InvalidValues) 
            Assert.ThrowsException<ArgumentException>(() => collection.Remove(value));
        Assert.AreEqual(count, collection.Count);
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Remove_DefaultValueWhenNotAllowed(int count)
    {
        if (!DefaultValueAllowed && !IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var collection = GenericICollectionFactory(count);
            if (DefaultValueWhenNotAllowed_Throws)
                Assert.ThrowsException<ArgumentNullException>(() => collection.Remove(default!));
            else
                Assert.IsFalse(collection.Remove(default!));
        }
    }

    #endregion
}