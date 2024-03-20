// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.DataTypes;
using PG.Commons.Hashing;
using PG.Commons.Utilities;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal readonly struct KeyTableRecord : IHasCrc32, IComparable<KeyTableRecord>
{
    public Crc32 Crc32 { get; }

    public string Key { get; }

    public byte[] Bytes => DatFileConstants.TextKeyEncoding.GetBytes(Key);

    public int Size => DatFileConstants.TextKeyEncoding.GetByteCountPG(Key.Length);

    public KeyTableRecord(string key, Crc32 checksum)
    {
        Key = key.Replace("\0", string.Empty);
        Crc32 = checksum;
    }

    public int CompareTo(KeyTableRecord other)
    {
        return Crc32.CompareTo(other.Crc32);
    }
}