// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Utilities;
using PG.Testing;
using PG.Testing.Collections;

namespace PG.Commons.Test.Utilities;

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

    [TestMethod]
    public void Ctor_NullList_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new ReadOnlyFrugalList<int>(null!));
    }

    [TestMethod]
    public void Empty_Idempotent()
    {
        Assert.IsNotNull(ReadOnlyFrugalList<int>.Empty);
        Assert.AreEqual(0, ReadOnlyFrugalList<int>.Empty.Count);
        Assert.AreEqual(ReadOnlyFrugalList<int>.Empty, ReadOnlyFrugalList<int>.Empty);
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
    public void Ctor_NoSideEffectWhenOriginalListChanges(int _, int enumerableLength, int __, int numberOfDuplicateElements)
    {
        var enumerable = CreateEnumerable(null, enumerableLength, 0, numberOfDuplicateElements);

        var frugal = new FrugalList<T>(enumerable);
        ref var refFrugal = ref frugal;

        var roFrugal = new ReadOnlyFrugalList<T>(in frugal);

        CollectionAssert.AreEqual(refFrugal.ToList(), roFrugal.ToList());

        if (enumerableLength == 0)
            return;

        var asEnumerable = (IList<T>)frugal;

        var mods = ModifyOperation.Add | ModifyOperation.Insert | ModifyOperation.Overwrite | ModifyOperation.Remove |
                   ModifyOperation.Clear;

        foreach (var modifyEnumerable in IListTestSuite<T>.GetModifyEnumerables(mods, CreateT))
        {
            if (modifyEnumerable(asEnumerable))
            {
                CollectionAssert.AreNotEqual(asEnumerable.ToList(), roFrugal.ToList());
            }
        }
    }


    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_CopyTo_NullArray_ThrowsArgumentNullException(int count)
    {
        var collection = GenericReadOnlyListFrugalListFactory(count);
        Assert.ThrowsException<ArgumentNullException>(() => collection.CopyTo(null!, 0));
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_CopyTo_NegativeIndex_ThrowsArgumentOutOfRangeException(int count)
    {
        var collection = GenericReadOnlyListFrugalListFactory(count);
        var array = new T[count];
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => collection.CopyTo(array, -1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => collection.CopyTo(array, int.MinValue));
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_CopyTo_IndexEqualToArrayCount_ThrowsArgumentException(int count)
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
    public void ICollection_Generic_CopyTo_IndexLargerThanArrayCount_ThrowsAnyArgumentException(int count)
    {
        var collection = GenericReadOnlyListFrugalListFactory(count);
        var array = new T[count];
        ExceptionUtilities.AssertThrowsException(ICollection_Generic_CopyTo_IndexLargerThanArrayCount_ThrowType, () => collection.CopyTo(array, count + 1));
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_CopyTo_NotEnoughSpaceInOffsettedArray_ThrowsArgumentException(int count)
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
    public void ICollection_Generic_CopyTo_ExactlyEnoughSpaceInArray(int count)
    {
        var collection = GenericReadOnlyListFrugalListFactory(count);
        var array = new T[count];
        collection.CopyTo(array, 0);
        Assert.IsTrue(collection.SequenceEqual(array));
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_CopyTo_ArrayIsLargerThanCollection(int count)
    {
        var collection = GenericReadOnlyListFrugalListFactory(count);
        var array = new T[count * 3 / 2];
        collection.CopyTo(array, 0);
        Assert.IsTrue(collection.SequenceEqual(array.Take(count)));
    }
}