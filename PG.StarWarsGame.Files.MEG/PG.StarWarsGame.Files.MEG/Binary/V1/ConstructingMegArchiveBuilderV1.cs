// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Binary.V1;

internal sealed class ConstructingMegArchiveBuilderV1(IServiceProvider services) : ConstructingMegArchiveBuilderBase(services)
{
    protected override MegFileVersion FileVersion => MegFileVersion.V1;

    protected override int GetFileDescriptorSize(bool entryGetsEncrypted)
    {
        return Metadata.MegFileTableRecord.SizeValue;
    }

    protected override int GetHeaderSize()
    {
        return Metadata.MegHeader.SizeValue;
    }
}