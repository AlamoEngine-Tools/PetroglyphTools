using System;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Binary.Size;

internal sealed class MegV1SizeCalculator : MegSizeCalculator
{
    protected override uint HeaderSize => (uint)MegHeader.SizeValue;

    protected override uint GetFileTableRecordSize(MegDataEntryBuilderInfo dataEntry)
    {
        if (dataEntry.Encrypted)
            throw new NotSupportedException("Encryption is not supported for this calculator.");
        return (uint)MegFileTableRecord.SizeValue;
    }
}