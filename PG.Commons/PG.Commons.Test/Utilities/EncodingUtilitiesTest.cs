using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Utilities;
using PG.Testing;

namespace PG.Commons.Test.Utilities;

[TestClass]
public class EncodingUtilitiesTest
{
    [TestMethod]
    public void Test__EncodeString_NullArgs_Throws()
    {
        Encoding encoding = null!;
        Assert.ThrowsException<ArgumentNullException>(() => encoding.EncodeString(""));
        Assert.ThrowsException<ArgumentNullException>(() => encoding.EncodeString("", 0));

        encoding = Encoding.Unicode;
        Assert.ThrowsException<ArgumentNullException>(() => encoding.EncodeString((string)null!));
        Assert.ThrowsException<ArgumentNullException>(() => encoding.EncodeString((ReadOnlySpan<char>)null!));
        Assert.ThrowsException<ArgumentNullException>(() => encoding.EncodeString((string)null!, 0));
        Assert.ThrowsException<ArgumentNullException>(() => encoding.EncodeString((ReadOnlySpan<char>)null!, 0));
    }

    [TestMethod]
    public void Test__EncodeString_NegativeCount_Throws()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Encoding.Unicode.EncodeString("123", -1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Encoding.Unicode.EncodeString("123", int.MinValue));
    }
    
    [TestMethod]
    [DataRow("", "")]
    [DataRow("\0", "\0")]
    [DataRow("  ", "  ")]
    [DataRow("123", "123")]
    [DataRow("üöä", "???")]
    [DataRow("123ü", "123?")]
    [DataRow("😅", "??")]
    public void Test__EncodeString_EncodeASCII(string input, string expected)
    {
        var encoding = Encoding.ASCII;
        var result = encoding.EncodeString(input);
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("", "")]
    [DataRow("\0", "\0")]
    [DataRow("  ", "  ")]
    [DataRow("123", "123")]
    [DataRow("üöä", "üöä")]
    [DataRow("123ü", "123ü")]
    [DataRow("😅", "😅")]
    public void Test__EncodeString_EncodeUnicode(string input, string expected)
    {
        var encoding = Encoding.Unicode;
        var result = encoding.EncodeString(input);
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("123", "123", 3)]
    [DataRow("123", "123", 4)]
    [DataRow("", "", 0)]
    [DataRow("", "", 1)]
    public void Test__EncodeString_Encode_CustomCount(string input, string expected, int count)
    {
        var encoding = Encoding.ASCII;
        var result = encoding.EncodeString(input, count);
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("1", 0)]
    [DataRow("123", 2)]
    public void Test__EncodeString_Encode_CustomCountInvalid_Throws(string input, int count)
    {
        var encoding = Encoding.ASCII;
        ExceptionUtilities.AssertThrows(new[] { typeof(ArgumentException), typeof(ArgumentNullException) },
            () => encoding.EncodeString(input, count));
    }

    [TestMethod]
    public void Test__EncodeString_Encode_LongString()
    {
        var encoding = Encoding.ASCII;
        var result = encoding.EncodeString(new string('a', 512));
        Assert.AreEqual(new string('a', 512), result);
    }

    [TestMethod]
    public void Test__EncodeString_Encode_CountError_Throws()
    {
        var encoding = Encoding.Unicode;
        Assert.ThrowsException<ArgumentException>(() => encoding.EncodeString("123", 5));
    }


    [TestMethod]
    public void Test__GetByteCountPG_NullArgs_Throws()
    {
        Encoding encoding = null!;
        Assert.ThrowsException<ArgumentNullException>(() => encoding.GetByteCountPG(4));
    }

    [TestMethod]
    public void Test__GetByteCountPG_NegativeCount_Throws()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Encoding.ASCII.GetByteCountPG(-1));
    }

    [DataTestMethod]
    [DynamicData(nameof(EncodingTestData), DynamicDataSourceType.Method)]
    public void Test__GetByteCountPG(Encoding encoding, int charCount, int expectedBytesCount)
    {
        Assert.AreEqual(expectedBytesCount, encoding.GetByteCountPG(charCount));
    }

    private static IEnumerable<object[]> EncodingTestData()
    {
        return new[]
        {
            new object[] { Encoding.Unicode, 0, 0 },
            new object[] { Encoding.Unicode, 1, 2 },
            new object[] { Encoding.Unicode, 2, 4 },
            new object[] { Encoding.Unicode, 3, 6 },
            new object[] { Encoding.Unicode, 256, 512 },

            new object[] { Encoding.ASCII, 0, 0 },
            new object[] { Encoding.ASCII, 1, 1 },
            new object[] { Encoding.ASCII, 2, 2 },
            new object[] { Encoding.ASCII, 3, 3 },
            new object[] { Encoding.ASCII, 256, 256 },
        };
    }

    [DataTestMethod]
    [DynamicData(nameof(NotSupportedEncodings), DynamicDataSourceType.Method)]
    public void Test__GetByteCountPG_NotSupportedEncodings_Throws(Encoding encoding)
    {
        Assert.ThrowsException<NotSupportedException>(() => encoding.GetByteCountPG(4));
    }

    private static IEnumerable<object[]> NotSupportedEncodings()
    {
        return new[]
        {
            new object[] { Encoding.BigEndianUnicode },
            new object[] { Encoding.GetEncoding(28591) }, // Latin1
            new object[] { Encoding.UTF32 },
            new object[] { Encoding.UTF8 },
            new object[] { Encoding.UTF7 },
            new object[] { new MyAsciiEncoding() },
            new object[] { new MyUnicodeEncoding() },
        };
    }

    internal sealed class MyAsciiEncoding : ASCIIEncoding
    {
    }

    internal sealed class MyUnicodeEncoding : UnicodeEncoding
    {
    }
}