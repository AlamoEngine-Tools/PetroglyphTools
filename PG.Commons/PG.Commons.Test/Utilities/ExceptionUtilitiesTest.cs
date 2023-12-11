using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Utilities;
using PG.Testing;

namespace PG.Commons.Test.Utilities;

[TestClass]
public class ExceptionUtilitiesTest
{
    [TestMethod]
    [DataRow("")]
    [DataRow("    ")]
    public void TestThrowIfNullOrWhiteSpace_Throws(string value)
    {
        Assert.ThrowsException<ArgumentException>(() => ThrowHelper.ThrowIfNullOrWhiteSpace(value));
    }

    [TestMethod]
    public void Test_ThrowIfNullOrWhiteSpace_Null_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() => ThrowHelper.ThrowIfNullOrWhiteSpace(null));
    }

    [TestMethod]
    [DataRow("value")]
    [DataRow("\0160")]
    public void Test_ThrowIfNullOrWhiteSpace_DoesNotThrow(string value)
    {
        ExceptionUtilities.AssertDoesNotThrowException(() => ThrowHelper.ThrowIfNullOrWhiteSpace(value));
    }

    [TestMethod]
    public void Test_ThrowArgumentNotSortedException_DoesNotThrow()
    {
        Assert.ThrowsException<ArgumentException>(() => ThrowHelper.ThrowArgumentNotSortedException(new List<string>()));
    }
}