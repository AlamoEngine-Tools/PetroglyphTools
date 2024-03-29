using System.Text;
using System;
using PG.Commons.Utilities;
using System.Collections.Generic;
using PG.Testing;
using Xunit;

namespace PG.Commons.Test.Utilities;
public class StringUtilitiesTest
{
    [Fact]
    public void Test_ValidateStringByteSizeUInt16_NullArgs()
    {
        Assert.Throws<ArgumentNullException>(() => StringUtilities.ValidateStringByteSizeUInt16(null!, Encoding.ASCII));
        Assert.Throws<ArgumentNullException>(() => StringUtilities.ValidateStringByteSizeUInt16("123".AsSpan(), null!));
    }

    [Fact]
    public void Test_ValidateStringByteSizeUInt16_Overflow()
    {
        Assert.Throws<ArgumentException>(() =>
            StringUtilities.ValidateStringByteSizeUInt16(new string('a', ushort.MaxValue + 1).AsSpan(), Encoding.ASCII));
        Assert.Throws<ArgumentException>(() =>
            StringUtilities.ValidateStringByteSizeUInt16(new string('a', ushort.MaxValue / 2 + 1).AsSpan(), Encoding.Unicode));
    }

    [Theory]
    [MemberData(nameof(TestData_GetByteSize))]
    public void Test_ValidateStringByteSizeUInt16_GetBytes(Encoding encoding, string input, ushort expectedBytes)
    {
        Assert.Equal(expectedBytes, StringUtilities.ValidateStringByteSizeUInt16(input.AsSpan(), encoding));
    }

    [Fact]
    public void Test_ValidateStringCharLengthUInt16_NullArgs()
    {
        Assert.Throws<ArgumentNullException>(() => StringUtilities.ValidateStringCharLengthUInt16(null!));
    }

    [Fact]
    public void Test_ValidateStringCharLengthUInt16_Overflow()
    {
        Assert.Throws<ArgumentException>(() =>
            StringUtilities.ValidateStringCharLengthUInt16(new string('a', ushort.MaxValue + 1).AsSpan()));
    }

    [Theory]
    [MemberData(nameof(TestData_GetCharCount))]
    public void Test_ValidateStringCharLengthUInt16_GetBytes(string input, ushort expectedBytes)
    {
        Assert.Equal(expectedBytes, StringUtilities.ValidateStringCharLengthUInt16(input.AsSpan()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("testö")]
    [InlineData("testÖ")]
    [InlineData("test\u00A0")]
    [InlineData("\uFFFFtest")]
    [InlineData("\u2122test")]
    public void Test_ValidateIsAsciiOnly_Throws(string? data)
    {
        if (data is null)
            Assert.Throws<ArgumentNullException>(() => StringUtilities.ValidateIsAsciiOnly(data.AsSpan()));
        else
            Assert.Throws<ArgumentException>(() => StringUtilities.ValidateIsAsciiOnly(data.AsSpan()));
    }

    [Theory]
    [InlineData("test")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\0\t\u007F\r\n")]
    public void Test_ValidateIsAsciiOnly(string data)
    {
        ExceptionUtilities.AssertDoesNotThrowException(() => StringUtilities.ValidateIsAsciiOnly(data.AsSpan()));
    }

    public static IEnumerable<object[]> TestData_GetByteSize()
    {
        return new[]
        {
            [Encoding.Unicode, "123", (ushort)6],
            [Encoding.Unicode, "😅", (ushort)4],
            [Encoding.Unicode, "\0", (ushort)2],
            [Encoding.Unicode, "", (ushort)0],
            [Encoding.Unicode, new string('a', ushort.MaxValue / 2), (ushort) (ushort.MaxValue - 1)],

            [Encoding.ASCII, "123", (ushort)3],
            [Encoding.ASCII, "😅", (ushort)2],
            [Encoding.ASCII, "\0", (ushort)1],
            [Encoding.ASCII, "", (ushort)0],
            new object[] { Encoding.ASCII, new string('a', ushort.MaxValue), ushort.MaxValue },
        };
    }

    public static IEnumerable<object[]> TestData_GetCharCount()
    {
        return new[]
        {
            ["123", (ushort)3],
            ["😅", (ushort)2],
            ["\0", (ushort)1],
            ["", (ushort)0],
            new object[] { new string('a', ushort.MaxValue), ushort.MaxValue },
        };
    }
}