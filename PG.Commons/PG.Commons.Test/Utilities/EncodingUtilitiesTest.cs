using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Utilities;
using PG.Testing;

namespace PG.Commons.Test.Utilities;

[TestClass]
public class EncodingUtilitiesTest
{
    private void ForEachEncoding(Action<Encoding> action)
    {
        var encodings = Encoding.GetEncodings().Select(x => Encoding.GetEncoding(x.Name));
        foreach (var encoding in encodings)
        {
            try
            {
                action(encoding);
            }
            catch
            {
                Console.WriteLine($"Failed for encoding '{encoding}'");
                throw;
            }
        }
    }


    #region .NET Framework Extensions

    // Enabled for .NET Core too, to make sure both runtimes behave the same.

    [TestMethod]
    [DataRow("")]
    [DataRow("\0")]
    [DataRow("  ")]
    [DataRow("123")]
    [DataRow("üöä")]
    [DataRow("123ü")]
    [DataRow("😅")]
    public void Test_GetBytes(string data)
    {
        ForEachEncoding(encoding =>
        {
            var expectedBytes = encoding.GetBytes(data);
            var actualBytes = new byte[encoding.GetMaxByteCount(data.Length)].AsSpan();
            var n = encoding.GetBytes(data.AsSpan(), actualBytes);
            CollectionAssert.AreEqual(expectedBytes, actualBytes.Slice(0, n).ToArray());
        });
    }

    [TestMethod]
    public void Test_GetBytes_DefaultSpan()
    {
        ForEachEncoding(e =>
        {
            var bytes = new byte[] { 1, 2 };
            var n = e.GetBytes(default, bytes);

            Assert.AreEqual(0, n);
            // Check that bytes is unaltered
            CollectionAssert.AreEqual(new byte[] { 1, 2 }, bytes);
        });
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("\0")]
    [DataRow("  ")]
    [DataRow("123")]
    [DataRow("üöä")]
    [DataRow("123ü")]
    [DataRow("😅")]
    public void Test_GetByteCount(string data)
    {
        ForEachEncoding(e =>
        {
            var actualCount = e.GetByteCount(data);
            Assert.AreEqual(actualCount, e.GetByteCount(data.AsSpan()));
        });
    }

    [TestMethod]
    public void Test_GetByteCount_DefaultSpan()
    {
        ForEachEncoding(e =>
        {
            Assert.AreEqual(0, e.GetByteCount(default(ReadOnlySpan<char>)));
        });
    }

#if NETFRAMEWORK
    [TestMethod]
    public void Test_GetString_DefaultSpan()
    {
        ForEachEncoding(e =>
        {
            // Force compiler to use EncodingUtilities instead of implicit casting to byte[]
            Assert.AreEqual(string.Empty, EncodingUtilities.GetString(e, default(ReadOnlySpan<byte>)));
        });
    }

    [TestMethod]
    public void Test_GetString()
    {
        // Force compiler to use EncodingUtilities instead of implicit casting to byte[]
        Assert.AreEqual("\0", EncodingUtilities.GetString(Encoding.ASCII, new byte[] { 00 }.AsSpan()));
        Assert.AreEqual("012", EncodingUtilities.GetString(Encoding.ASCII, "012"u8.ToArray().AsSpan()));
        Assert.AreEqual("?", EncodingUtilities.GetString(Encoding.ASCII, new byte[] { 255 }.AsSpan()));
    }
#endif



    #endregion

    #region EncodeString

    [TestMethod]
    public void Test__EncodeString_NullArgs_Throws()
    {
        Encoding encoding = null!;
        Assert.ThrowsException<ArgumentNullException>(() => encoding.EncodeString(""));
        Assert.ThrowsException<ArgumentNullException>(() => encoding.EncodeString("", 0));

        ForEachEncoding(e =>
        {
            Assert.ThrowsException<ArgumentNullException>(() => e.EncodeString((string)null!));
            Assert.ThrowsException<ArgumentNullException>(() => e.EncodeString((string)null!, 0));
        });
    }

    [TestMethod]
    public void Test__EncodeString_NegativeCount_Throws()
    {
        ForEachEncoding(e =>
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => e.EncodeString("123", -1));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => e.EncodeString("123", int.MinValue));

        });
    }

    [TestMethod]
    public void Test__EncodeString_DefaultSpan()
    {
        var encodings = Encoding.GetEncodings().Select(x => Encoding.GetEncoding(x.Name));

        foreach (var encoding in encodings)
        {
            Assert.AreEqual(string.Empty, encoding.EncodeString(default(ReadOnlySpan<char>)));
            Assert.AreEqual(string.Empty, encoding.EncodeString((default(ReadOnlySpan<char>)), 5));
        }
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
        ExceptionUtilities.AssertThrows([typeof(ArgumentException), typeof(ArgumentNullException)],
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
        ForEachEncoding(e =>
        {
            var actualCount = e.GetByteCount("123");
            Assert.ThrowsException<ArgumentException>(() => e.EncodeString("123", actualCount - 1));
        });
    }

    #endregion

    #region GetBytesReadOnly

    [TestMethod]
    public void Test_GetBytesReadOnly_DefaultSpan()
    {
        ForEachEncoding(e =>
        {
            var bufferBytes = new byte[] { 1, 2 };

            var bytes = e.GetBytesReadOnly(default, bufferBytes);

            Assert.IsTrue(bytes.IsEmpty);
            // Check that bytes is unaltered
            CollectionAssert.AreEqual(new byte[] { 1, 2 }, bufferBytes);
        });
    }

    [TestMethod]
    [DataRow("", DisplayName = "Empty")]
    [DataRow("\0", DisplayName = "ASCII NUL")]
    [DataRow("  ")]
    [DataRow("123")]
    [DataRow("üöä")]
    [DataRow("123ü")]
    [DataRow("😅")]
    public void Test_GetBytesReadOnly(string? input)
    {
        ForEachEncoding(e =>
        {
            var expectedBytes = e.GetBytes(input);

            var maxByteCount = input is null ? 5 : e.GetByteCount(input) + 5;
            var buffer = new byte[maxByteCount].AsSpan();
            var stringBytes = e.GetBytesReadOnly(input.AsSpan(), buffer);
            CollectionAssert.AreEqual(expectedBytes, stringBytes.ToArray());
        });
    }

    [TestMethod]
    public void Test_GetBytesReadOnly_BufferMutationChangesResult()
    {
        var encoding = Encoding.ASCII;
        Span<byte> buffer = stackalloc byte[10];
        var roSpan = encoding.GetBytesReadOnly("123".AsSpan(), buffer);

        CollectionAssert.AreEqual("123"u8.ToArray(), roSpan.ToArray());

        buffer.Clear();

        Assert.IsTrue(roSpan.ToArray().All(x => x == 0));
    }

    #endregion

    #region GetByteCountPG

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

    #endregion

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