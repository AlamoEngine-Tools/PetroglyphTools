// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Binary.Size;

internal sealed class MegV2SizeCalculator : MegSizeCalculator
{
    protected override uint HeaderSize => 20u;

    protected override uint GetFileTableRecordSize(MegDataEntryBuilderInfo dataEntry)
    {
        if (dataEntry.Encrypted)
            throw new System.NotSupportedException("Encryption is not supported for this calculator.");
        return 20u;
    }
}