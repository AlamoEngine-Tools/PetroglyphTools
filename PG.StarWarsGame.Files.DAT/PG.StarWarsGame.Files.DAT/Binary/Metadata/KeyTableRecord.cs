// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using AnakinRaW.CommonUtilities;
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
        // While it's not recommended whitespace keys are not disallowed, so we only check for empty keys
        ThrowHelper.ThrowIfNullOrEmpty(key);
        StringUtilities.ValidateIsAsciiOnly(key.AsSpan());

        Key = key;
        Crc32 = checksum;
    }

    public int CompareTo(KeyTableRecord other)
    {
        return Crc32.CompareTo(other.Crc32);
    }
}