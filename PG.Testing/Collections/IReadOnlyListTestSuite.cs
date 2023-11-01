using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PG.Testing.Collections;

// This test suite is taken from the .NET runtime repository (https://github.com/dotnet/runtime) and adapted to the VSTesting Framework.
// The .NET Foundation licenses this under the MIT license.
public abstract class IReadOnlyListTestSuite<T> : IReadOnlyCollectionTestSuite<T>
{
    protected virtual Type IList_Generic_Item_InvalidIndex_ThrowType => typeof(ArgumentOutOfRangeException);

    protected abstract IReadOnlyList<T> GenericIReadOnlyListFactory(IEnumerable<T> baseCollection);

    protected virtual IReadOnlyList<T> GenericIReadOnlyListFactory(int count)
    {
        var baseCollection = CreateEnumerable(null, count, 0, 0);
        return GenericIReadOnlyListFactory(baseCollection);
    }

    protected override IReadOnlyCollection<T> GenericIReadOnlyCollectionFactory(IEnumerable<T> baseCollection)
    {
        return GenericIReadOnlyListFactory(baseCollection);
    }

    #region FromEnumerable

    [TestMethod]
    [DynamicData(nameof(GetEnumerableTestData), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void From_IEnumerable(int _, int enumerableLength, int __, int numberOfDuplicateElements)
    {
        var enumerable = CreateEnumerable(null, enumerableLength, 0, numberOfDuplicateElements);
        var list = GenericIReadOnlyListFactory(enumerable);

        var expected = enumerable.ToList();
       
        Assert.AreEqual(enumerableLength, list.Count);
        
        for (var i = 0; i < enumerableLength; i++) 
            Assert.AreEqual(expected[i], list[i]);
    }

    #endregion

    #region Item Getter

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_ItemGet_NegativeIndex_ThrowsException(int count)
    {
        var list = GenericIReadOnlyListFactory(count);
        ExceptionUtilities.AssertThrowsException(IList_Generic_Item_InvalidIndex_ThrowType, () => list[-1]);
        ExceptionUtilities.AssertThrowsException(IList_Generic_Item_InvalidIndex_ThrowType, () => list[int.MinValue]);
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_ItemGet_IndexGreaterThanListCount_ThrowsException(int count)
    {
        var list = GenericIReadOnlyListFactory(count);
        ExceptionUtilities.AssertThrowsException(IList_Generic_Item_InvalidIndex_ThrowType, () => list[count]);
        ExceptionUtilities.AssertThrowsException(IList_Generic_Item_InvalidIndex_ThrowType, () => list[count + 1]);
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IList_Generic_ItemGet_ValidGetWithinListBounds(int count)
    {
        var list = GenericIReadOnlyListFactory(count);

        foreach (var i in Enumerable.Range(0, count))
            Sink(list[i]);
        return;

        [MethodImpl(MethodImplOptions.NoInlining)]
        void Sink(T _) { }
    }

    #endregion
}