// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Data;
using System.Collections.Generic;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal class MegBinaryInformation(
    uint metadataSize,
    MegVersion MegVersion,
    bool encrypted,
    IEnumerable<MegDataEntryBinaryInformation> entries)
{
    public uint MetadataSize { get; } = metadataSize;

    public bool Encrypted { get; } = encrypted;

    public MegVersion MegVersion { get; } = MegVersion;

    public IEnumerable<MegDataEntryBinaryInformation> Entries { get; } = entries;
}