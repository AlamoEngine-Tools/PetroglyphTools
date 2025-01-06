using System;
using System.Collections.Generic;
using System.Text;
using PG.Commons.Utilities;
using Xunit;

namespace PG.Commons.Test.Utilities;

public class EncodingExtensionsTest
{
    [Fact]
    public void GetByteCountPG_NullArgs_Throws()
    {
        Encoding encoding = null!;
        Assert.Throws<ArgumentNullException>(() => encoding.GetByteCountPG(4));
    }

    [Fact]
    public void GetByteCountPG_NegativeCount_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Encoding.ASCII.GetByteCountPG(-1));
    }

    [Theory]
    [MemberData(nameof(EncodingTestData))]
    public void GetByteCountPG(Encoding encoding, int charCount, int expectedBytesCount)
    {
        Assert.Equal(expectedBytesCount, encoding.GetByteCountPG(charCount));
    }

    [Theory]
    [MemberData(nameof(NotSupportedEncodings))]
    public void GetByteCountPG_NotSupportedEncodings_Throws(Encoding encoding)
    {
        Assert.Throws<NotSupportedException>(() => encoding.GetByteCountPG(4));
    }

    public static IEnumerable<object[]> EncodingTestData()
    {
        return
        [
            [Encoding.Unicode, 0, 0],
            [Encoding.Unicode, 1, 2],
            [Encoding.Unicode, 2, 4],
            [Encoding.Unicode, 3, 6],
            [Encoding.Unicode, 256, 512],

            [Encoding.ASCII, 0, 0],
            [Encoding.ASCII, 1, 1],
            [Encoding.ASCII, 2, 2],
            [Encoding.ASCII, 3, 3],
            [Encoding.ASCII, 256, 256]
        ];
    }

    public static IEnumerable<object[]> NotSupportedEncodings()
    {
        return new[]
        {
            [Encoding.BigEndianUnicode],
            [Encoding.GetEncoding(28591)], // Latin1
            [Encoding.UTF32],
            [Encoding.UTF8],
#pragma warning disable SYSLIB0001
            [Encoding.UTF7],
#pragma warning restore SYSLIB0001
            [new MyAsciiEncoding()],
            new object[] { new MyUnicodeEncoding() },
        };
    }

    internal sealed class MyAsciiEncoding : ASCIIEncoding;

    internal sealed class MyUnicodeEncoding : UnicodeEncoding;
}