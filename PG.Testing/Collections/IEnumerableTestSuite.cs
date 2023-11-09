using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.Testing.Collections;

// This test suite is taken from the .NET runtime repository (https://github.com/dotnet/runtime) and adapted to the VSTesting Framework.
// The .NET Foundation licenses this under the MIT license.
/// <summary>
/// Contains tests that ensure the correctness of any class that implements the generic
/// IEnumerable interface.
/// </summary>
[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
[SuppressMessage("ReSharper", "AccessToDisposedClosure")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public abstract class IEnumerableTestSuite<T> : INonModifyingEnumerableTestSuite<T>
{
    /// <summary>
    /// Modifies the given IEnumerable such that any enumerators for that IEnumerable will be
    /// invalidated.
    /// </summary>
    /// <param name="enumerable">An IEnumerable to modify</param>
    /// <returns>true if the enumerable was successfully modified. Else false.</returns>
    public delegate bool ModifyEnumerable(IEnumerable<T> enumerable);

    /// <summary>
    /// When calling MoveNext or Reset after modification of the enumeration, the resulting behavior is
    /// undefined. Tests are included to cover two behavioral scenarios:
    ///   - Throwing an InvalidOperationException
    ///   - Execute MoveNext or Reset.
    ///
    /// If this property is set to true, the tests ensure that the exception is thrown. The default value is
    /// true.
    /// </summary>
    protected virtual bool Enumerator_ModifiedDuringEnumeration_ThrowsInvalidOperationException => true;

    /// <summary>
    /// When calling MoveNext or Reset after modification of an empty enumeration, the resulting behavior is
    /// undefined. Tests are included to cover two behavioral scenarios:
    ///   - Throwing an InvalidOperationException
    ///   - Execute MoveNext or Reset.
    ///
    /// If this property is set to true, the tests ensure that the exception is thrown. The default value is
    /// <see cref="Enumerator_ModifiedDuringEnumeration_ThrowsInvalidOperationException"/>.
    /// </summary>
    protected virtual bool Enumerator_Empty_ModifiedDuringEnumeration_ThrowsInvalidOperationException => Enumerator_ModifiedDuringEnumeration_ThrowsInvalidOperationException;

    protected virtual ModifyOperation ModifyEnumeratorThrows => ModifyOperation.Add | ModifyOperation.Insert | ModifyOperation.Overwrite | ModifyOperation.Remove | ModifyOperation.Clear;

    protected virtual ModifyOperation ModifyEnumeratorAllowed => ModifyOperation.None;

    /// <summary>
    /// To be implemented in the concrete collections test classes. Returns a set of ModifyEnumerable delegates
    /// that modify the enumerable passed to them.
    /// </summary>
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
            while (enumerator.MoveNext())
            {
            }

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
            {
            }

            if (modifyEnumerable(enumerable)) 
                enumerator.Reset();
        }
    }

    #endregion
}