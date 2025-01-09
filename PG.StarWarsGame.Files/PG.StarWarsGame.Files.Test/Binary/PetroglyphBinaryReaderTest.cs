using System;
using System.IO;
using System.Text;
using PG.StarWarsGame.Files.Binary;
using PG.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.Test.Utilities;

public class PetroglyphBinaryReaderTest_Ascii : PetroglyphBinaryReaderTestBase
{
    protected override Encoding GetEncoding()
    {
        return Encoding.ASCII;
    }
}

public class PetroglyphBinaryReaderTest_Latin1 : PetroglyphBinaryReaderTestBase
{
    protected override Encoding GetEncoding()
    {
        return Encoding.GetEncoding(28591);
    }
}

public class PetroglyphBinaryReaderTest_Unicode : PetroglyphBinaryReaderTestBase
{
    protected override Encoding GetEncoding()
    {
        return Encoding.Unicode;
    }
}

public abstract class PetroglyphBinaryReaderTestBase
{
    protected abstract Encoding GetEncoding();

    [Fact]
    public void Ctor_NullArgs_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new PetroglyphBinaryReader(null!, TestUtility.GetRandomBool()));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Ctor_LeavesStreamOpen(bool leaveOpen)
    {
        var stream = new MemoryStream([1, 2, 3, 4, 5, 6]);
        var reader = new PetroglyphBinaryReader(stream, leaveOpen);
        reader.Dispose();

        var e = Record.Exception(() => stream.Position = 1);

        Assert.Equal(leaveOpen, e is null);
    }

    [Fact]
    public void UnsupportedMethods_Throws()
    {
        var stream = new MemoryStream([1, 2, 3, 4, 5, 6]);
        var reader = new PetroglyphBinaryReader(stream, TestUtility.GetRandomBool());

        Assert.Throws<NotSupportedException>(() => reader.Read());
        Assert.Throws<NotSupportedException>(() => reader.Read(new char[1], 0, 0));
        Assert.Throws<NotSupportedException>(() => reader.ReadString());
        Assert.Throws<NotSupportedException>(() => reader.PeekChar());
        Assert.Throws<NotSupportedException>(() => reader.ReadChar());
        Assert.Throws<NotSupportedException>(() => reader.ReadChars(123));
#if NET
        Assert.Throws<NotSupportedException>(() => reader.Read(Span<char>.Empty));
#endif
    }

    #region ReadString

    [Fact]
    public void ReadString_NullArgs_Throws()
    {
        var ms = new MemoryStream([]);
        var reader = new PetroglyphBinaryReader(ms, false);
        Assert.Throws<ArgumentNullException>(() => reader.ReadString(null!, 4));
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("123  ")]
    [InlineData("123\0\0")]
    public void ReadString(string input)
    {
        var encoding = GetEncoding();
        var stringBytes = encoding.GetBytes(input);
        var ms = new MemoryStream(stringBytes);

        var reader = new PetroglyphBinaryReader(ms, false);

        var posBeforeRead = reader.BaseStream.Position;

        var resultString = reader.ReadString(encoding, input.Length);
        
        Assert.Equal(input, resultString);
        Assert.Equal(posBeforeRead + encoding.GetByteCount(input), reader.BaseStream.Position);
    }

    [Theory]
    [InlineData("", 0, "")]
    [InlineData("123", 2, "12")]
    [InlineData("123", 1, "1")]
    [InlineData("123", 0, "")]
    [InlineData("123\0\0", 5, "123\0\0")]
    [InlineData("123  ", 5, "123  ")]
    public void ReadString_WithExplicitCount(string input, int charCount, string expected)
    {
        var encoding = GetEncoding();
        var stringBytes = encoding.GetBytes(input);
        var ms = new MemoryStream(stringBytes);

        var reader = new PetroglyphBinaryReader(ms, false);

        var posBeforeRead = reader.BaseStream.Position;

        var resultString = reader.ReadString(encoding, charCount);
        
        Assert.Equal(expected, resultString);
        Assert.Equal(posBeforeRead + encoding.GetByteCount(input.Substring(0, charCount)), reader.BaseStream.Position);
    }

    [Fact]
    public void ReadString_InvalidCount()
    {
        const string input = "123";
        var encoding = GetEncoding();
        var stringBytes = encoding.GetBytes(input);
        var ms = new MemoryStream(stringBytes);
        
        var reader = new PetroglyphBinaryReader(ms, false);
        
        Assert.Throws<EndOfStreamException>(() => reader.ReadString(encoding, input.Length + 1));
    }

    [Fact]
    public void ReadString_LongString()
    {
        var input = new string('a', 257);
        var encoding = GetEncoding();
        var stringBytes = encoding.GetBytes(input);
        var ms = new MemoryStream(stringBytes);
        
        var reader = new PetroglyphBinaryReader(ms, false);

        var posBeforeRead = reader.BaseStream.Position;

        var resultString = reader.ReadString(encoding, input.Length);
       
        Assert.Equal(input, resultString);
        Assert.Equal(posBeforeRead + encoding.GetByteCount(input), reader.BaseStream.Position);
    }

    [Theory]
    [InlineData("123\0\0", "123")]
    [InlineData("123  \0\0", "123  ")]
    [InlineData("123\0456\0", "123")]
    public void ReadString_ZeroTermination(string input, string expected)
    {
        var encoding = GetEncoding();
        var stringBytes = encoding.GetBytes(input);
        var ms = new MemoryStream(stringBytes);

        var reader = new PetroglyphBinaryReader(ms, false);

        var posBeforeRead = reader.BaseStream.Position;

        var resultString = reader.ReadString(encoding, input.Length, true);
       
        Assert.Equal(expected, resultString);
        Assert.Equal(posBeforeRead + encoding.GetByteCount(input), reader.BaseStream.Position);
    }

    [Fact]
    public void ReadString_ZeroTerminationRequired_Throws()
    {
        const string input = "123 ";
        var encoding = GetEncoding();
        var stringBytes = encoding.GetBytes(input);
        var ms = new MemoryStream(stringBytes);

        var reader = new PetroglyphBinaryReader(ms, false);

        Assert.Throws<IOException>(() => reader.ReadString(encoding, input.Length, true));
    }

    #endregion

    #region ReadString Span

    [Fact]
    public void ReadString_Span_NullArgs_Throws()
    {
        var ms = new MemoryStream([]);
        var reader = new PetroglyphBinaryReader(ms, false);
        Assert.Throws<ArgumentNullException>(() => reader.ReadString([], null!, 0));
    }

    [Fact]
    public void ReadString_SpanNotLongEnough_Throws()
    {
        const string input = "some test";
        var encoding = GetEncoding();
        var stringBytes = encoding.GetBytes(input);
        var ms = new MemoryStream(stringBytes);

        var reader = new PetroglyphBinaryReader(ms, false);

        Assert.Throws<ArgumentException>(() => reader.ReadString(new char[input.Length - 1].AsSpan(), encoding, input.Length));
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("123  ")]
    [InlineData("123\0\0")]
    [InlineData("öäü")]
    [InlineData("😅")]
    public void ReadString_Span(string input)
    {
        var encoding = GetEncoding();
        var stringBytes = encoding.GetBytes(input);
        var expected = encoding.GetString(stringBytes);
        var ms = new MemoryStream(stringBytes);

        var reader = new PetroglyphBinaryReader(ms, false);

        var posBeforeRead = reader.BaseStream.Position;

        var buffer = new char[input.Length + 10].AsSpan();
        var n = reader.ReadString(buffer, encoding, input.Length);

        Assert.Equal(input.Length, n);
        Assert.Equal(expected, buffer.Slice(0, n).ToString());
        Assert.Equal(posBeforeRead + encoding.GetByteCount(input), reader.BaseStream.Position);
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("öäü", 2)]
    [InlineData("öäü", 1)]
    [InlineData("öäü", 0)]
    [InlineData("😅", 2)]
    [InlineData("123\0\0", 5)]
    [InlineData("123  ", 5)]
    public void ReadString_Span_WithExplicitCount(string input, int count)
    {
        var encoding = GetEncoding();
        var stringBytes = encoding.GetBytes(input);
        var expected = encoding.GetString(encoding.GetBytes(input.Substring(0, count)));
        var ms = new MemoryStream(stringBytes);

        var reader = new PetroglyphBinaryReader(ms, false);

        var posBeforeRead = reader.BaseStream.Position;

        var buffer = new char[input.Length + 10].AsSpan();
        var n = reader.ReadString(buffer, encoding, count);

        Assert.Equal(count, n);
        Assert.Equal(expected, buffer.Slice(0, n).ToString());
        Assert.Equal(posBeforeRead + encoding.GetByteCount(input.Substring(0, count)), reader.BaseStream.Position);
    }

    [Fact]
    public void ReadString_Span_InvalidCount()
    {
        const string input = "123";
        var encoding = GetEncoding();
        var stringBytes = encoding.GetBytes(input);
        var ms = new MemoryStream(stringBytes);
       
        var reader = new PetroglyphBinaryReader(ms, false);
        
        Assert.Throws<EndOfStreamException>(() =>
        {
            var buffer = new char[input.Length + 10].AsSpan();
            return reader.ReadString(buffer, encoding, input.Length + 1);
        });
    }

    [Fact]
    public void ReadString_Span_LongString()
    {
        var input = new string('a', 257);
        var encoding = GetEncoding();
        var stringBytes = encoding.GetBytes(input);
        var ms = new MemoryStream(stringBytes);
        
        var reader = new PetroglyphBinaryReader(ms, false);

        var posBeforeRead = reader.BaseStream.Position;

        var buffer = new char[input.Length + 10].AsSpan();
        var n = reader.ReadString(buffer, encoding, input.Length);

        Assert.Equal(input.Length, n);
        Assert.Equal(input, buffer.Slice(0, n).ToString());
        Assert.Equal(posBeforeRead + encoding.GetByteCount(input), reader.BaseStream.Position);
    }

    [Theory]
    [InlineData("123\0\0", "123")]
    [InlineData("123  \0\0", "123  ")]
    [InlineData("123\0456\0", "123")]
    public void ReadString_Span_ZeroTermination(string input, string expected)
    {
        var encoding = GetEncoding();
        var stringBytes = encoding.GetBytes(input);
        var ms = new MemoryStream(stringBytes);

        var reader = new PetroglyphBinaryReader(ms, false);

        var posBeforeRead = reader.BaseStream.Position;

        var buffer = new char[input.Length + 10].AsSpan();
        var n = reader.ReadString(buffer, encoding, input.Length, true);

        Assert.Equal(input.Length, n);
        Assert.Equal(expected, buffer.Slice(0, n).ToString());
        Assert.Equal(posBeforeRead + encoding.GetByteCount(input), reader.BaseStream.Position);
    }

    [Fact]
    public void ReadString_Span_ZeroTerminationRequired_Throws()
    {
        var input = "123 ";
        var encoding = GetEncoding();
        var stringBytes = encoding.GetBytes(input);

        var ms = new MemoryStream(stringBytes);

        var reader = new PetroglyphBinaryReader(ms, false);

        Assert.Throws<IOException>(() =>
        {
            // Note that this will create a char[] (un)initialized with zeros, which is \0. 
            // We should still throw, because the uninitialized values should not be used.
            var buffer = new char[input.Length + 10].AsSpan();

            // initialize explicit with \0, just to express the intention of the test
            buffer.Fill('\0');

            reader.ReadString(buffer, encoding, 4, true);
        });
    }

    #endregion
}