// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Binary.V1;

internal sealed class ConstructingMegArchiveBuilderV1(IServiceProvider services) : ConstructingMegArchiveBuilderBase(services)
{
    // NB: We do not override the MaxEntryFileSize, because this limitation, so far,
    // only applies to Empire at War / Forces of Corruption but not to the V1 format in general.

    protected override MegFileVersion FileVersion => MegFileVersion.V1;
}