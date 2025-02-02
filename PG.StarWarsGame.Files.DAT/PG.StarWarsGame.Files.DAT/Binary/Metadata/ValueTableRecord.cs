// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.Binary;
#if NETSTANDARD2_0 || NETFRAMEWORK
using AnakinRaW.CommonUtilities.Extensions;
#endif

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal readonly struct ValueTableRecord(string value) : IBinary
{
    public string Value { get; } = value ?? throw new ArgumentNullException(nameof(value));

    public byte[] Bytes
    {
        get
        {
            var bytes = new byte[Size];
            GetBytes(bytes);
            return bytes;
        }
    }

    public int Size => DatFileConstants.TextValueEncoding.GetByteCountPG(Value.Length);

    public void GetBytes(Span<byte> bytes)
    { 
        DatFileConstants.TextValueEncoding.GetBytes(Value.AsSpan(), bytes);
    }
}