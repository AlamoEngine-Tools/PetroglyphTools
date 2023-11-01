using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Utilities;
using PG.Testing.Collections;

namespace PG.Commons.Test.Utilities;

[TestClass]
public class FrugalList_Test_String : FrugalListTest<string>
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
public class FrugalList_Test_Int : FrugalListTest<int>
{
    protected override int CreateT(int seed)
    {
        var rand = new Random(seed);
        return rand.Next();
    }
}

public abstract class FrugalListTest<T> : IListTesSuite<T>
{
    protected override bool Enumerator_ModifiedDuringEnumeration_ThrowsInvalidOperationException => false;
    
    protected override IList<T> GenericIListFactory()
    {
        return GenericListFactory();
    }

    protected override IList<T> GenericIListFactory(int count)
    {
        return GenericListFactory(count);
    }

    private static FrugalList<T> GenericListFactory()
    {
        return new FrugalList<T>();
    }

    private FrugalList<T> GenericListFactory(int count)
    {
        var toCreateFrom = CreateEnumerable(null, count, 0, 0);
        return new FrugalList<T>(toCreateFrom);
    }

    [TestMethod]
    public void Struct_Default()
    {
        var list = default(FrugalList<T>);
        Assert.AreEqual(0, list.Count);
        Assert.IsFalse(list.IsReadOnly);
    }

    [TestMethod]
    public void Constructor_Default()
    {
        var list = new FrugalList<T>();
        Assert.AreEqual(0, list.Count);
        Assert.IsFalse(list.IsReadOnly);
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void Constructor_OtherFrugalList_Creates_Copy(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorThrows))
        {
            var source = GenericListFactory(count);
            var other = new FrugalList<T>(in source);

            IList<T> asEnumerable = source;

            if (modifyEnumerable(asEnumerable)) 
                CollectionAssert.AreNotEqual(asEnumerable.ToList(), other.ToList());
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void BoxingHasNoSideEffects(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorThrows))
        {
            var source = GenericIEnumerableFactory(count);
            var copy = source;

            if (modifyEnumerable(source))
            {
                CollectionAssert.AreEqual(source.ToList(), copy.ToList());
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ByRefHasNoSideEffects(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorThrows))
        {
            var source = GenericListFactory(count);

            ref var copy = ref source;

            if (modifyEnumerable(copy))
            {
                CollectionAssert.AreEqual(source.ToList(), copy.ToList());
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ByRefHasNoSideEffects2(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorThrows))
        {
            var source = GenericListFactory(count);

            ref var byRefSource = ref source;

            var copy = byRefSource;

            if (modifyEnumerable(byRefSource))
            {
                CollectionAssert.AreEqual(byRefSource.ToList(), copy.ToList());
                CollectionAssert.AreEqual(byRefSource.ToList(), source.ToList());
            }
        }
    }



    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyByValue_SideEffects_Clear(int count)
    {
        var source = GenericListFactory(count);
        var copy = source;

        source.Clear();
        if (count >= 1)
            CollectionAssert.AreNotEqual(source.ToList(), copy.ToList());
        else
            CollectionAssert.AreEqual(source.ToList(), copy.ToList());
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyByValue_SideEffects_AddingItems(int count)
    {
        var source = GenericListFactory(count);
        var copy = source;

        source.Add(CreateT(0));
        if (count <= 1) 
            CollectionAssert.AreNotEqual(source.ToList(), copy.ToList());
        else 
            CollectionAssert.AreEqual(source.ToList(), copy.ToList());
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyByValue_SideEffects_InsertFirst(int count)
    {
        var source = GenericListFactory(count);
        var copy = source;

        source.Insert(0, CreateT(0));
        CollectionAssert.AreNotEqual(source.ToList(), copy.ToList());
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyByValue_SideEffects_RemoveFirst(int count)
    {
        var source = GenericListFactory(count);
        var copy = source;
        if (count > 0)
        {
            source.RemoveAt(0);
            CollectionAssert.AreNotEqual(source.ToList(), copy.ToList());
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyByValue_SideEffects_RemoveLast(int count)
    {
        var source = GenericListFactory(count);
        var copy = source;
        if (count > 0)
        {
            source.RemoveAt(count - 1);
            if (count == 1)
                CollectionAssert.AreNotEqual(source.ToList(), copy.ToList());
            else
                CollectionAssert.AreEqual(source.ToList(), copy.ToList());
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyByValue_SideEffects_OverrideFirst(int count)
    {
        var source = GenericListFactory(count);
        var copy = source;
        if (count > 0)
        {
            source[0] = CreateT(0);
            CollectionAssert.AreNotEqual(source.ToList(), copy.ToList());
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void CopyByValue_SideEffects_OverrideLast(int count)
    {
        var source = GenericListFactory(count);
        var copy = source;
        if (count > 0)
        {
            source[count - 1] = CreateT(0);
            if (count == 1)
                CollectionAssert.AreNotEqual(source.ToList(), copy.ToList());
            else
                CollectionAssert.AreEqual(source.ToList(), copy.ToList());
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
    public void Constructor_NullIEnumerable_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => { _ = new FrugalList<T>(null!); });
    }
}