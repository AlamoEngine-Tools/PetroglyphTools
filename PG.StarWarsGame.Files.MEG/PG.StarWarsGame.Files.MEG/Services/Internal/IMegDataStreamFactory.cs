// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.IO;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Utilities;

namespace PG.StarWarsGame.Files.MEG.Services;

internal interface IMegDataStreamFactory
{
    Stream GetDataStream(MegDataEntryOriginInfo originInfo);

    MegFileDataStream GetDataStream(MegDataEntryLocationReference locationReference);
}