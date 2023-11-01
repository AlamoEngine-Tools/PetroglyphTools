using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.Testing.Collections;

// This test suite is taken from the .NET runtime repository (https://github.com/dotnet/runtime) and adapted to the VSTesting Framework.
// The .NET Foundation licenses this under the MIT license.
[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
public abstract class IEnumerableTestSuite<T> : INonModifyingEnumerableTestSuite<T>
{
    public delegate bool ModifyEnumerable(IEnumerable<T> enumerable);

    protected virtual bool Enumerator_ModifiedDuringEnumeration_ThrowsInvalidOperationException => true;

    protected virtual bool Enumerator_Empty_ModifiedDuringEnumeration_ThrowsInvalidOperationException => Enumerator_ModifiedDuringEnumeration_ThrowsInvalidOperationException;

    protected virtual ModifyOperation ModifyEnumeratorThrows => ModifyOperation.Add | ModifyOperation.Insert | ModifyOperation.Overwrite | ModifyOperation.Remove | ModifyOperation.Clear;

    protected virtual ModifyOperation ModifyEnumeratorAllowed => ModifyOperation.None;

    protected abstract IEnumerable<ModifyEnumerable> GetModifyEnumerables(ModifyOperation operations);
    

    #region Enumerator.MoveNext

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IEnumerable_Generic_Enumerator_MoveNext_ModifiedBeforeEnumeration_ThrowsInvalidOperationException(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorThrows))
        {
            var enumerable = GenericIEnumerableFactory(count);
            using var enumerator = enumerable.GetEnumerator();
            if (modifyEnumerable(enumerable))
            {
                if (count == 0 ? Enumerator_Empty_ModifiedDuringEnumeration_ThrowsInvalidOperationException : Enumerator_ModifiedDuringEnumeration_ThrowsInvalidOperationException)
                {
                    Assert.ThrowsException<InvalidOperationException>(() => enumerator.MoveNext());
                }
                else
                {
                    enumerator.MoveNext();
                }
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IEnumerable_Generic_Enumerator_MoveNext_ModifiedBeforeEnumeration_Succeeds(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorAllowed))
        {
            var enumerable = GenericIEnumerableFactory(count);
            using var enumerator = enumerable.GetEnumerator();
            if (modifyEnumerable(enumerable))
            {
                if (Enumerator_ModifiedDuringEnumeration_ThrowsInvalidOperationException)
                {
                    enumerator.MoveNext();
                }
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IEnumerable_Generic_Enumerator_MoveNext_ModifiedDuringEnumeration_ThrowsInvalidOperationException(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorThrows))
        {
            var enumerable = GenericIEnumerableFactory(count);
            using var enumerator = enumerable.GetEnumerator();
            for (var i = 0; i < count / 2; i++)
                enumerator.MoveNext();
            if (modifyEnumerable(enumerable))
            {
                if (count == 0 ? Enumerator_Empty_ModifiedDuringEnumeration_ThrowsInvalidOperationException : Enumerator_ModifiedDuringEnumeration_ThrowsInvalidOperationException)
                {
                    Assert.ThrowsException<InvalidOperationException>(() => enumerator.MoveNext());
                }
                else
                {
                    enumerator.MoveNext();
                }
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IEnumerable_Generic_Enumerator_MoveNext_ModifiedDuringEnumeration_Succeeds(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorAllowed))
        {
            var enumerable = GenericIEnumerableFactory(count);
            using var enumerator = enumerable.GetEnumerator();
            for (var i = 0; i < count / 2; i++)
                enumerator.MoveNext();
            if (modifyEnumerable(enumerable))
            {
                enumerator.MoveNext();
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IEnumerable_Generic_Enumerator_MoveNext_ModifiedAfterEnumeration_ThrowsInvalidOperationException(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorThrows))
        {
            var enumerable = GenericIEnumerableFactory(count);
            using var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
            }

            if (modifyEnumerable(enumerable))
            {
                if (count == 0 ? Enumerator_Empty_ModifiedDuringEnumeration_ThrowsInvalidOperationException : Enumerator_ModifiedDuringEnumeration_ThrowsInvalidOperationException)
                {
                    Assert.ThrowsException<InvalidOperationException>(() => enumerator.MoveNext());
                }
                else
                {
                    enumerator.MoveNext();
                }
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IEnumerable_Generic_Enumerator_MoveNext_ModifiedAfterEnumeration_Succeeds(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorAllowed))
        {
            var enumerable = GenericIEnumerableFactory(count);
            using var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
            }

            if (modifyEnumerable(enumerable))
            {
                enumerator.MoveNext();
            }
        }
    }

    #endregion

    #region Enumerator.Current


    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IEnumerable_Generic_Enumerator_Current_ModifiedDuringEnumeration_UndefinedBehavior(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorThrows))
        {
            var enumerable = GenericIEnumerableFactory(count);
            using var enumerator = enumerable.GetEnumerator();
            if (modifyEnumerable(enumerable))
            {
                if (count == 0 ? Enumerator_Empty_Current_UndefinedOperation_Throws : Enumerator_Current_UndefinedOperation_Throws)
                    Assert.ThrowsException<InvalidOperationException>(() => enumerator.Current);
                else
                    _ = enumerator.Current;
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IEnumerable_Generic_Enumerator_Current_ModifiedDuringEnumeration_Succeeds(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorAllowed))
        {
            var enumerable = GenericIEnumerableFactory(count);
            using var enumerator = enumerable.GetEnumerator();
            if (modifyEnumerable(enumerable))
            {
                _ = enumerator.Current;
            }
        }
    }

    #endregion

    #region Enumerator.Reset

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IEnumerable_Generic_Enumerator_Reset_ModifiedBeforeEnumeration_ThrowsInvalidOperationException(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorThrows))
        {
            var enumerable = GenericIEnumerableFactory(count);
            using var enumerator = enumerable.GetEnumerator();
            if (modifyEnumerable(enumerable))
            {
                if (count == 0 ? Enumerator_Empty_ModifiedDuringEnumeration_ThrowsInvalidOperationException : Enumerator_ModifiedDuringEnumeration_ThrowsInvalidOperationException)
                {
                    Assert.ThrowsException<InvalidOperationException>(enumerator.Reset);
                }
                else
                {
                    enumerator.Reset();
                }
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IEnumerable_Generic_Enumerator_Reset_ModifiedBeforeEnumeration_Succeeds(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorAllowed))
        {
            var enumerable = GenericIEnumerableFactory(count);
            using var enumerator = enumerable.GetEnumerator();
            if (modifyEnumerable(enumerable))
            {
                enumerator.Reset();
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IEnumerable_Generic_Enumerator_Reset_ModifiedDuringEnumeration_ThrowsInvalidOperationException(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorThrows))
        {
            var enumerable = GenericIEnumerableFactory(count);
            using var enumerator = enumerable.GetEnumerator();
            for (var i = 0; i < count / 2; i++)
                enumerator.MoveNext();
            if (modifyEnumerable(enumerable))
            {
                if (count == 0 ? Enumerator_Empty_ModifiedDuringEnumeration_ThrowsInvalidOperationException : Enumerator_ModifiedDuringEnumeration_ThrowsInvalidOperationException)
                {
                    Assert.ThrowsException<InvalidOperationException>(enumerator.Reset);
                }
                else
                {
                    enumerator.Reset();
                }
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IEnumerable_Generic_Enumerator_Reset_ModifiedDuringEnumeration_Succeeds(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorAllowed))
        {
            var enumerable = GenericIEnumerableFactory(count);
            using var enumerator = enumerable.GetEnumerator();
            for (var i = 0; i < count / 2; i++)
                enumerator.MoveNext();
            if (modifyEnumerable(enumerable))
            {
                enumerator.Reset();
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IEnumerable_Generic_Enumerator_Reset_ModifiedAfterEnumeration_ThrowsInvalidOperationException(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorThrows))
        {
            var enumerable = GenericIEnumerableFactory(count);
            using var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext()) ;
            if (modifyEnumerable(enumerable))
            {
                if (count == 0 ? Enumerator_Empty_ModifiedDuringEnumeration_ThrowsInvalidOperationException : Enumerator_ModifiedDuringEnumeration_ThrowsInvalidOperationException)
                {
                    Assert.ThrowsException<InvalidOperationException>(enumerator.Reset);
                }
                else
                {
                    enumerator.Reset();
                }
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(ValidCollectionSizes), typeof(CollectionsTestSuite), DynamicDataSourceType.Method)]
    public void IEnumerable_Generic_Enumerator_Reset_ModifiedAfterEnumeration_Succeeds(int count)
    {
        foreach (var modifyEnumerable in GetModifyEnumerables(ModifyEnumeratorAllowed))
        {
            var enumerable = GenericIEnumerableFactory(count);
            using var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
                ;
            if (modifyEnumerable(enumerable))
            {
                enumerator.Reset();
            }
        }
    }

    #endregion
}

#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.