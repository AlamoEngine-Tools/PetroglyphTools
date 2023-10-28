// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

internal sealed class ConstructingMegDataEntry : MegFileEntryDataBase<MegDataEntryOriginInfo>
{
    public ConstructingMegDataEntry(MegFileDataEntry fileEntry, MegDataEntryOriginInfo originInfo) : base(fileEntry, originInfo)
    {
    }
}