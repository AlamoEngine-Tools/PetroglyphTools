// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Text;
using AnakinRaW.CommonUtilities.Extensions;

namespace PG.Commons.Hashing;

/// <summary>
/// A service to compute CRC32 checksum which is used throughout the Alamo Engine 
/// </summary>
public class ChecksumService : IChecksumService
{
    /// <inheritdoc/>
    public unsafe Crc32 GetChecksum(string value, Encoding encoding)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));
        if (encoding == null)
            throw new ArgumentNullException(nameof(encoding));

        var stringSpan = value.AsSpan();

        var maxByteSize = encoding.GetMaxByteCount(stringSpan.Length);

        var buffer = maxByteSize > 256 ? new byte[maxByteSize] : stackalloc byte[maxByteSize];

        var bytesToHash = encoding.GetBytesReadOnly(stringSpan, buffer);

        return GetChecksum(bytesToHash);
    }

    /// <inheritdoc/>
    public unsafe Crc32 GetChecksum(ReadOnlySpan<byte> data)
    {
        Span<byte> checksum = stackalloc byte[sizeof(Crc32)];
        System.IO.Hashing.Crc32.Hash(data, checksum);
        return new Crc32(checksum);
    }
}