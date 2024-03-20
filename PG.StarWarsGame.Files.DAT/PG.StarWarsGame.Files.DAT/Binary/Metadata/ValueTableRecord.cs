// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Binary;
using PG.Commons.Utilities;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal readonly struct ValueTableRecord : IBinary
{
    public string Value { get; }

    public byte[] Bytes => DatFileConstants.TextValueEncoding.GetBytes(Value);

    public int Size => DatFileConstants.TextKeyEncoding.GetByteCountPG(Value.Length);

    public ValueTableRecord(string? value)
    {
        if (value != null)
        {
            Value = value.Replace("\0", string.Empty);
        }
        else
        {
            Value = string.Empty;
        }
    }

    public ValueTableRecord(byte[] bytes, long index, long stringLength)
    {
        var chars =
            DatFileConstants.TextValueEncoding.GetChars(bytes, Convert.ToInt32(index),
                Convert.ToInt32(stringLength * 2));
        Value = new string(chars);
    }
}