using System;
using System.IO;
using System.Text;
using PG.Commons.Utilities;
using Xunit;

namespace PG.Commons.Test.Utilities;

public class BinaryReaderUtilitiesTest
{
    [Fact]
    public void Test_ReadString_NullArgs()
    {
        var encoding = Encoding.Default;
        var ms = new MemoryStream(Array.Empty<byte>());
        BinaryReader binaryReader = null!;
        Assert.Throws<ArgumentNullException>(() => binaryReader.ReadString(4, encoding));

        binaryReader = new BinaryReader(ms);
        Assert.Throws<ArgumentNullException>(() => binaryReader!.ReadString(4, null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("123  ")]
    [InlineData("123\0\0")]
    public void Test_ReadString_NormalCondition_Ascii(string input)
    {
        var ascii = Encoding.ASCII;
        var stringBytes = ascii.GetBytes(input);
        
        var ms = new MemoryStream(stringBytes);

        var binaryReader = new BinaryReader(ms);

        var resultString = binaryReader.ReadString(stringBytes.Length, ascii);
        Assert.Equal(input, resultString);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("123")]
    [InlineData("123  ")]
    [InlineData("123\0\0")]
    [InlineData("öäü")]
    [InlineData("😅")]
    public void Test_ReadString_NormalCondition_Unicode(string input)
    {
        var ascii = Encoding.Unicode;
        var stringBytes = ascii.GetBytes(input);

        var ms = new MemoryStream(stringBytes);

        var binaryReader = new BinaryReader(ms);

        var resultString = binaryReader.ReadString(stringBytes.Length, ascii);
        Assert.Equal(input, resultString);
    }

    [Theory]
    [InlineData("", 0, "")]
    [InlineData("123", 2, "12")]
    [InlineData("123", 1, "1")]
    [InlineData("123", 0, "")]
    [InlineData("123\0\0", 5, "123\0\0")]
    [InlineData("123  ", 5, "123  ")]
    public void Test_ReadString_Count_Ascii(string input, int count, string expected)
    {
        var unicode = Encoding.ASCII;
        var stringBytes = unicode.GetBytes(input);

        var ms = new MemoryStream(stringBytes);

        var binaryReader = new BinaryReader(ms);

        var resultString = binaryReader.ReadString(count, unicode);
        Assert.Equal(expected, resultString);
    }

    [Theory]
    [InlineData("", 0, "")]
    [InlineData("öäü", 4, "öä")]
    [InlineData("öäü", 2, "ö")]
    [InlineData("öäü", 0, "")]
    [InlineData("😅", 4, "😅")]
    [InlineData("123\0\0", 10, "123\0\0")]
    [InlineData("123  ", 10, "123  ")]
    public void Test_ReadString_Count_Unicode(string input, int count, string expected)
    {
        var unicode = Encoding.Unicode;
        var stringBytes = unicode.GetBytes(input);

        var ms = new MemoryStream(stringBytes);

        var binaryReader = new BinaryReader(ms);

        var resultString = binaryReader.ReadString(count, unicode);
        Assert.Equal(expected, resultString);
    }

    [Fact]
    public void Test_ReadString_InvalidCount_Ascii()
    {
        var ascii = Encoding.ASCII;
        var stringBytes = ascii.GetBytes("123");
        var ms = new MemoryStream(stringBytes);
        var binaryReader = new BinaryReader(ms);
        Assert.Throws<IndexOutOfRangeException>(() => binaryReader.ReadString(4, ascii));
    }

    [Fact]
    public void Test_ReadString_InvalidCount_Unicode()
    {
        var ascii = Encoding.Unicode;
        var stringBytes = ascii.GetBytes("123");
        var ms = new MemoryStream(stringBytes);
        var binaryReader = new BinaryReader(ms); 
        Assert.Throws<IndexOutOfRangeException>(() => binaryReader.ReadString(7, ascii));
    }

    [Fact]
    public void Test_ReadString_LongString_Ascii()
    {
        var ascii = Encoding.ASCII;
        var stringBytes = ascii.GetBytes(new string('a', 257));
        var ms = new MemoryStream(stringBytes);
        var binaryReader = new BinaryReader(ms);
        var resultString = binaryReader.ReadString(257, ascii);
        Assert.Equal(new string('a', 257), resultString);
    }

    [Theory]
    [InlineData("123\0\0", "123")]
    [InlineData("123  ", "123  ")]
    [InlineData("123  \0\0", "123  ")]
    public void Test_ReadString_ZeroTermination(string input, string expected)
    {
        var encoding = Encoding.ASCII;
        var stringBytes = encoding.GetBytes(input);

        var ms = new MemoryStream(stringBytes);

        var binaryReader = new BinaryReader(ms);

        var resultString = binaryReader.ReadString(input.Length, encoding, true);
        Assert.Equal(expected, resultString);
    }
}