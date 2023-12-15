using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.DataTypes;

namespace PG.Commons.Test.DataTypes;

[TestClass]
public class IndexRangeTest
{
    [TestMethod]
    public void Test_Ctor()
    {
        var range = new IndexRange(1, 2);
        Assert.AreEqual(1, range.Start);
        Assert.AreEqual(2, range.Length);
    }

    [TestMethod]
    public void Test_Ctor_Throws()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new IndexRange(-1, 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new IndexRange(0, -1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new IndexRange(0, 0));
        Assert.ThrowsException<OverflowException>(() => new IndexRange(int.MaxValue, 2));
    }

    [TestMethod]
    [DataRow(0, 1, 0)]
    [DataRow(1, 1, 1)]
    [DataRow(0, 2, 1)]
    [DataRow(1, 2, 2)]
    [DataRow(1, 2, 2)]
    public void Test_End(int start, int length, int expectedEnd)
    {
        Assert.AreEqual(expectedEnd, new IndexRange(start, length).End);
    }

    [TestMethod]
    public void Test_Equals_HashCode()
    {
        var range1 = new IndexRange(1, 2);
        var range2 = new IndexRange(1, 2);

        var range3 = new IndexRange(1, 3);
        var range4 = new IndexRange(2, 2);
        
        Assert.IsTrue(range1.Equals(range2));
        Assert.IsTrue(range1.Equals((object)range2));

        Assert.IsFalse(range1.Equals(null!));
        Assert.IsFalse(range1.Equals(default));
        Assert.IsFalse(range1.Equals(range3));
        Assert.IsFalse(range1.Equals(range4));

        Assert.AreEqual(range1.GetHashCode(), range2.GetHashCode());
        Assert.AreNotEqual(range1.GetHashCode(), range3.GetHashCode());
        Assert.AreNotEqual(range1.GetHashCode(), range4.GetHashCode());

        Assert.IsTrue(range1 == range2);
        Assert.IsTrue(range1 != range3);

        Assert.IsFalse(range1 == range4);
        Assert.IsFalse(range1 != range2);
    }
}