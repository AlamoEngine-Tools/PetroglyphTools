// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

public sealed class MegFileDataEntryReference : MegFileEntryDataBase<IMegFile>
{
    public MegFileDataEntryReference(IMegFile megFile, MegFileDataEntry fileEntry) : base(fileEntry, megFile)
    {
    }
}