using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PG.Testing.Collections;

// This test suite is taken from the .NET runtime repository (https://github.com/dotnet/runtime) and adapted to the VSTesting Framework.
// The .NET Foundation licenses this under the MIT license.
public abstract class IReadOnlyCollectionTestSuite<T> : INonModifyingEnumerableTestSuite<T>
{
    protected abstract IReadOnlyCollection<T> GenericIReadOnlyCollectionFactory(IEnumerable<T> baseCollection);

    protected override IEnumerable<T> GenericIEnumerableFactory(int count)
    {
        return GenericIReadOnlyCollectionFactory(count);
    }

    protected virtual IReadOnlyCollection<T> GenericIReadOnlyCollectionFactory(int count)
    {
        var collection = CreateEnumerable(null, count, 0, 0);
        return GenericIReadOnlyCollectionFactory(collection);
    }

    #region Count

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void ICollection_Generic_Count_Validity(int count)
    {
        var collection = GenericIReadOnlyCollectionFactory(count);
        Assert.AreEqual(count, collection.Count);
    }

    #endregion
}