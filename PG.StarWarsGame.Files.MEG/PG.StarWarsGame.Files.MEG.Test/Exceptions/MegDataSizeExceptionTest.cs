using AnakinRaW.CommonUtilities.Testing.Extensions;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Exceptions;

public class MegDataSizeExceptionTest : CommonMegTestBase
{
    [Fact]
    public void Ctor()
    {
        var e = new MegDataSizeException("message");
        Assert.Exception(e, message: "message");
    }
}