using PG.StarWarsGame.Files.MEG.Data;
using System;

namespace PG.StarWarsGame.Files.MEG.Binary.Size;

internal sealed class MegV2SizeCalculator : MegSizeCalculator
{
    protected override uint HeaderSize => 20u;

    protected override uint GetFileTableRecordSize(MegDataEntryBuilderInfo dataEntry)
    {
        if (dataEntry.Encrypted)
            throw new NotSupportedException("Encryption is not supported for this calculator.");
        return 20u;
    }
}