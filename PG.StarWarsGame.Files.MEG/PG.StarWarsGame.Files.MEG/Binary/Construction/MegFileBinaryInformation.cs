// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal class MegFileBinaryInformation(
    int metadataSize,
    MegFileVersion megFileVersion,
    bool encrypted,
    IEnumerable<MegDataEntryBinaryInformation> entries)
{
    public int MetadataSize { get; } = metadataSize;

    public bool Encrypted { get; } = encrypted;

    public MegFileVersion MegFileVersion { get; } = megFileVersion;

    public IEnumerable<MegDataEntryBinaryInformation> Entries { get; } = entries;
}