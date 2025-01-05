using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Data.EntryLocations;

public class MegDataEntryLocationTest
{
    [Fact]
    public void Test_Ctor()
    {
        var location = new MegDataEntryLocation(1, 2);
        Assert.Equal(1u, location.Offset);
        Assert.Equal(2u, location.Size);
    }

    [Fact]
    public void Test_CtorEmpty()
    {
        var location = new MegDataEntryLocation();
        Assert.Equal(0u, location.Offset);
        Assert.Equal(0u, location.Size);
    }

    [Fact]
    public void Test_Default()
    {
        var location = default(MegDataEntryLocation);
        Assert.Equal(0u, location.Offset);
        Assert.Equal(0u, location.Size);
    }

    [Fact]
    public void Test_Equality_HashCode()
    {
        var location1 = new MegDataEntryLocation(1, 2);
        var location2 = new MegDataEntryLocation(1, 2);
        var location3 = default(MegDataEntryLocation);

        Assert.Equal(location1, location2);
        Assert.Equal(location1.GetHashCode(), location2.GetHashCode());

        Assert.NotEqual(location1, location3);
        Assert.NotEqual(location1, new object());
        Assert.NotEqual((object?)null, location1);
        Assert.NotEqual(location1.GetHashCode(), location3.GetHashCode());
    }
}