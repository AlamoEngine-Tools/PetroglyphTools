// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.MEG.Binary;

internal readonly struct MegDataEntrySize(uint dataSize, uint binarySize)
{
    public uint DataSize { get; } = dataSize;

    public uint BinarySize { get; } = binarySize;
}