// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal class MegSizeInformation(int metadataSize, IReadOnlyList<MegSizeInformation.MegDataEntrySize> entrySizes)
{
    public int MetadataSize { get; } = metadataSize;

    public MegDataEntrySize this[int i] => entrySizes[i];

    internal readonly struct MegDataEntrySize(uint dataSize, uint binarySize)
    {
        public uint DataSize { get; } = dataSize;

        public uint BinarySize { get; } = binarySize;
    }
}