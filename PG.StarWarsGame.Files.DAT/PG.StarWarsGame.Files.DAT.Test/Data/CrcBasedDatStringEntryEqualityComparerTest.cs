using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Data;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Data;

public class CrcBasedDatStringEntryEqualityComparerTest
{
    [Fact]
    public void Test_Equals()
    {
        var comparer = CrcBasedDatStringEntryEqualityComparer.Instance;

        Assert.True(comparer.Equals(new DatStringEntry(), new DatStringEntry()));
        Assert.True(comparer.Equals(
            new DatStringEntry("1", new Crc32(1), "1", "1"), 
            new DatStringEntry("1", new Crc32(1), "2", "2")));

        Assert.True(comparer.Equals(
            new DatStringEntry("1", new Crc32(1), "1", "1"),
            new DatStringEntry("2", new Crc32(1), "2", "2")));

        Assert.False(comparer.Equals(
            new DatStringEntry("1", new Crc32(1), "1", "1"),
            new DatStringEntry("1", new Crc32(2), "1", "1")));
    }

    [Fact]
    public void Test_GetHashCode()
    {
        var comparer = CrcBasedDatStringEntryEqualityComparer.Instance;

        Assert.Equal(
            comparer.GetHashCode(new DatStringEntry("1", new Crc32(1), "1", "1")),
            comparer.GetHashCode(new DatStringEntry("1", new Crc32(1), "2", "2")));

        Assert.Equal(
            comparer.GetHashCode(new DatStringEntry("1", new Crc32(1), "1", "1")),
            comparer.GetHashCode(new DatStringEntry("2", new Crc32(1), "2", "2")));

        Assert.NotEqual(
            comparer.GetHashCode(new DatStringEntry("1", new Crc32(1), "1", "1")),
            comparer.GetHashCode(new DatStringEntry("1", new Crc32(2), "2", "2")));
    }
}