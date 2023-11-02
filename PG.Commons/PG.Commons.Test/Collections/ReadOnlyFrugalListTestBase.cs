// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Collections;
using PG.Testing;
using PG.Testing.Collections;

namespace PG.Commons.Test.Collections;

/// <summary>
/// Contains tests that ensure the correctness of the <see cref="ReadOnlyFrugalList{T}"/> class.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class ReadOnlyFrugalListTestBase<T> : IReadOnlyListTestSuite<T>
{
    protected virtual Type ICollection_Generic_CopyTo_IndexLargerThanArrayCount_ThrowType => typeof(ArgumentException);

    protected virtual ReadOnlyFrugalList<T> GenericReadOnlyListFrugalListFactory(IEnumerable<T> enumerable)
    {
        return new ReadOnlyFrugalList<T>(enumerable);
    }

    protected virtual ReadOnlyFrugalList<T> GenericReadOnlyListFrugalListFactory(int count)
    {
        var baseCollection = CreateEnumerable(null, count, 0, 0);
        return GenericReadOnlyListFrugalListFactory(baseCollection);
    }

    protected override IReadOnlyList<T> GenericIReadOnlyListFactory(IEnumerable<T> enumerable)
    {
        return GenericReadOnlyListFrugalListFactory(enumerable);
    }

    #region Empty

    [TestMethod]
    public void Empty_Idempotent()
    {
        Assert.IsNotNull(ReadOnlyFrugalList<T>.Empty);
        Assert.AreEqual(0, ReadOnlyFrugalList<T>.Empty.Count);
        Assert.AreEqual(ReadOnlyFrugalList<T>.Empty, ReadOnlyFrugalList<T>.Empty);
    }

    #endregion

    #region Ctors

    [TestMethod]
    public void Ctor_NullList_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new ReadOnlyFrugalList<int>(null!));
    }

    [TestMethod]
    public void Ctor_Single()
    {
        var t = CreateT(0);
        var list = new ReadOnlyFrugalList<T>(t);
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(t, list[0]);
    }

    [TestMethod]
    [DynamicData(nameof(GetEnumerableTestData), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void Ctor_ModificationsGetNotReflectedWhenOriginalListChanges(int _, int enumerableLength, int __, int numberOfDuplicateElements)
    {
        var enumerable = CreateEnumerable(null, enumerableLength, 0, numberOfDuplicateElements);

        var frugal = new FrugalList<T>(enumerable);
        ref var refFrugal = ref frugal;

        var roFrugal = new ReadOnlyFrugalList<T>(in frugal);

        CollectionAssert.AreEqual(refFrugal.ToList(), roFrugal.ToList());

        if (enumerableLength == 0)
            return;

        var asEnumerable = (IList<T>)frugal;

        var mods = ModifyOperation.Add | ModifyOperation.Insert | ModifyOperation.Overwrite | ModifyOperation.Remove | ModifyOperation.Clear;

        foreach (var modifyEnumerable in IListTestSuite<T>.GetModifyEnumerables(mods, CreateT))
            if (modifyEnumerable(asEnumerable))
                CollectionAssert.AreNotEqual(asEnumerable.ToList(), roFrugal.ToList());
    }

    #endregion

    #region Copy To

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyTo_NullArray_ThrowsArgumentNullException(int count)
    {
        var collection = GenericReadOnlyListFrugalListFactory(count);
        Assert.ThrowsException<ArgumentNullException>(() => collection.CopyTo(null!, 0));
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyTo_NegativeIndex_ThrowsArgumentOutOfRangeException(int count)
    {
        var collection = GenericReadOnlyListFrugalListFactory(count);
        var array = new T[count];
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => collection.CopyTo(array, -1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => collection.CopyTo(array, int.MinValue));
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyTo_IndexEqualToArrayCount_ThrowsArgumentException(int count)
    {
        var collection = GenericReadOnlyListFrugalListFactory(count);
        var array = new T[count];
        if (count > 0)
            Assert.ThrowsException<ArgumentException>(() => collection.CopyTo(array, count));
        else
            collection.CopyTo(array, count); // does nothing since the array is empty
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyTo_IndexLargerThanArrayCount_ThrowsAnyArgumentException(int count)
    {
        var collection = GenericReadOnlyListFrugalListFactory(count);
        var array = new T[count];
        ExceptionUtilities.AssertThrowsException(ICollection_Generic_CopyTo_IndexLargerThanArrayCount_ThrowType, () => collection.CopyTo(array, count + 1));
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyTo_NotEnoughSpaceInOffsettedArray_ThrowsArgumentException(int count)
    {
        if (count > 0) // Want the T array to have at least 1 element
        {
            var collection = GenericReadOnlyListFrugalListFactory(count);
            var array = new T[count];
            Assert.ThrowsException<ArgumentException>(() => collection.CopyTo(array, 1));
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyTo_ExactlyEnoughSpaceInArray(int count)
    {
        var collection = GenericReadOnlyListFrugalListFactory(count);
        var array = new T[count];
        collection.CopyTo(array, 0);
        Assert.IsTrue(collection.SequenceEqual(array));
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyTo_ArrayIsLargerThanCollection(int count)
    {
        var collection = GenericReadOnlyListFrugalListFactory(count);
        var array = new T[count * 3 / 2];
        collection.CopyTo(array, 0);
        Assert.IsTrue(collection.SequenceEqual(array.Take(count)));
    }

    #endregion

    #region Linq Equivalents

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ToList(int count)
    {
        var enumerable = CreateEnumerable(null, count, 0, 0);
        var list = new FrugalList<T>(enumerable);
        CollectionAssert.AreEqual(enumerable.ToList(), list.ToList());
    }


    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void FirstOrDefault(int count)
    {
        var list = GenericReadOnlyListFrugalListFactory(count);
        Assert.AreEqual(count == 0 ? default : list[0], list.FirstOrDefault());
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void First(int count)
    {
        var list = GenericReadOnlyListFrugalListFactory(count);
        if (count == 0)
            Assert.ThrowsException<InvalidOperationException>(() => list.First());
        else
            Assert.AreEqual(list[0], list.First());
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void LastOrDefault(int count)
    {
        var list = GenericReadOnlyListFrugalListFactory(count);
        Assert.AreEqual(count == 0 ? default : list[count - 1], list.LastOrDefault());
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void Last(int count)
    {
        var list = GenericReadOnlyListFrugalListFactory(count);
        if (count == 0)
            Assert.ThrowsException<InvalidOperationException>(() => list.Last());
        else
            Assert.AreEqual(list[count - 1], list.Last());
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ToArray(int count)
    {
        var list = GenericReadOnlyListFrugalListFactory(count);
        var array = list.ToArray();
        CollectionAssert.AreEqual(list.ToList(), array);
    }

    #endregion

    #region Get Enumerator

    [TestMethod]
    [DynamicData(nameof(GetEnumerableTestData), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void GetEnumerator(int _, int enumerableLength, int __, int numberOfDuplicateElements)
    {
        var enumerable = CreateEnumerable(null, enumerableLength, 0, numberOfDuplicateElements);
        var list = new FrugalList<T>(enumerable);

        var actualList = new List<T>();

        using var enumerator = list.GetEnumerator();
        while (enumerator.MoveNext())
            actualList.Add(enumerator.Current);

        CollectionAssert.AreEqual(enumerable.ToList(), actualList);
    }

    #endregion
}