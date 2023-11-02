using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PG.Testing.Collections;

// This test suite is taken from the .NET runtime repository (https://github.com/dotnet/runtime) and adapted to the VSTesting Framework.
// The .NET Foundation licenses this under the MIT license.
/// <summary>
/// Provides a base set of nongeneric operations that are used by all other testing interfaces.
/// </summary>
public abstract class CollectionsTestSuite
{
    [Flags]
    public enum ModifyOperation
    {
        None = 0,
        Add = 1,
        Insert = 2,
        Overwrite = 4,
        Remove = 8,
        Clear = 16
    }

    public static IEnumerable<object[]> ValidCollectionSizes()
    {
        yield return new object[] { 0 };
        yield return new object[] { 1 };
        yield return new object[] { 75 };
    }
    
    protected static IEnumerable<object[]> GetEnumerableTestData()
    {
        foreach (var collectionSizeArray in ValidCollectionSizes())
        {
            var count = (int)collectionSizeArray[0];
            yield return new object[] { count, 0, 0, 0 };                       // Empty Enumerable
            yield return new object[] { count, count + 1, 0, 0 };               // Enumerable that is 1 larger

            if (count >= 1)
            {
                yield return new object[] { count, count, 0, 0 };               // Enumerable of the same size
                yield return new object[] { count, count - 1, 0, 0 };           // Enumerable that is 1 smaller
                yield return new object[] { count, count, 1, 0 };               // Enumerable of the same size with 1 matching element
                yield return new object[] { count, count + 1, 1, 0 };           // Enumerable that is 1 longer with 1 matching element
                yield return new object[] { count, count, count, 0 };           // Enumerable with all elements matching
                yield return new object[] { count, count + 1, count, 0 };       // Enumerable with all elements matching plus one extra
            }

            if (count >= 2)
            {
                yield return new object[] { count, count - 1, 1, 0 };           // Enumerable that is 1 smaller with 1 matching element
                yield return new object[] { count, count + 2, 2, 0 };           // Enumerable that is 2 longer with 2 matching element
                yield return new object[] { count, count - 1, count - 1, 0 };   // Enumerable with all elements matching minus one
                yield return new object[] { count, count, 2, 0 };               // Enumerable of the same size with 2 matching element
                yield return new object[] { count, count, 0, 1 };               // Enumerable with 1 element duplicated
            }

            if (count >= 3)
            {
                yield return new object[] { count, count, 0, 1 };               // Enumerable with all elements duplicated
                yield return new object[] { count, count - 1, 2, 0 };           // Enumerable that is 1 smaller with 2 matching elements
            }
        }
    }
}


// This test suite is taken from the .NET runtime repository (https://github.com/dotnet/runtime) and adapted to the VSTesting Framework.
// The .NET Foundation licenses this under the MIT license.
/// <summary>
/// Provides a base set of generic operations that are used by all other generic testing interfaces.
/// </summary>
public abstract class CollectionsTestSuite<T> : CollectionsTestSuite
{
    /// <summary>
    /// To be implemented in the concrete collections test classes. Creates an instance of T that
    /// is dependent only on the seed passed as input and will return the same value on repeated
    /// calls with the same seed.
    /// </summary>
    protected abstract T CreateT(int seed);

    /// <summary>
    /// The EqualityComparer that can be used in the overriding class when creating test enumerables
    /// or test collections. Default if not overridden is the default comparator.
    /// </summary>
    protected virtual IEqualityComparer<T> GetIEqualityComparer() => EqualityComparer<T>.Default;

    /// <summary>
    /// Helper function to create an enumerable fulfilling the given specific parameters. The function will
    /// create an enumerable of the desired type using the Default constructor for that type and then add values
    /// to it until it is full. It will begin by adding the desired number of matching and duplicate elements,
    /// followed by random (deterministic) elements until the desired count is reached.
    /// </summary>
    protected IEnumerable<T> CreateEnumerable(IEnumerable<T>? enumerableToMatchTo, int count, int numberOfMatchingElements, int numberOfDuplicateElements)
    {
        return CreateList(enumerableToMatchTo, count, numberOfMatchingElements, numberOfDuplicateElements);
    }

    /// <summary>
    /// Helper function to create an List fulfilling the given specific parameters. The function will
    /// create an List and then add values
    /// to it until it is full. It will begin by adding the desired number of matching,
    /// followed by random (deterministic) elements until the desired count is reached.
    /// </summary>
    protected IEnumerable<T> CreateList(IEnumerable<T>? enumerableToMatchTo, int count, int numberOfMatchingElements, int numberOfDuplicateElements)
    {
        var list = new List<T>(count);
        var seed = 528;
        var duplicateAdded = 0;
        List<T> match = null!;

        // Add Matching elements
        if (enumerableToMatchTo != null)
        {
            match = enumerableToMatchTo.ToList();
            for (var i = 0; i < numberOfMatchingElements; i++)
            {
                list.Add(match[i]);
                while (duplicateAdded++ < numberOfDuplicateElements)
                    list.Add(match[i]);
            }
        }

        // Add elements to reach the desired count
        while (list.Count < count)
        {
            var toAdd = CreateT(seed++);
            while (list.Contains(toAdd) || (match != null && match.Contains(toAdd))) // Don't want any unexpectedly duplicate values
                toAdd = CreateT(seed++);
            list.Add(toAdd);
            while (duplicateAdded++ < numberOfDuplicateElements)
                list.Add(toAdd);
        }

        // Validate that the Enumerable fits the guidelines as expected
        Debug.Assert(list.Count == count);
        if (match != null)
        {
            var actualMatchingCount = 0;
            foreach (var lookingFor in match)
                actualMatchingCount += list.Contains(lookingFor) ? 1 : 0;
            Assert.AreEqual(numberOfMatchingElements, actualMatchingCount);
        }

        return list;
    }
}