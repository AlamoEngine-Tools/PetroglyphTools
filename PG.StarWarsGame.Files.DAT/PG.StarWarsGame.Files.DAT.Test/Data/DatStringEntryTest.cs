using System;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Data;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Data;

public class DatStringEntryTest
{
    [Fact]
    public void Ctor()
    {
        var entry = new DatStringEntry("abc", new Crc32(1), "valueöäü😊", "def");
        Assert.Equal("abc", entry.Key);
        Assert.Equal("def", entry.OriginalKey);
        Assert.Equal("valueöäü😊", entry.Value);
        Assert.Equal(new Crc32(1), entry.Crc32);
    }

    [Fact]
    public void Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new DatStringEntry(null!, new Crc32(1), "value", "def"));

        Assert.Throws<ArgumentNullException>(() =>
            new DatStringEntry("abc", new Crc32(1), null!, "def"));

        Assert.Throws<ArgumentNullException>(() =>
            new DatStringEntry("abc", new Crc32(1), "value", null!));

        Assert.Throws<ArgumentException>(() =>
            new DatStringEntry("öäü", new Crc32(1), "value"));
    }

    [Fact]
    public void Equals_HashCode()
    {
        var entry1 = new DatStringEntry("123", new Crc32(1), "abc", "456");
        var entry2 = new DatStringEntry("123", new Crc32(2), "abc", "456");
        var entry3 = new DatStringEntry("123", new Crc32(1), "def", "456");
        var entry4 = new DatStringEntry("123", new Crc32(1), "abc", "789");
        var entry5 = new DatStringEntry("456", new Crc32(1), "abc", "789");

        Assert.True(entry1.Equals(entry4));
        Assert.Equal(entry1.GetHashCode(), entry4.GetHashCode());

        Assert.False(entry1.Equals(entry2));
        Assert.False(entry1.Equals(entry3));
        Assert.False(entry1.Equals(entry5));

        Assert.NotEqual(entry1.GetHashCode(), entry2.GetHashCode());
        Assert.NotEqual(entry1.GetHashCode(), entry3.GetHashCode());
        Assert.NotEqual(entry1.GetHashCode(), entry5.GetHashCode());
    }
}