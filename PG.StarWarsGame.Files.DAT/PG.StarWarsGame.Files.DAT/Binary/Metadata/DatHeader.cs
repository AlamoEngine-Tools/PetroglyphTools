// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Binary;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal readonly struct DatHeader : IBinary
{
    internal uint RecordCount { get; }

    internal DatHeader(uint recordCount)
    {
        if (recordCount > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(recordCount),
                $".DAT files with more than {int.MaxValue} records are not supported.");
        RecordCount = recordCount;
    }

    public byte[] Bytes => BitConverter.GetBytes(RecordCount);

    public int Size => sizeof(uint);
}