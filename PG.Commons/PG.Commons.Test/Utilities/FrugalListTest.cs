using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using PG.Testing;
using System.Collections;

namespace PG.Commons.Test.Utilities;

[TestClass]
public class FrugalList_Class_Test : FrugalListTest<object, FrugalList<object>>
{
    protected override bool ExpectedIsReadOnly => false;

    protected override FrugalList<object> CreateList(IEnumerable<object> items)
    {
        return new FrugalList<object>(items);
    }

    protected override object CreateItem()
    {
        return TestUtility.GetRandomStringOfLength(2);
    }
}

[TestClass]
public class FrugalList_Struct_Test : FrugalListTest<int, FrugalList<int>>
{
    private readonly Random _random = new();

    protected override bool ExpectedIsReadOnly => false;

    protected override FrugalList<int> CreateList(IEnumerable<int> items)
    {
        return new FrugalList<int>(items);
    }

    protected override int CreateItem()
    {
        return _random.Next();
    }
}

public abstract class FrugalListTest<T, TList> where TList : IList<T>
{
    protected abstract bool ExpectedIsReadOnly { get; }

    [TestMethod]
    public void IsReadOnly()
    {
        Assert.AreEqual(ExpectedIsReadOnly, GetList(null).IsReadOnly);
    }

    [TestMethod]
    [DataRow(int.MinValue)]
    [DataRow(-1)]
    [DataRow(0)]
    public void GetItemArgumentOutOfRange(int index)
    {
        var list = GetList(null);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => list[index]);
        CollectionAssert.AreEqual(Array.Empty<object>(), list.ToList());
    }

    [TestMethod]
    public void GetItemArgumentOutOfRangeFilledCollection()
    {
        var items = GenerateItems(32);
        var list = GetList(items);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => list[list.Count]);
        CollectionAssert.AreEqual(items, list.ToList());
    }

    [TestMethod]
    public void GetItemEmpty()
    {
        var items = Array.Empty<T>();
        var list = GetList(items);
        if (ExpectedIsReadOnly)
        {
            CollectionAssert.AreEqual(items, list.ToList());
        }
        else
        {
            list.Clear();
            items = GenerateItems(1);

            AddRange(ref list, items);
            CollectionAssert.AreEqual(items, list.ToList());

            list.Clear();
            items = GenerateItems(1024);
            AddRange(ref list, items);
            CollectionAssert.AreEqual(items, list.ToList());
        }
    }

    [TestMethod]
    public void SetItemNotSupported()
    {
        if (ExpectedIsReadOnly)
        {
            var items = GenerateItems(10);
            var list = GetList(items);
            Assert.ThrowsException<NotSupportedException>(() => list[0] = CreateItem());
            CollectionAssert.AreEqual(items, list.ToList());

            Assert.ThrowsException<NotSupportedException>(() => list[list.Count - 1] = CreateItem());
            CollectionAssert.AreEqual(items, list.ToList());
        }
    }

    [TestMethod]
    public void SetItemArgumentException()
    {
        Type[] expectedExceptions;
        if (ExpectedIsReadOnly)
        {
            expectedExceptions = new[]
            {
                typeof (ArgumentOutOfRangeException),
                typeof (NotSupportedException)
            };
        }
        else
        {
            expectedExceptions = new[]
            { 
                typeof (ArgumentOutOfRangeException)
            };
        }
        {
            var items = GenerateItems(10);
            var list = GetList(items);
            ExceptionRecord.AssertThrows(expectedExceptions, () => list[int.MinValue] = CreateItem());
            CollectionAssert.AreEqual(items, list.ToList());

            ExceptionRecord.AssertThrows(expectedExceptions, () => list[-1] = CreateItem());
            CollectionAssert.AreEqual(items, list.ToList());
        }

        {
            var items = GenerateItems(0);
            var list = GetList(items);
            ExceptionRecord.AssertThrows(expectedExceptions, () => list[0] = CreateItem());
            CollectionAssert.AreEqual(items, list.ToList());

            if (!ExpectedIsReadOnly)
            {
                items = GenerateItems(32);
                AddRange(ref list, items);
                ExceptionRecord.AssertThrows(expectedExceptions, () => list[list.Count] = CreateItem());
                CollectionAssert.AreEqual(items, list.ToList());
            }
        }
    }


    [TestMethod]
    public void SetItemInvalidValue()
    {
        var items = GenerateItems(32);
        var list = GetList(items);
        if (IsGenericCompatibility)
        {
            foreach (object invalid in GetInvalidValues())
            {
                Type[] expectedExceptions;
                if (invalid == null)
                {
                    if (ExpectedIsReadOnly)
                    {
                        expectedExceptions = new[]
                        {
                            typeof (ArgumentNullException),
                            typeof (NotSupportedException)
                        };
                    }
                    else
                    {
                        expectedExceptions = new[]
                        {
                            typeof (ArgumentNullException)
                        };
                    }
                }
                else if (ExpectedIsReadOnly)
                {
                    expectedExceptions = new[]
                    {
                        typeof (ArgumentException),
                        typeof (NotSupportedException)
                    };
                }
                else
                {
                    expectedExceptions = new[]
                    {
                        typeof (ArgumentException)
                    };
                }

                object invalid1 = invalid;
                ExceptionRecord.AssertThrows(expectedExceptions, () => list[0] = invalid1);
                CollectionAssert.AreEqual(items, list.ToList());
            }
        }
    }



    protected abstract TList CreateList(IEnumerable<T> items);

    protected abstract T CreateItem();

    protected TList GetList(IEnumerable<T>? items)
    {
        return CreateList(items ?? Enumerable.Empty<T>());
    }

    protected T[] GenerateItems(int size)
    {
        var ret = new T[size];
        for (var i = 0; i < size; i++) 
            ret[i] = CreateItem();
        return ret;
    }



    protected static void AddRange(ref TList list, IEnumerable<T> items)
    {
        foreach (var item in items)
            list.Add(item);
    }

    protected static void InsertRange(ref TList list, T[] items)
    {
        InsertRange(ref list, 0, items, 0, items.Length);
    }

    protected static void InsertRange(ref TList list, int listStartIndex, T[] items, int startIndex, int count)
    {
        var numToInsert = items.Length - startIndex;
        if (count < numToInsert) numToInsert = count;
        for (var i = 0; i < numToInsert; i++) list.Insert(listStartIndex + i, items[startIndex + i]);
    }

    protected static void AddRange(ref TList list, T[] items, int startIndex, int count)
    {
        var numToAdd = items.Length - startIndex;
        if (count < numToAdd)
            numToAdd = count;
        for (var i = 0; i < numToAdd; i++)
            list.Add(items[startIndex + i]);
    }
}