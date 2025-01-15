// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.Binary;
#if NETSTANDARD2_0 || NETFRAMEWORK
using AnakinRaW.CommonUtilities.Extensions;
#endif

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal readonly struct KeyTableRecord : IBinary
{
    public string Key { get; }

    public string OriginalKey { get; }

    public byte[] Bytes
    {
        get
        {
            var bytes = new byte[Size];
            GetBytes(bytes);
            return bytes;
        }
    }

    public int Size => DatFileConstants.TextKeyEncoding.GetByteCountPG(Key.Length);

    public KeyTableRecord(string key, string originalKey)
    {
        // While it's not recommended empty or whitespace keys are not disallowed, so we only check for null
        if (key == null) 
            throw new ArgumentNullException(nameof(key));
        if (originalKey == null) 
            throw new ArgumentNullException(nameof(originalKey));

        StringUtilities.ValidateIsAsciiOnly(key.AsSpan());
        Key = key;
        OriginalKey = originalKey;
    }

    public void GetBytes(Span<byte> bytes)
    {
        DatFileConstants.TextKeyEncoding.GetBytes(Key.AsSpan(), bytes);
    }
}