using System;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.V1;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Construction;

public class ConstructingMegArchiveBuilderV1Test : ConstructingMegArchiveBuilderBaseTest
{
    private protected override ConstructingMegArchiveBuilderBase CreateService(IServiceProvider serviceProvider)
    {
        return new ConstructingMegArchiveBuilderV1(serviceProvider);
    }

    protected override int GetExpectedHeaderSize()
    {
        return 8;
    }

    protected override MegFileVersion GetExpectedFileVersion()
    {
        return MegFileVersion.V1;
    }
}