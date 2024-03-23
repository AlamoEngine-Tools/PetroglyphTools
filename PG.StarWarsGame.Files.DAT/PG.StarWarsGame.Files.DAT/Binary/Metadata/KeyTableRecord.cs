// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Binary;
using PG.Commons.Utilities;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal readonly struct KeyTableRecord : IBinary
{
    public string Key { get; }

    public byte[] Bytes => DatFileConstants.TextKeyEncoding.GetBytes(Key);

    public int Size => DatFileConstants.TextKeyEncoding.GetByteCountPG(Key.Length);

    public KeyTableRecord(string key)
    {
        // While it's not recommended empty or whitespace keys are not disallowed, so we only check for null
        if (key == null) 
            throw new ArgumentNullException(nameof(key));
        StringUtilities.ValidateIsAsciiOnly(key.AsSpan());
        Key = key;
    }
}