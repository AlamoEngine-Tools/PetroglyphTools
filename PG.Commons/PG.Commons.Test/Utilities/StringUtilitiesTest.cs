using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System;
using PG.Commons.Utilities;
using System.Collections.Generic;

namespace PG.Commons.Test.Utilities;

[TestClass]
public class StringUtilitiesTest
{
    [TestMethod]
    public void Test__ValidateStringByteSizeUInt16_NullArgs()
    {
        Assert.ThrowsException<ArgumentNullException>(() => StringUtilities.ValidateStringByteSizeUInt16(null!, Encoding.ASCII));
        Assert.ThrowsException<ArgumentNullException>(() => StringUtilities.ValidateStringByteSizeUInt16("123", null!));
    }

    [TestMethod]
    public void Test__ValidateStringByteSizeUInt16_Overflow()
    {
        Assert.ThrowsException<OverflowException>(() =>
            StringUtilities.ValidateStringByteSizeUInt16(new string('a', ushort.MaxValue + 1), Encoding.ASCII));
        Assert.ThrowsException<OverflowException>(() =>
            StringUtilities.ValidateStringByteSizeUInt16(new string('a', ushort.MaxValue / 2 + 1), Encoding.Unicode));
    }

    [DataTestMethod]
    [DynamicData(nameof(TestData), DynamicDataSourceType.Method)]
    public void Test__ValidateStringByteSizeUInt16_GetBytes(Encoding encoding, string input, ushort expectedBytes)
    {
        Assert.AreEqual(expectedBytes, StringUtilities.ValidateStringByteSizeUInt16(input, encoding));
    }

    private static IEnumerable<object[]> TestData()
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
}