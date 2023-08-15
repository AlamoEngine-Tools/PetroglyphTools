// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Text;

namespace PG.Commons.Services;

/// <summary>
/// A service to compute CRC32 checksum which is used throughout the Alamo Engine 
/// </summary>
public class ChecksumService : IChecksumService
{
    /// <summary>
    /// Returns a singletone instance of this class for static usage.
    /// </summary>
    public static readonly IChecksumService Instance = new ChecksumService();

    /// <inheritdoc/>
    public unsafe Crc32 GetChecksum(string s, Encoding encoding)
    {
        if (s == null)
            throw new ArgumentNullException(nameof(s));
        if (encoding == null)
            throw new ArgumentNullException(nameof(encoding));

        Span<byte> buffer;

        if (s.Length > 256)
        {
#if NET
            var stringSpan = s.AsSpan();
            var maxByteSize = encoding.GetMaxByteCount(stringSpan.Length);

            var b = new byte[maxByteSize].AsSpan();
            var nb = encoding.GetBytes(stringSpan, b);
            buffer = b.Slice(0, nb);
#else
            buffer = encoding.GetBytes(s).AsSpan();
#endif
        }
        else
        {
            var stringSpan = s.AsSpan();
            var maxByteSize = encoding.GetMaxByteCount(stringSpan.Length);

            var buff = stackalloc byte[maxByteSize];
            fixed (char* sp = &stringSpan.GetPinnableReference())
            {
                var a = encoding.GetBytes(sp, s.Length, buff, maxByteSize);
                buffer = new Span<byte>(buff, a);
            }
        }

        Span<byte> checksum = stackalloc byte[sizeof(Crc32)];
        System.IO.Hashing.Crc32.Hash(buffer, checksum);
        return new Crc32(checksum);
    }
}