using System;
using System.Text;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Binary.Metadata;

public class KeyTableRecordTest
{
    [Fact]
    public void Ctor_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new KeyTableRecord(null!, ""));
        Assert.Throws<ArgumentNullException>(() => new KeyTableRecord("", null!));
    }

    [Theory]
    [InlineData("testöäü")]
    [InlineData("👌")]
    [InlineData("\u00A0")]
    public void Ctor_NotAscii_Throws(string input)
    {
        Assert.Throws<ArgumentException>(() => new KeyTableRecord(input, input));
    }

    [Theory]
    [InlineData("", "randomÖÄÜ")]
    [InlineData("   ", "randomÖÄÜ")]
    [InlineData("test", "randomÖÄÜ")]
    [InlineData("test\tTAB", "randomÖÄÜ")]
    [InlineData("test\\r\\n", "randomÖÄÜ")]
    public void Ctor(string input, string original)
    {
        var record = new KeyTableRecord(input, original);
        Assert.Equal(input, record.Key);
        Assert.Equal(original, record.OriginalKey);
        Assert.Equal(input.Length, record.Size); // Value has unicode which is two times the char length.
        Assert.Equal(Encoding.ASCII.GetBytes(record.Key), record.Bytes);
    }
}