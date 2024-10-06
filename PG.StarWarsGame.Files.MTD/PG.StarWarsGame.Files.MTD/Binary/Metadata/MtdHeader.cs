// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Buffers.Binary;
using PG.Commons.Binary;

namespace PG.StarWarsGame.Files.MTD.Binary.Metadata;

internal readonly struct MtdHeader : IBinary
{
    internal uint Count { get; }

    internal MtdHeader(uint recordCount)
    {
        if (recordCount > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(recordCount), $".MTD files with more than {int.MaxValue} records are not supported.");
        Count = recordCount;
    }

    public byte[] Bytes
    {
        get
        {
            var data = new byte[Size];
            BinaryPrimitives.WriteUInt32LittleEndian(data, Count);
            return data;
        }
    }

    public int Size => sizeof(uint);
}