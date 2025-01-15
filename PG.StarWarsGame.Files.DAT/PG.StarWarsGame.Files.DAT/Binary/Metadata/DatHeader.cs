// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Buffers.Binary;
using PG.StarWarsGame.Files.Binary;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal readonly struct DatHeader : IBinary
{
    internal uint RecordCount { get; }

    public byte[] Bytes
    {
        get
        {
            var data = new byte[Size];
            GetBytes(data);
            return data;
        }
    }

    public int Size => sizeof(uint);

    internal DatHeader(uint recordCount)
    {
        if (recordCount > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(recordCount), $".DAT files with more than {int.MaxValue} records are not supported.");
        RecordCount = recordCount;
    }

    public void GetBytes(Span<byte> bytes)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(bytes, RecordCount);
    }
}