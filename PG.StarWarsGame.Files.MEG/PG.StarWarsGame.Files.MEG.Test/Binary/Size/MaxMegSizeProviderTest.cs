using System;
using PG.StarWarsGame.Files.MEG.Binary.Size;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Size;

public class MaxMegSizeProviderTest
{
    [Fact]
    public void GetMegMaxSize_Binary_ReturnsPinnedSizes()
    {
        var sizes = MaxMegSizeProvider.GetMegMaxSize(MaxMegSizeMode.Binary);
        Assert.Equal(uint.MaxValue, sizes.MaxEntrySize);
        Assert.Equal(uint.MaxValue, sizes.MaxFileSize);
    }

    [Fact]
    public void GetMegMaxSize_EawFoc_ReturnsPinnedSizes()
    {
        var sizes = MaxMegSizeProvider.GetMegMaxSize(MaxMegSizeMode.EawFoc);
        Assert.Equal((uint)int.MaxValue, sizes.MaxEntrySize);
        Assert.Equal((uint)int.MaxValue, sizes.MaxFileSize);
    }

    [Fact]
    public void GetMegMaxSize_InvalidMode_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => MaxMegSizeProvider.GetMegMaxSize((MaxMegSizeMode)999));
    }
}
