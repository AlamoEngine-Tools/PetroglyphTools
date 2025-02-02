using System;
using System.Text;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Binary.Metadata;

public class ValueTableRecordTest
{
    [Fact]
    public void Ctor_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new ValueTableRecord(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("test")]
    [InlineData("testöäü")]
    [InlineData("test\\r\\n")]
    [InlineData("👌")]
    [InlineData("\u00A0")]
    public void Ctor(string input)
    {
        var record = new ValueTableRecord(input);
        Assert.Equal(input, record.Value);
        Assert.Equal(input.Length * 2, record.Size); // Value has unicode which is two times the char length.
        Assert.Equal(Encoding.Unicode.GetBytes(record.Value), record.Bytes);

        var buffer = new byte[record.Size];
        buffer.AsSpan().Fill(1);

        record.GetBytes(buffer);

        Assert.Equal(Encoding.Unicode.GetBytes(record.Value), buffer);
    }
}