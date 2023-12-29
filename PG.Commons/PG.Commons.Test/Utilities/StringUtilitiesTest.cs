using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System;
using PG.Commons.Utilities;
using System.Collections.Generic;
using PG.Testing;

namespace PG.Commons.Test.Utilities;

[TestClass]
public class StringUtilitiesTest
{
    [TestMethod]
    public void Test_ValidateStringByteSizeUInt16_NullArgs()
    {
        Assert.ThrowsException<ArgumentNullException>(() => StringUtilities.ValidateStringByteSizeUInt16(null!, Encoding.ASCII));
        Assert.ThrowsException<ArgumentNullException>(() => StringUtilities.ValidateStringByteSizeUInt16("123".AsSpan(), null!));
    }

    [TestMethod]
    public void Test_ValidateStringByteSizeUInt16_Overflow()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            StringUtilities.ValidateStringByteSizeUInt16(new string('a', ushort.MaxValue + 1).AsSpan(), Encoding.ASCII));
        Assert.ThrowsException<ArgumentException>(() =>
            StringUtilities.ValidateStringByteSizeUInt16(new string('a', ushort.MaxValue / 2 + 1).AsSpan(), Encoding.Unicode));
    }

    [DataTestMethod]
    [DynamicData(nameof(TestData_GetByteSize), DynamicDataSourceType.Method)]
    public void Test_ValidateStringByteSizeUInt16_GetBytes(Encoding encoding, string input, ushort expectedBytes)
    {
        Assert.AreEqual(expectedBytes, StringUtilities.ValidateStringByteSizeUInt16(input.AsSpan(), encoding));
    }

    [TestMethod]
    public void Test_ValidateStringCharLengthUInt16_NullArgs()
    {
        Assert.ThrowsException<ArgumentNullException>(() => StringUtilities.ValidateStringCharLengthUInt16(null!));
    }

    [TestMethod]
    public void Test_ValidateStringCharLengthUInt16_Overflow()
    {
        Assert.ThrowsException<ArgumentException>(() =>
            StringUtilities.ValidateStringCharLengthUInt16(new string('a', ushort.MaxValue + 1).AsSpan()));
    }

    [DataTestMethod]
    [DynamicData(nameof(TestData_GetCharCount), DynamicDataSourceType.Method)]
    public void Test_ValidateStringCharLengthUInt16_GetBytes(string input, ushort expectedBytes)
    {
        Assert.AreEqual(expectedBytes, StringUtilities.ValidateStringCharLengthUInt16(input.AsSpan()));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("testö")]
    [DataRow("testÖ")]
    [DataRow("test\u00A0")]
    [DataRow("\uFFFFtest")]
    [DataRow("\u2122test")]
    public void Test_ValidateIsAsciiOnly_Throws(string? data)
    {
        if (data is null)
            Assert.ThrowsException<ArgumentNullException>(() => StringUtilities.ValidateIsAsciiOnly(data.AsSpan()));
        else
            Assert.ThrowsException<ArgumentException>(() => StringUtilities.ValidateIsAsciiOnly(data.AsSpan()));
    }

    [TestMethod]
    [DataRow("test")]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("\0\t\u007F\r\n")]
    public void Test_ValidateIsAsciiOnly(string data)
    {
        ExceptionUtilities.AssertDoesNotThrowException(() => StringUtilities.ValidateIsAsciiOnly(data.AsSpan()));
    }

    private static IEnumerable<object[]> TestData_GetByteSize()
    {
        return new[]
        {
            new object[] { Encoding.Unicode, "123", (ushort)6 },
            new object[] { Encoding.Unicode, "😅", (ushort)4 },
            new object[] { Encoding.Unicode, "\0", (ushort)2 },
            new object[] { Encoding.Unicode, "", (ushort)0 },
            new object[] { Encoding.Unicode, new string('a', ushort.MaxValue / 2), (ushort) (ushort.MaxValue - 1) },

            new object[] { Encoding.ASCII, "123", (ushort)3 },
            new object[] { Encoding.ASCII, "😅", (ushort)2 },
            new object[] { Encoding.ASCII, "\0", (ushort)1 },
            new object[] { Encoding.ASCII, "", (ushort)0 },
            new object[] { Encoding.ASCII, new string('a', ushort.MaxValue), ushort.MaxValue },
        };
    }

    private static IEnumerable<object[]> TestData_GetCharCount()
    {
        return new[]
        {
            new object[] { "123", (ushort)3 },
            new object[] { "😅", (ushort)2 },
            new object[] { "\0", (ushort)1 },
            new object[] { "", (ushort)0 },
            new object[] { new string('a', ushort.MaxValue), ushort.MaxValue },
        };
    }
}