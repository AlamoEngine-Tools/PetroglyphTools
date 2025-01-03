// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Buffers.Binary;
using PG.Commons.Data;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.Binary;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal readonly struct IndexTableRecord : IHasCrc32, IComparable<IndexTableRecord>, IBinary
{
    public IndexTableRecord(Crc32 crc32, uint keyLength, uint valueLength)
    {
        if (keyLength > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(keyLength), ".DAT key length over int32.MaxValue is not supported.");
        if (valueLength > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(keyLength), ".DAT value length over int32.MaxValue is not supported.");
        Crc32 = crc32;
        KeyLength = keyLength;
        ValueLength = valueLength;
    }

    public Crc32 Crc32 { get; }

    public uint KeyLength { get; }

    public uint ValueLength { get; }

    public byte[] Bytes
    {
        get
        {
            var data = new byte[Size];
            var dataSpan = data.AsSpan();
            Crc32.GetBytes(dataSpan);
            var valueArea = dataSpan.Slice(sizeof(uint) * 1);
            BinaryPrimitives.WriteUInt32LittleEndian(valueArea, ValueLength);

            var keyArea = dataSpan.Slice(sizeof(uint) * 2);
            BinaryPrimitives.WriteUInt32LittleEndian(keyArea, KeyLength);

            return data;
        }
    }

    public int Size => sizeof(uint) * 3;

    public int CompareTo(IndexTableRecord other)
    {
        return Crc32.CompareTo(other.Crc32);
    }
}