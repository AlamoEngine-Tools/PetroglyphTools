using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Exceptions;

public static class MegThrowHelperTest
{
    [Fact]
    public static void ThrowDataEntryExceeds4GigabyteException()
    {
        var e = Record.Exception(() => MegThrowHelper.ThrowDataEntryExceeds4GigabyteException("filePath"));
        Assert.IsType<MegEntrySizeException>(e, true);
        Assert.Equal("Entries larger than 4GB are not supported in MEG archives. File: 'filePath'", e.Message);
    }

    [Fact]
    public static void ThrowMegExceeds4GigabyteException()
    {
        var e = Record.Exception(() => MegThrowHelper.ThrowMegExceeds4GigabyteException("filePath"));
        Assert.IsType<MegSizeException>(e, true);
        Assert.Equal("MEG files larger than 4GB are not supported. File: 'filePath'", e.Message);
    }
}