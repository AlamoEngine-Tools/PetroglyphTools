using AnakinRaW.CommonUtilities.Testing.Extensions;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Exceptions;

public class MegSizeExceptionTest : CommonMegTestBase
{
    [Fact]
    public void Ctor()
    { 
        var e = new MegSizeException("message");
        Assert.Exception(e, message: "message");
    }
}