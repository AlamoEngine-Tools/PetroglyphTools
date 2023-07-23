// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Binary;

namespace PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;

internal readonly struct MegHeader : IBinary
{
    internal uint NumFileNames { get; }

    internal uint NumFiles { get; }

    public byte[] Bytes
    {
        get
        {
            var bytes = new byte[Size];

            return bytes;
        }
    }

    public int Size => sizeof(uint) + sizeof(uint);

    internal MegHeader(uint numFileNames, uint numFiles)
    {
        if (numFileNames > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(numFileNames), ".MEG archives file number over int32.MaxValue is not supported.");
        if (numFiles > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(numFiles), ".MEG archives file number over int32.MaxValue is not supported.");
        NumFileNames = numFileNames;
        NumFiles = numFiles;
    }
}
