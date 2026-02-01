using AnakinRaW.CommonUtilities.Testing.Extensions;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Exceptions;

public class MegEntrySizeExceptionTest : CommonMegTestBase
{
    [Fact]
    public void Ctor()
    {
        var e = new MegEntrySizeException("message");
        Assert.Exception(e, message: "message");
    }
}