using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PG.Testing.Collections;

// This test suite is taken from the .NET runtime repository (https://github.com/dotnet/runtime) and adapted to the VSTesting Framework.
// The .NET Foundation licenses this under the MIT license.
[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
public abstract class IListTestSuite<T> : ICollectionTestSuite<T> 
{
    protected virtual Type IList_Generic_Item_InvalidIndex_ThrowType => typeof(ArgumentOutOfRangeException);

    protected abstract IList<T> GenericIListFactory();

    protected virtual IList<T> GenericIListFactory(int count)
    {
        var collection = GenericIListFactory();
        AddToCollection(collection, count);
        return collection;
    }

    public static IEnumerable<ModifyEnumerable> GetModifyEnumerables(ModifyOperation operations, Func<int, T> createT)
    {
        return new ModifyEnumerableList(createT).GetModifyEnumerables(operations);
    }

    private class ModifyEnumerableList : IListTestSuite<T>
    {
        private readonly Func<int, T> _createT;

        public ModifyEnumerableList(Func<int, T> createT)
        {
            _createT = createT;
        }

        protected override T CreateT(int seed)
        {
            return _createT(seed);
        }

        protected override IList<T> GenericIListFactory()
        {
            throw new NotImplementedException();
        }
    }


    protected override IEnumerable<ModifyEnumerable> GetModifyEnumerables(ModifyOperation operations)
    {
        foreach (var item in base.GetModifyEnumerables(operations))
            yield return item;

        if (!AddRemoveClear_ThrowsNotSupported && operations.HasFlag(ModifyOperation.Insert))
        {
            yield return enumerable =>
            {
                var casted = (IList<T>)enumerable;
                if (casted.Count > 0)
                {
                    casted.Insert(0, CreateT(12));
                    return true;
                }
                return false;
            };
        }
        if (!AddRemoveClear_ThrowsNotSupported && operations.HasFlag(ModifyOperation.Overwrite))
        {
            yield return enumerable =>
            {
                var casted = (IList<T>)enumerable;
                if (casted.Count > 0)
                {
                    casted[0] = CreateT(12);
                    return true;
                }
                return false;
            };
        }
        if (!AddRemoveClear_ThrowsNotSupported && operations.HasFlag(ModifyOperation.Remove))
        {
            yield return enumerable =>
            {
                var casted = (IList<T>)enumerable;
                if (casted.Count > 0)
                {
                    casted.RemoveAt(0);
                    return true;
                }
                return false;
            };
        }
    }

    protected override ICollection<T> GenericICollectionFactory()
    {
        return GenericIListFactory();
    }

    protected override ICollection<T> GenericICollectionFactory(int count)
    {
        return GenericIListFactory(count);
    }


    #region Item Getter

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_ItemGet_NegativeIndex_ThrowsException(int count)
    {
        var list = GenericIListFactory(count);
        ExceptionUtilities.AssertThrowsException(IList_Generic_Item_InvalidIndex_ThrowType, () => list[-1]);
        ExceptionUtilities.AssertThrowsException(IList_Generic_Item_InvalidIndex_ThrowType, () => list[int.MinValue]);
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_ItemGet_IndexGreaterThanListCount_ThrowsException(int count)
    {
        var list = GenericIListFactory(count);
        ExceptionUtilities.AssertThrowsException(IList_Generic_Item_InvalidIndex_ThrowType, () => list[count]);
        ExceptionUtilities.AssertThrowsException(IList_Generic_Item_InvalidIndex_ThrowType, () => list[count + 1]);
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_ItemGet_ValidGetWithinListBounds(int count)
    {
        var list = GenericIListFactory(count);

        foreach (var i in Enumerable.Range(0, count)) 
            Sink(list[i]);
        return;

        [MethodImpl(MethodImplOptions.NoInlining)]
        void Sink(T t) { }
    }

    #endregion

    #region Item Setter

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_ItemSet_NegativeIndex_ThrowsException(int count)
    {
        if (!IsReadOnly)
        {
            var list = GenericIListFactory(count);
            var validAdd = CreateT(0);
            ExceptionUtilities.AssertThrowsException(IList_Generic_Item_InvalidIndex_ThrowType, () => list[-1] = validAdd);
            ExceptionUtilities.AssertThrowsException(IList_Generic_Item_InvalidIndex_ThrowType, () => list[int.MinValue] = validAdd);
            Assert.AreEqual(count, list.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_ItemSet_IndexGreaterThanListCount_ThrowsException(int count)
    {
        if (!IsReadOnly)
        {
            var list = GenericIListFactory(count);
            var validAdd = CreateT(0);
            ExceptionUtilities.AssertThrowsException(IList_Generic_Item_InvalidIndex_ThrowType, () => list[count] = validAdd);
            ExceptionUtilities.AssertThrowsException(IList_Generic_Item_InvalidIndex_ThrowType, () => list[count + 1] = validAdd);
            Assert.AreEqual(count, list.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_ItemSet_OnReadOnlyList(int count)
    {
        if (IsReadOnly && count > 0)
        {
            var list = GenericIListFactory(count);
            var before = list[count / 2];
            Assert.ThrowsException<NotSupportedException>(() => list[count / 2] = CreateT(321432));
            Assert.AreEqual(before, list[count / 2]);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_ItemSet_FirstItemToNonDefaultValue(int count)
    {
        if (count > 0 && !IsReadOnly)
        {
            var list = GenericIListFactory(count);
            var value = CreateT(123452);
            list[0] = value;
            Assert.AreEqual(value, list[0]);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_ItemSet_FirstItemToDefaultValue(int count)
    {
        if (count > 0 && !IsReadOnly)
        {
            var list = GenericIListFactory(count);
            if (DefaultValueAllowed)
            {
                list[0] = default!;
                Assert.AreEqual(default, list[0]);
            }
            else
            {
                Assert.ThrowsException<ArgumentNullException>(() => list[0] = default!);
                Assert.AreNotEqual(default, list[0]);
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_ItemSet_LastItemToNonDefaultValue(int count)
    {
        if (count > 0 && !IsReadOnly)
        {
            var list = GenericIListFactory(count);
            var value = CreateT(123452);
            var lastIndex = count - 1;
            list[lastIndex] = value;
            Assert.AreEqual(value, list[lastIndex]);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_ItemSet_LastItemToDefaultValue(int count)
    {
        if (count > 0 && !IsReadOnly && DefaultValueAllowed)
        {
            var list = GenericIListFactory(count);
            var lastIndex = count - 1;
            if (DefaultValueAllowed)
            {
                list[lastIndex] = default!;
                Assert.AreEqual(default, list[lastIndex]);
            }
            else
            {
                Assert.ThrowsException<ArgumentNullException>(() => list[lastIndex] = default!);
                Assert.AreNotEqual(default, list[lastIndex]);
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_ItemSet_DuplicateValues(int count)
    {
        if (count >= 2 && !IsReadOnly && DuplicateValuesAllowed)
        {
            var list = GenericIListFactory(count);
            var value = CreateT(123452);
            list[0] = value;
            list[1] = value;
            Assert.AreEqual(value, list[0]);
            Assert.AreEqual(value, list[1]);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_ItemSet_InvalidValue(int count)
    {
        if (count > 0 && !IsReadOnly)
        {

            foreach (var invalidValue in InvalidValues)
            {
                var list = GenericIListFactory(count);
                Assert.ThrowsException<ArgumentException>(() => list[count / 2] = invalidValue);
            }
        }
    }

    #endregion

    #region IndexOf

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_IndexOf_DefaultValueNotContainedInList(int count)
    {
        if (DefaultValueAllowed)
        {
            var list = GenericIListFactory(count);
            var value = default(T);
            if (list.Contains(value!))
            {
                if (IsReadOnly)
                    return;
                list.Remove(value!);
            }
            Assert.AreEqual(-1, list.IndexOf(value!));
        }
        else
        {
            var list = GenericIListFactory(count);
            Assert.ThrowsException<ArgumentNullException>(() => list.IndexOf(default!));
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_IndexOf_DefaultValueContainedInList(int count)
    {
        if (count > 0 && DefaultValueAllowed)
        {
            var list = GenericIListFactory(count);
            var value = default(T);
            if (!list.Contains(value!))
            {
                if (IsReadOnly)
                    return;
                list[0] = value!;
            }
            Assert.AreEqual(0, list.IndexOf(value!));
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_IndexOf_ValidValueNotContainedInList(int count)
    {
        var list = GenericIListFactory(count);
        var seed = 54321;
        var value = CreateT(seed++);
        while (list.Contains(value))
            value = CreateT(seed++);
        Assert.AreEqual(-1, list.IndexOf(value));
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_IndexOf_ValueInCollectionMultipleTimes(int count)
    {
        if (count > 0 && !IsReadOnly && DuplicateValuesAllowed)
        {
            // IndexOf should always return the lowest index for which a matching element is found
            var list = GenericIListFactory(count);
            var value = CreateT(12345);
            list[0] = value;
            list[count / 2] = value;
            Assert.AreEqual(0, list.IndexOf(value));
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_IndexOf_EachValueNoDuplicates(int count)
    {
        // Assumes no duplicate elements contained in the list returned by GenericIListFactory
        var list = GenericIListFactory(count);
        foreach (var i in Enumerable.Range(0, count))
        {
            Assert.AreEqual(i, list.IndexOf(list[i]));
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_IndexOf_InvalidValue(int count)
    {
        if (!IsReadOnly)
        {
            foreach (var value in InvalidValues)
            {
                var list = GenericIListFactory(count);
                Assert.ThrowsException<ArgumentException>(() => list.IndexOf(value));
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_IndexOf_ReturnsFirstMatchingValue(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var list = GenericIListFactory(count);
            foreach (var duplicate in list.ToList()) // hard copies list to circumvent enumeration error
                list.Add(duplicate);
            var expectedList = list.ToList();

            foreach (var i in Enumerable.Range(0, count))
            {
                Assert.AreEqual(i, list.IndexOf(expectedList[i]));
            }
        }
    }

    #endregion

    #region Insert

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_Insert_NegativeIndex_ThrowsArgumentOutOfRangeException(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var list = GenericIListFactory(count);
            var validAdd = CreateT(0);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.Insert(-1, validAdd));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.Insert(int.MinValue, validAdd));
            Assert.AreEqual(count, list.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_Insert_IndexGreaterThanListCount_Appends(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var list = GenericIListFactory(count);
            var validAdd = CreateT(12350);
            list.Insert(count, validAdd);
            Assert.AreEqual(count + 1, list.Count);
            Assert.AreEqual(validAdd, list[count]);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_Insert_ToReadOnlyList(int count)
    {
        if (IsReadOnly || AddRemoveClear_ThrowsNotSupported)
        {
            var list = GenericIListFactory(count);
            Assert.ThrowsException<NotSupportedException>(() => list.Insert(count / 2, CreateT(321432)));
            Assert.AreEqual(count, list.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_Insert_FirstItemToNonDefaultValue(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var list = GenericIListFactory(count);
            var value = CreateT(123452);
            list.Insert(0, value);
            Assert.AreEqual(value, list[0]);
            Assert.AreEqual(count + 1, list.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_Insert_FirstItemToDefaultValue(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported && DefaultValueAllowed)
        {
            var list = GenericIListFactory(count);
            var value = default(T);
            list.Insert(0, value!);
            Assert.AreEqual(value, list[0]);
            Assert.AreEqual(count + 1, list.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_Insert_LastItemToNonDefaultValue(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var list = GenericIListFactory(count);
            var value = CreateT(123452);
            var lastIndex = count > 0 ? count - 1 : 0;
            list.Insert(lastIndex, value);
            Assert.AreEqual(value, list[lastIndex]);
            Assert.AreEqual(count + 1, list.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_Insert_LastItemToDefaultValue(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported && DefaultValueAllowed)
        {
            var list = GenericIListFactory(count);
            var value = default(T);
            var lastIndex = count > 0 ? count - 1 : 0;
            list.Insert(lastIndex, value!);
            Assert.AreEqual(value, list[lastIndex]);
            Assert.AreEqual(count + 1, list.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_Insert_DuplicateValues(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported && DuplicateValuesAllowed)
        {
            var list = GenericIListFactory(count);
            var value = CreateT(123452);
            if (AddRemoveClear_ThrowsNotSupported)
            {
                Assert.ThrowsException<NotSupportedException>(() => list.Insert(0, value));
            }
            else
            {
                list.Insert(0, value);
                list.Insert(1, value);
                Assert.AreEqual(value, list[0]);
                Assert.AreEqual(value, list[1]);
                Assert.AreEqual(count + 2, list.Count);
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_Insert_InvalidValue(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            foreach (var value in InvalidValues)
            {
                var list = GenericIListFactory(count);
                Assert.ThrowsException<ArgumentException>(() => list.Insert(count / 2, value));
            }
        }
    }

    #endregion

    #region RemoveAt

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_RemoveAt_NegativeIndex_ThrowsArgumentOutOfRangeException(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var list = GenericIListFactory(count);
            _ = CreateT(0);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.RemoveAt(-1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.RemoveAt(int.MinValue));
            Assert.AreEqual(count, list.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_RemoveAt_IndexGreaterThanListCount_ThrowsArgumentOutOfRangeException(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var list = GenericIListFactory(count);
            _ = CreateT(0);
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.RemoveAt(count));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => list.RemoveAt(count + 1));
            Assert.AreEqual(count, list.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_RemoveAt_OnReadOnlyList(int count)
    {
        if (IsReadOnly || AddRemoveClear_ThrowsNotSupported)
        {
            var list = GenericIListFactory(count);
            Assert.ThrowsException<NotSupportedException>(() => list.RemoveAt(count / 2));
            Assert.AreEqual(count, list.Count);
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_RemoveAt_AllValidIndices(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var list = GenericIListFactory(count);
            Assert.AreEqual(count, list.Count);
            foreach (var i in Enumerable.Range(0, count).Reverse())
            {
                list.RemoveAt(i);
                Assert.AreEqual(i, list.Count);
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_RemoveAt_ZeroMultipleTimes(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var list = GenericIListFactory(count);
            foreach (var i in Enumerable.Range(0, count))
            {
                list.RemoveAt(0);
                Assert.AreEqual(count - i - 1, list.Count);
            }
        }
    }

    #endregion

    #region Enumerator.Current

    // Test Enumerator.Current at end after new elements was added
    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_CurrentAtEnd_AfterAdd(int count)
    {
        if (!IsReadOnly && !AddRemoveClear_ThrowsNotSupported)
        {
            var collection = GenericIListFactory(count);

            using IEnumerator<T> enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
            }

            T? current;
            if (count == 0 ? Enumerator_Empty_Current_UndefinedOperation_Throws : Enumerator_Current_UndefinedOperation_Throws)
            {
                Assert.ThrowsException<InvalidOperationException>(() => enumerator.Current); // enumerator.Current should fail
            }
            else
            {
                current = enumerator.Current;
                Assert.AreEqual(default, current);
            }

            // Test after add
            var seed = 3538963;
            for (var i = 0; i < 3; i++)
            {
                collection.Add(CreateT(seed++));

                if (count == 0 ? Enumerator_Empty_Current_UndefinedOperation_Throws : Enumerator_Current_UndefinedOperation_Throws)
                {
                    Assert.ThrowsException<InvalidOperationException>(() => enumerator.Current); // enumerator.Current should fail
                }
                else
                {
                    current = enumerator.Current;
                    Assert.AreEqual(default, current);
                }
            }
        }
    }

    #endregion
}