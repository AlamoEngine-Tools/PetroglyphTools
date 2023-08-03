// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

#region

using System;
using PG.Commons.Binary;

#endregion

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal sealed class ValueTableRecord : IBinary
{
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
        char[] chars =
            DatFileConstants.TextValueEncoding.GetChars(bytes, Convert.ToInt32(index),
                Convert.ToInt32(stringLength * 2));
        Value = new string(chars);
    }

    public string Value { get; }

    public byte[] Bytes => DatFileConstants.TextValueEncoding.GetBytes(Value);

    public int Size => Bytes.Length;
}