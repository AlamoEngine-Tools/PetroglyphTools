using PG.StarWarsGame.Files.MEG.Binary.Size;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Size;

public class MegV2SizeCalculatorTest : MegSizeCalculatorTestBase
{
    private protected override IMegSizeCalculator CreateCalculator() => new MegV2SizeCalculator();

    protected override uint ExpectedHeaderSize => 20u;

    protected override bool SupportsEncryption => false;

    protected override uint GetExpectedFileTableRecordSize(MegDataEntryBuilderInfo entry)
    {
        return 20u;
    }
}
