using System;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.V1;
using PG.StarWarsGame.Files.MEG.Files;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Construction;

public class ConstructingMegArchiveBuilderV1Test : ConstructingMegArchiveBuilderBaseTest
{
    private protected override ConstructingMegArchiveBuilderBase CreateService()
    {
        return new ConstructingMegArchiveBuilderV1(ServiceProvider);
    }

    protected override int GetExpectedHeaderSize()
    {
        return 8;
    }

    protected override MegFileVersion GetExpectedFileVersion()
    {
        return MegFileVersion.V1;
    }

    [Fact]
    public void Ctor_NullArg_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ConstructingMegArchiveBuilderV1(null!));
    }
}