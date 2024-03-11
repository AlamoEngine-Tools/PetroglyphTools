using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Utilities;

namespace PG.Commons.Test.Utilities;

[TestClass]
public class EncodingExtensionsTest
{
    [TestMethod]
    public void Test_GetByteCountPG_NullArgs_Throws()
    {
        Encoding encoding = null!;
        Assert.ThrowsException<ArgumentNullException>(() => encoding.GetByteCountPG(4));
    }

    [TestMethod]
    public void Test_GetByteCountPG_NegativeCount_Throws()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => Encoding.ASCII.GetByteCountPG(-1));
    }

    [DataTestMethod]
    [DynamicData(nameof(EncodingTestData), DynamicDataSourceType.Method)]
    public void Test_GetByteCountPG(Encoding encoding, int charCount, int expectedBytesCount)
    {
        Assert.AreEqual(expectedBytesCount, encoding.GetByteCountPG(charCount));
    }

    [DataTestMethod]
    [DynamicData(nameof(NotSupportedEncodings), DynamicDataSourceType.Method)]
    public void Test_GetByteCountPG_NotSupportedEncodings_Throws(Encoding encoding)
    {
        Assert.ThrowsException<NotSupportedException>(() => encoding.GetByteCountPG(4));
    }

    private static IEnumerable<object[]> EncodingTestData()
    {
        return new[]
        {
            [Encoding.Unicode, 0, 0],
            [Encoding.Unicode, 1, 2],
            [Encoding.Unicode, 2, 4],
            [Encoding.Unicode, 3, 6],
            [Encoding.Unicode, 256, 512],

            [Encoding.ASCII, 0, 0],
            [Encoding.ASCII, 1, 1],
            [Encoding.ASCII, 2, 2],
            [Encoding.ASCII, 3, 3],
            new object[] { Encoding.ASCII, 256, 256 },
        };
    }

    private static IEnumerable<object[]> NotSupportedEncodings()
    {
        return new[]
        {
            [Encoding.BigEndianUnicode],
            [Encoding.GetEncoding(28591)], // Latin1
            [Encoding.UTF32],
            [Encoding.UTF8],
            [Encoding.UTF7],
            [new MyAsciiEncoding()],
            new object[] { new MyUnicodeEncoding() },
        };
    }

    internal sealed class MyAsciiEncoding : ASCIIEncoding;

    internal sealed class MyUnicodeEncoding : UnicodeEncoding;
}