// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using AnakinRaW.CommonUtilities;
using PG.Commons.Utilities;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal readonly struct KeyTableRecord
{
    public string Key { get; }

    public byte[] Bytes => DatFileConstants.TextKeyEncoding.GetBytes(Key);

    public int Size => DatFileConstants.TextKeyEncoding.GetByteCountPG(Key.Length);

    public KeyTableRecord(string key)
    {
        // While it's not recommended whitespace keys are not disallowed, so we only check for empty keys
        ThrowHelper.ThrowIfNullOrEmpty(key);
        StringUtilities.ValidateIsAsciiOnly(key.AsSpan());

        Key = key;
    }
}