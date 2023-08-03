// Copyright (c) Alamo Engine Tools and contributors. All rights reserved. Licensed under the MIT
// license. See LICENSE file in the project root for details.

using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Binary.Shared.Metadata;
using System;

namespace PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;

internal readonly struct MegFileContentTableRecord : IMegFileDescriptor, IComparable<MegFileContentTableRecord>
{
    public Crc32 Crc32 { get; }

    public uint FileOffset { get; }

    public uint FileSize { get; }

    internal uint FileNameIndex { get; }

    internal uint FileTableRecordIndex { get; }

    int IMegFileDescriptor.FileNameIndex => (int)FileNameIndex;

    public byte[] Bytes
    {
        get
        {
            var bytes = new byte[Size];
            Buffer.BlockCopy(Crc32.GetBytes(), 0, bytes, 0, sizeof(uint));
            Buffer.BlockCopy(BitConverter.GetBytes(FileTableRecordIndex), 0, bytes, sizeof(uint) * 1, sizeof(uint));
            Buffer.BlockCopy(BitConverter.GetBytes(FileSize), 0, bytes, sizeof(uint) * 2, sizeof(uint));
            Buffer.BlockCopy(BitConverter.GetBytes(FileOffset), 0, bytes, sizeof(uint) * 3, sizeof(uint));
            Buffer.BlockCopy(BitConverter.GetBytes(FileNameIndex), 0, bytes, sizeof(uint) * 4, sizeof(uint));
            return bytes;
        }
    }

    public int Size => sizeof(uint) * 5;

    public MegFileContentTableRecord(
        Crc32 crc32,
        uint fileTableRecordIndex,
        uint fileSizeInBytes,
        uint fileStartOffsetInBytes,
        uint fileNameTableIndex)
    {
        if (fileTableRecordIndex > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(fileTableRecordIndex), ".MEG archives file number over int32.MaxValue is not supported.");
        if (fileNameTableIndex > int.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(fileNameTableIndex), ".MEG archives file number over int32.MaxValue is not supported.");
        Crc32 = crc32;
        FileTableRecordIndex = fileTableRecordIndex;
        FileSize = fileSizeInBytes;
        FileOffset = fileStartOffsetInBytes;
        FileNameIndex = fileNameTableIndex;
    }

    public int CompareTo(MegFileContentTableRecord other)
    {
        return Crc32.CompareTo(other.Crc32);
    }

    int IComparable<IMegFileDescriptor>.CompareTo(IMegFileDescriptor? other)
    {
        return other is null ? 1 : Crc32.CompareTo(other.Crc32);
    }
}