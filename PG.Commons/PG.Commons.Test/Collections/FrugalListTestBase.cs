// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Collections;
using PG.Testing.Collections;

namespace PG.Commons.Test.Collections;

/// <summary>
/// Contains tests that ensure the correctness of the <see cref=" FrugalList{T}"/> class.
/// </summary>
public abstract class FrugalListTestBase<T> : IListTestSuite<T>
{
    protected override bool Enumerator_ModifiedDuringEnumeration_ThrowsInvalidOperationException => false;

    protected override IList<T> GenericIListFactory()
    {
        return GenericFrugalListFactory();
    }

    protected override IList<T> GenericIListFactory(int count)
    {
        return GenericFrugalListFactory(count);
    }

    private static FrugalList<T> GenericFrugalListFactory()
    {
        return new FrugalList<T>();
    }

    private FrugalList<T> GenericFrugalListFactory(int count)
    {
        var toCreateFrom = CreateEnumerable(null, count, 0, 0);
        return new FrugalList<T>(toCreateFrom);
    }

    #region Ctors

    [TestMethod]
    public void Struct_Default()
    {
        var list = default(FrugalList<T>);
        Assert.AreEqual(0, list.Count);
        Assert.IsFalse(list.IsReadOnly);
    }

    [TestMethod]
    public void Constructor_Empty()
    {
        var list = new FrugalList<T>();
        Assert.AreEqual(0, list.Count);
        Assert.IsFalse(list.IsReadOnly);
    }

    [TestMethod]
    public void Constructor_Single()
    {
        var t = CreateT(0);
        var list = new FrugalList<T>(t);
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(t, list[0]);
        Assert.IsFalse(list.IsReadOnly);
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void Constructor_OtherFrugalList_Creates_Copy(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorThrows))
        {
            var source = GenericFrugalListFactory(count);
            ref var refSource = ref source;
            var other = new FrugalList<T>(in refSource);

            IList<T> asEnumerable = refSource;

            if (modifyEnumerable(asEnumerable))
                CollectionAssert.AreNotEqual(asEnumerable.ToList(), other.ToList());
        }
    }

    [TestMethod]
    [DynamicData(nameof(GetEnumerableTestData), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void Constructor_IEnumerable(int _, int enumerableLength, int __, int numberOfDuplicateElements)
    {
        var enumerable = CreateEnumerable(null, enumerableLength, 0, numberOfDuplicateElements);
        var list = new FrugalList<T>(enumerable);
        var expected = enumerable.ToList();

        Assert.AreEqual(enumerableLength, list.Count); //"Number of items in list do not match the number of items given."

        for (var i = 0; i < enumerableLength; i++)
            Assert.AreEqual(expected[i], list[i]); //"Expected object in item array to be the same as in the list"

        Assert.IsFalse(list.IsReadOnly); //"List should not be readonly"
    }

    [TestMethod]
    [DynamicData(nameof(GetEnumerableTestData), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void Constructor_IEnumerable_Creates_Copy(int _, int enumerableLength, int __, int numberOfDuplicateElements)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorThrows))
        {
            var enumerable = CreateEnumerable(null, enumerableLength, 0, numberOfDuplicateElements);
            var list = new FrugalList<T>(enumerable);

            if (modifyEnumerable(enumerable))
                CollectionAssert.AreNotEqual(enumerable.ToList(), list.ToList());
        }
    }

    [TestMethod]
    public void Constructor_NullIEnumerable_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => { _ = new FrugalList<T>(null!); });
    }

    #endregion

    #region Boxing & ByRef Behavior

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void Boxing_ReflectsAllChanges(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorThrows))
        {
            var source = GenericIEnumerableFactory(count);
            var copy = source;

            if (modifyEnumerable(source))
                CollectionAssert.AreEqual(source.ToList(), copy.ToList());
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ByRef_ReflectsAllChanges(int count)
    {
        var source = GenericFrugalListFactory(count);

        ref var copy = ref source;

        copy.Add(CreateT(0));
        copy.Insert(0, CreateT(1));

        CollectionAssert.AreEqual(source.ToList(), copy.ToList());
    }

    #endregion

    #region CopyByValue Behavior

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyByValue_SideEffects_Clear(int count)
    {
        var source = GenericFrugalListFactory(count);
        var copy = source;

        source.Clear();
        if (count >= 1)
            CollectionAssert.AreNotEqual(source.ToList(), copy.ToList());
        else
            CollectionAssert.AreEqual(source.ToList(), copy.ToList()); // Clear on empty list does not have visible changes
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyByValue_SideEffects_AddingItems(int count)
    {
        var source = GenericFrugalListFactory(count);
        var copy = source;

        source.Add(CreateT(0));
        if (count <= 1)
            CollectionAssert.AreNotEqual(source.ToList(), copy.ToList());
        else
            CollectionAssert.AreEqual(source.ToList(), copy.ToList()); // Only adding items to the backing lists gets reflected
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyByValue_SideEffects_InsertFirst(int count)
    {
        var source = GenericFrugalListFactory(count);
        var copy = source;

        source.Insert(0, CreateT(0));
        CollectionAssert.AreNotEqual(source.ToList(), copy.ToList());
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyByValue_SideEffects_RemoveFirst(int count)
    {
        var source = GenericFrugalListFactory(count);
        var copy = source;
        if (count <= 0)
            return;

        source.RemoveAt(0);
        CollectionAssert.AreNotEqual(source.ToList(), copy.ToList());
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyByValue_SideEffects_RemoveLast(int count)
    {
        var source = GenericFrugalListFactory(count);
        var copy = source;
        if (count <= 0)
            return;

        source.RemoveAt(count - 1);
        if (count == 1)
            CollectionAssert.AreNotEqual(source.ToList(), copy.ToList());
        else
            CollectionAssert.AreEqual(source.ToList(), copy.ToList()); // Only removing items from the backing lists gets reflected
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyByValue_SideEffects_OverrideFirst(int count)
    {
        var source = GenericFrugalListFactory(count);
        var copy = source;
        if (count <= 0)
            return;

        source[0] = CreateT(0);
        CollectionAssert.AreNotEqual(source.ToList(), copy.ToList());
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyByValue_SideEffects_OverrideLast(int count)
    {
        var source = GenericFrugalListFactory(count);
        var copy = source;
        if (count <= 0)
            return;

        source[count - 1] = CreateT(0);
        if (count == 1)
            CollectionAssert.AreNotEqual(source.ToList(), copy.ToList());
        else
            CollectionAssert.AreEqual(source.ToList(), copy.ToList()); // Only modifying items from the backing lists gets reflected
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
        var list = GenericFrugalListFactory(count);
        Assert.AreEqual(count == 0 ? default : list[0], list.FirstOrDefault());
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void First(int count)
    {
        var list = GenericFrugalListFactory(count);
        if (count == 0)
            Assert.ThrowsException<InvalidOperationException>(() => list.First());
        else
            Assert.AreEqual(list[0], list.First());
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void LastOrDefault(int count)
    {
        var list = GenericFrugalListFactory(count);
        Assert.AreEqual(count == 0 ? default : list[count - 1], list.LastOrDefault());
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void Last(int count)
    {
        var list = GenericFrugalListFactory(count);
        if (count == 0)
            Assert.ThrowsException<InvalidOperationException>(() => list.Last());
        else
            Assert.AreEqual(list[count - 1], list.Last());
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ToArray(int count)
    {
        var list = GenericFrugalListFactory(count);
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