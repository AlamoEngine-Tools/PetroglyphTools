// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Buffers.Binary;
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