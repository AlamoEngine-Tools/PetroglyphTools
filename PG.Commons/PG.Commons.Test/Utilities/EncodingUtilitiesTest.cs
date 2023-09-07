using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Utilities;

namespace PG.Commons.Test.Utilities;

[TestClass]
public class EncodingUtilitiesTest
{
    [TestMethod]
    public void Test__GetByteCountPG_NullArgs()
    {
        Encoding encoding = null!;
        Assert.ThrowsException<ArgumentNullException>(() => encoding.GetByteCountPG(4));
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
    public void Test__GetByteCountPG_NotSupportedEncodings(Encoding encoding)
    {
        Assert.ThrowsException<NotSupportedException>(() => encoding.GetByteCountPG(4));
    }

    private static IEnumerable<object[]> NotSupportedEncodings()
    {
        return new[]
        {
            new object[] { Encoding.BigEndianUnicode },
#if NET
            new object[] { Encoding.Latin1 },
#endif
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