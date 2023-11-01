// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PG.Testing.Collections;

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