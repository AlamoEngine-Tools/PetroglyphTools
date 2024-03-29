// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Binary;
using PG.Commons.Utilities;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal readonly struct ValueTableRecord(string value) : IBinary
{
    public string Value { get; } = value ?? throw new ArgumentNullException(nameof(value));

    public byte[] Bytes => DatFileConstants.TextValueEncoding.GetBytes(Value);

    public int Size => DatFileConstants.TextValueEncoding.GetByteCountPG(Value.Length);
}