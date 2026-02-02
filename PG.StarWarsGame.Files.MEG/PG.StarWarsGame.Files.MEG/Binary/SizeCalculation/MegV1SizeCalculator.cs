using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Binary.SizeCalculation;

internal sealed class MegV1SizeCalculator : MegSizeCalculator
{
    protected override uint HeaderSize => (uint)MegHeader.SizeValue;

    protected override uint GetFileTableRecordSize(MegDataEntryBuilderInfo dataEntry) => (uint)MegFileTableRecord.SizeValue;
}