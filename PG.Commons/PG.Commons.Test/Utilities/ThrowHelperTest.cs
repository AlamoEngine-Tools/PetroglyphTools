using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Utilities;
using PG.Testing;

namespace PG.Commons.Test.Utilities;

[TestClass]
public class ThrowHelperTest
{
    [TestMethod]
    [DataRow("")]
    [DataRow("    ")]
    [DataRow("\u00A0")]
    public void Test_ThrowIfNullOrWhiteSpace_Throws(string value)
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
    public void Test_ThrowIfNullOrWhiteSpace_DoesNotThrow(string value)
    {
        ExceptionUtilities.AssertDoesNotThrowException(() => ThrowHelper.ThrowIfNullOrWhiteSpace(value));
    }

    [TestMethod]
    [DataRow("")]
    public void Test_ThrowIfNullOrEmpty_Throws(string value)
    {
        Assert.ThrowsException<ArgumentException>(() => ThrowHelper.ThrowIfNullOrEmpty(value));
    }

    [TestMethod]
    public void Test_ThrowIfNullOrEmpty_Null_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() => ThrowHelper.ThrowIfNullOrEmpty(null));
    }

    [TestMethod]
    [DataRow("   ")]
    public void Test_ThrowIfNullOrEmpty_DoesNotThrow(string value)
    {
        ExceptionUtilities.AssertDoesNotThrowException(() => ThrowHelper.ThrowIfNullOrEmpty(value));
    }

    [TestMethod]
    public void Test_ThrowArgumentNotSortedException_DoesNotThrow()
    {
        Assert.ThrowsException<ArgumentException>(() => ThrowHelper.ThrowArgumentNotSortedException(new List<string>()));
    }
}