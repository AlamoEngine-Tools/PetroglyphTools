using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Binary.Size;

internal sealed class MegV2SizeCalculator : MegSizeCalculator
{
    protected override uint HeaderSize => 20u;

    protected override uint GetFileTableRecordSize(MegDataEntryBuilderInfo dataEntry) => 20u;
}