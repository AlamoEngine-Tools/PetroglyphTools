using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Utilities;

namespace PG.Commons.Test.Utilities;

[TestClass]
public class BinaryReaderUtilitiesTest
{
    [TestMethod]
    public void Test_ReadString_NullArgs()
    {
        var encoding = Encoding.Default;
        var ms = new MemoryStream(Array.Empty<byte>());
        BinaryReader binaryReader = null!;
        Assert.ThrowsException<ArgumentNullException>(() => binaryReader.ReadString(4, encoding));

        binaryReader = new BinaryReader(ms);
        Assert.ThrowsException<ArgumentNullException>(() => binaryReader!.ReadString(4, null!));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("123")]
    [DataRow("123  ")]
    [DataRow("123\0\0")]
    public void Test_ReadString_NormalCondition_Ascii(string input)
    {
        var ascii = Encoding.ASCII;
        var stringBytes = ascii.GetBytes(input);
        
        var ms = new MemoryStream(stringBytes);

        var binaryReader = new BinaryReader(ms);

        var resultString = binaryReader.ReadString(stringBytes.Length, ascii);
        Assert.AreEqual(input, resultString);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("123")]
    [DataRow("123")]
    [DataRow("123  ")]
    [DataRow("123\0\0")]
    [DataRow("öäü")]
    [DataRow("😅")]
    public void Test_ReadString_NormalCondition_Unicode(string input)
    {
        var ascii = Encoding.Unicode;
        var stringBytes = ascii.GetBytes(input);

        var ms = new MemoryStream(stringBytes);

        var binaryReader = new BinaryReader(ms);

        var resultString = binaryReader.ReadString(stringBytes.Length, ascii);
        Assert.AreEqual(input, resultString);
    }

    [TestMethod]
    [DataRow("", 0, "")]
    [DataRow("123", 2, "12")]
    [DataRow("123", 1, "1")]
    [DataRow("123", 0, "")]
    [DataRow("123\0\0", 5, "123\0\0")]
    [DataRow("123  ", 5, "123  ")]
    public void Test_ReadString_Count_Ascii(string input, int count, string expected)
    {
        var unicode = Encoding.ASCII;
        var stringBytes = unicode.GetBytes(input);

        var ms = new MemoryStream(stringBytes);

        var binaryReader = new BinaryReader(ms);

        var resultString = binaryReader.ReadString(count, unicode);
        Assert.AreEqual(expected, resultString);
    }

    [TestMethod]
    [DataRow("", 0, "")]
    [DataRow("öäü", 4, "öä")]
    [DataRow("öäü", 2, "ö")]
    [DataRow("öäü", 0, "")]
    [DataRow("😅", 4, "😅")]
    [DataRow("123\0\0", 10, "123\0\0")]
    [DataRow("123  ", 10, "123  ")]
    public void Test_ReadString_Count_Unicode(string input, int count, string expected)
    {
        var unicode = Encoding.Unicode;
        var stringBytes = unicode.GetBytes(input);

        var ms = new MemoryStream(stringBytes);

        var binaryReader = new BinaryReader(ms);

        var resultString = binaryReader.ReadString(count, unicode);
        Assert.AreEqual(expected, resultString);
    }

    [TestMethod]
    public void Test_ReadString_InvalidCount_Ascii()
    {
        var ascii = Encoding.ASCII;
        var stringBytes = ascii.GetBytes("123");
        var ms = new MemoryStream(stringBytes);
        var binaryReader = new BinaryReader(ms);
        Assert.ThrowsException<IndexOutOfRangeException>(() => binaryReader.ReadString(4, ascii));
    }

    [TestMethod]
    public void Test_ReadString_InvalidCount_Unicode()
    {
        var ascii = Encoding.Unicode;
        var stringBytes = ascii.GetBytes("123");
        var ms = new MemoryStream(stringBytes);
        var binaryReader = new BinaryReader(ms); 
        Assert.ThrowsException<IndexOutOfRangeException>(() => binaryReader.ReadString(7, ascii));
    }

    [TestMethod]
    public void Test_ReadString_LongString_Ascii()
    {
        var ascii = Encoding.ASCII;
        var stringBytes = ascii.GetBytes(new string('a', 257));
        var ms = new MemoryStream(stringBytes);
        var binaryReader = new BinaryReader(ms);
        var resultString = binaryReader.ReadString(257, ascii);
        Assert.AreEqual(new string('a', 257), resultString);
    }

    [TestMethod]
    [DataRow("123\0\0", "123")]
    [DataRow("123  ", "123  ")]
    [DataRow("123  \0\0", "123  ")]
    public void Test_ReadString_ZeroTermination(string input, string expected)
    {
        var encoding = Encoding.ASCII;
        var stringBytes = encoding.GetBytes(input);

        var ms = new MemoryStream(stringBytes);

        var binaryReader = new BinaryReader(ms);

        var resultString = binaryReader.ReadString(input.Length, encoding, true);
        Assert.AreEqual(expected, resultString);
    }
}