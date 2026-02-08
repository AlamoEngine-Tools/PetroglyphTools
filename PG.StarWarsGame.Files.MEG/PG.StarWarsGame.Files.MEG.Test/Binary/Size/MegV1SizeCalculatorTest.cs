using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Binary.Size;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Size;

public class MegV1SizeCalculatorTest : MegSizeCalculatorTestBase
{
    private protected override IMegSizeCalculator CreateCalculator() => new MegV1SizeCalculator();

    protected override uint ExpectedHeaderSize => (uint)MegHeader.SizeValue;

    protected override bool SupportsEncryption => false;

    protected override uint GetExpectedFileTableRecordSize(MegDataEntryBuilderInfo entry)
    {
        return (uint)MegFileTableRecord.SizeValue;
    }
}
