// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Binary;
using PG.Commons.Services;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal sealed class KeyTableRecord : IBinary, IComparable<KeyTableRecord>
{
    private Crc32 Crc32 => ChecksumService.Instance.GetChecksum(Key, DatFileConstants.TextKeyEncoding);

    public KeyTableRecord(string key)
    {
        Key = key.Replace("\0", string.Empty);
    }

    public KeyTableRecord(byte[] bytes, long index, long stringLength)
    {
        char[] chars =
            DatFileConstants.TextKeyEncoding.GetChars(bytes, Convert.ToInt32(index), Convert.ToInt32(stringLength));
        Key = new string(chars);
    }

    public string Key { get; }

    public byte[] Bytes => DatFileConstants.TextKeyEncoding.GetBytes(Key);

    public int Size => Bytes.Length;

    public int CompareTo(KeyTableRecord? other)
    {
        return other is null ? 1 : Crc32.CompareTo(other.Crc32);
    }
}