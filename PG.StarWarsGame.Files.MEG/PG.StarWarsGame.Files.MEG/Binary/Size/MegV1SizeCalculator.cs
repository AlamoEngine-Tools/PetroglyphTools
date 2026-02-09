// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Binary.Size;

internal sealed class MegV1SizeCalculator : MegSizeCalculator
{
    protected override uint HeaderSize => (uint)MegHeader.SizeValue;
    
    protected override uint GetFileTableRecordSize(MegDataEntryBuilderInfo dataEntry)
    {
        if (dataEntry.Encrypted)
            throw new System.NotSupportedException("Encryption is not supported for this calculator.");
        return (uint)MegFileTableRecord.SizeValue;
    }
}