// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Utilities;
using PG.Testing.Collections;

namespace PG.Commons.Test.Utilities;

public abstract class ReadOnlyFrugalListTest<T> : IReadOnlyListTestSuite<T>
{
    protected override IReadOnlyList<T> GenericIReadOnlyListFactory(IEnumerable<T> enumerable)
    {
        return new ReadOnlyFrugalList<T>(enumerable);
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

        foreach (var modifyEnumerable in IListTesSuite<T>.GetModifyEnumerables(mods, CreateT))
        {
            if (modifyEnumerable(asEnumerable))
            {
                CollectionAssert.AreNotEqual(asEnumerable.ToList(), roFrugal.ToList());
            }
        }
    }



    //[TestMethod]
    //public void CopyTo()
    //{
    //    var collection = new ReadOnlyFrugalList<int>(SIntArray);
    //    const int targetIndex = 3;
    //    int[] intArray = new int[SIntArray.Length + targetIndex];

    //    Assert.ThrowsException<ArgumentNullException>(() => collection.CopyTo(null, 0));
    //    Assert.ThrowsException<ArgumentException>(() => ((ICollection)collection).CopyTo(new int[SIntArray.Length, SIntArray.Length], 0));
    //    Assert.ThrowsException<ArgumentOutOfRangeException>(() => collection.CopyTo(intArray, -1));
    //    Assert.ThrowsException<ArgumentException>(() => collection.CopyTo(intArray, SIntArray.Length - 1));

    //    collection.CopyTo(intArray, targetIndex);
    //    for (int i = targetIndex; i < intArray.Length; i++)
    //    {
    //        Assert.AreEqual(collection[i - targetIndex], intArray[i]);
    //    }

    //    object[] objectArray = new object[SIntArray.Length + targetIndex];
    //    ((ICollection)collection).CopyTo(intArray, targetIndex);
    //    for (int i = targetIndex; i < intArray.Length; i++)
    //    {
    //        Assert.AreEqual(collection[i - targetIndex], intArray[i]);
    //    }
    //}
}


[TestClass]
public class ReadOnlyFrugalList_Test_String : ReadOnlyFrugalListTest<string>
{
    protected override string CreateT(int seed)
    {
        var stringLength = seed % 10 + 5;
        var rand = new Random(seed);
        var bytes = new byte[stringLength];
        rand.NextBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}

[TestClass]
public class ReadOnlyFrugalList_Test_Int : ReadOnlyFrugalListTest<int>
{
    protected override int CreateT(int seed)
    {
        var rand = new Random(seed);
        return rand.Next();
    }
}


[TestClass]
public class ReadOnlyFrugalList_Test_Int_FromFrugal : ReadOnlyFrugalListTest<int>
{
    protected override int CreateT(int seed)
    {
        var rand = new Random(seed);
        return rand.Next();
    }

    protected override IReadOnlyList<int> GenericIReadOnlyListFactory(IEnumerable<int> enumerable)
    {
        var frugal = new FrugalList<int>(enumerable);
        return frugal.AsReadOnly();
    }
}