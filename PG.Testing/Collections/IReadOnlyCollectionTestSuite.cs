using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PG.Testing.Collections;

// This test suite is taken from the .NET runtime repository (https://github.com/dotnet/runtime) and adapted to the VSTesting Framework.
// The .NET Foundation licenses this under the MIT license.
/// <summary>
/// Contains tests that ensure the correctness of any class that implements the generic
/// <see cref="IReadOnlyCollection{T}"/> interface
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class IReadOnlyCollectionTestSuite<T> : INonModifyingEnumerableTestSuite<T>
{
    /// <summary>
    /// Creates an instance of an <see cref="IReadOnlyCollection{T}"/> that can be used for testing.
    /// </summary>
    /// <returns>An instance of an <see cref="IReadOnlyCollection{T}"/> that can be used for testing.</returns>
    protected abstract IReadOnlyCollection<T> GenericIReadOnlyCollectionFactory(IEnumerable<T> baseCollection);

    protected override IEnumerable<T> GenericIEnumerableFactory(int count)
    {
        return GenericIReadOnlyCollectionFactory(count);
    }

    /// <summary>
    /// Creates an instance of an <see cref="IReadOnlyCollection{T}"/> that can be used for testing.
    /// </summary>
    /// <param name="count">The number of unique items that the returned <see cref="IReadOnlyCollection{T}"/> contains.</param>
    /// <returns>An instance of an <see cref="IReadOnlyCollection{T}"/> that can be used for testing.</returns>
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