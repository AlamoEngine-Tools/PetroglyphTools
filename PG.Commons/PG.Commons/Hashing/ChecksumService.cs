// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Text;

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

        Span<byte> stringBytes;

        var stringSpan = value.AsSpan();

        if (stringSpan.IsEmpty)
            return new Crc32(0);

        var maxByteSize = encoding.GetMaxByteCount(stringSpan.Length);

        if (maxByteSize > 256)
        {
#if NETSTANDARD2_1_OR_GREATER || NET
            var buff = new byte[maxByteSize].AsSpan();
            var nb = encoding.GetBytes(stringSpan, buff);
            stringBytes = buff.Slice(0, nb);
#else
            stringBytes = encoding.GetBytes(value).AsSpan();
#endif
        }
        else
        {
            var buff = stackalloc byte[maxByteSize];
            fixed (char* sp = stringSpan)
            {
                var a = encoding.GetBytes(sp, stringSpan.Length, buff, maxByteSize);
                stringBytes = new Span<byte>(buff, a);
            }
        }

        Span<byte> checksum = stackalloc byte[sizeof(Crc32)];
        System.IO.Hashing.Crc32.Hash(stringBytes, checksum);
        return new Crc32(checksum);
    }
}