// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Services;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal sealed class IndexTableRecord : IDatRecordDescriptor, IComparable<IndexTableRecord>
{
    public Crc32 Crc32 { get; }
    public uint KeyLength { get; }
    public uint ValueLength { get; }


    public IndexTableRecord(Crc32 crc32, uint keyLength, uint valueLength)
    {
        Crc32 = crc32;
        KeyLength = keyLength;
        ValueLength = valueLength;
    }

    public byte[] Bytes
    {
        get
        {
            var bytes = new byte[Size];
            Buffer.BlockCopy(Crc32.GetBytes(), 0, bytes, 0, sizeof(uint));
            Buffer.BlockCopy(BitConverter.GetBytes(ValueLength), 0, bytes, sizeof(uint) * 1, sizeof(uint));
            Buffer.BlockCopy(BitConverter.GetBytes(KeyLength), 0, bytes, sizeof(uint) * 2, sizeof(uint));
            return bytes;
        }
    }

    public int Size { get; } = sizeof(uint) * 3;

    public int CompareTo(IndexTableRecord? other)
    {
        return other is null ? 1 : Crc32.CompareTo(other.Crc32);
    }

    int IComparable<IDatRecordDescriptor>.CompareTo(IDatRecordDescriptor? other)
    {
        return other is null ? 1 : Crc32.CompareTo(other.Crc32);
    }
}