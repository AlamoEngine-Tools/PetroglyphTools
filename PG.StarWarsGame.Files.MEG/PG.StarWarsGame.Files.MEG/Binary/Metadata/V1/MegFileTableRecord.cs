// Copyright (c) Alamo Engine Tools and contributors. All rights reserved. Licensed under the MIT
// license. See LICENSE file in the project root for details.

using System;
using System.Buffers.Binary;
using PG.Commons.Hashing;

namespace PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;

internal readonly struct MegFileTableRecord : IMegFileDescriptor, IComparable<MegFileTableRecord>
{
    internal static readonly int SizeValue = sizeof(uint) * 5;

    public Crc32 Crc32 { get; }

    public uint FileOffset { get; }

    public uint FileSize { get; }

    internal uint FileNameIndex { get; }

    internal uint FileTableRecordIndex { get; }

    int IMegFileDescriptor.FileNameIndex => (int)FileNameIndex;
    int IMegFileDescriptor.Index => (int)FileTableRecordIndex;

    // For V1 this field is not part of the binary.
    public bool Encrypted => false;

    public byte[] Bytes
    {
        get
        {
            var data = new byte[Size];
            GetBytes(data);
            return data;
        }
    }

    public int Size => SizeValue;

    public MegFileTableRecord(
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

    public unsafe void GetBytes(Span<byte> bytes)
    {
        var crcArea = bytes.Slice(0);
        Crc32.GetBytes(crcArea);
        var indexArea = bytes.Slice(sizeof(Crc32));
        BinaryPrimitives.WriteUInt32LittleEndian(indexArea, FileTableRecordIndex);
        var sizeArea = bytes.Slice(sizeof(Crc32) + sizeof(uint));
        BinaryPrimitives.WriteUInt32LittleEndian(sizeArea, FileSize);
        var startArea = bytes.Slice(sizeof(Crc32) + sizeof(uint) + sizeof(uint));
        BinaryPrimitives.WriteUInt32LittleEndian(startArea, FileOffset);
        var nameArea = bytes.Slice(sizeof(Crc32) + sizeof(uint) + sizeof(uint) + sizeof(uint));
        BinaryPrimitives.WriteUInt32LittleEndian(nameArea, FileNameIndex);
    }

    public int CompareTo(MegFileTableRecord other)
    {
        // IMPORTANT: Changing the logic here also requires to update MegDataEntryIdentity.cs
        return Crc32.CompareTo(other.Crc32);
    }

    int IComparable<IMegFileDescriptor>.CompareTo(IMegFileDescriptor? other)
    {
        // IMPORTANT: Changing the logic here also requires to update MegDataEntryIdentity.cs
        return other is null ? 1 : Crc32.CompareTo(other.Crc32);
    }

    /// <summary>
    /// Determines whether one record is greater than another by its checksum.
    /// </summary>
    /// <param name="a">The first record to compare.</param>
    /// <param name="b">The second record to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="a"/> is greater than <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator >(MegFileTableRecord a, MegFileTableRecord b)
    {
        return a.Crc32 > b.Crc32;
    }

    /// <summary>
    /// Determines whether one record is less than another by its checksum.
    /// </summary>
    /// <param name="a">The first record to compare.</param>
    /// <param name="b">The second record to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="a"/> is less than <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator <(MegFileTableRecord a, MegFileTableRecord b)
    {
        return a.Crc32 < b.Crc32;
    }

    /// <summary>
    /// Determines whether one record is greater than or equal to another  by its checksum.
    /// </summary>
    /// <param name="a">The first record to compare.</param>
    /// <param name="b">The second record to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="a"/> is greater than or equal to <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator >=(MegFileTableRecord a, MegFileTableRecord b)
    {
        return a.Crc32 >= b.Crc32;
    }

    /// <summary>
    /// Determines whether one record is less than or equal to another  by its checksum.
    /// </summary>
    /// <param name="a">The first record checksum to compare.</param>
    /// <param name="b">The second record to </param>
    /// <returns><see langword="true"/> if <paramref name="a"/> is less than or equal to <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator <=(MegFileTableRecord a, MegFileTableRecord b)
    {
        return a.Crc32 <= b.Crc32;
    }
}