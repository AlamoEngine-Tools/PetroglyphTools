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

        var stringSpan = s.AsSpan();
        var maxByteSize = encoding.GetMaxByteCount(stringSpan.Length);
        
        Span<byte> buffer;

        // This threshold is somewhat arbitrary. It should be <= 1MB though. 
        if (maxByteSize > 256)
        {
#if NETSTANDARD2_1_OR_GREATER || NET
            // This is actually slightly faster than using encoding.GetBytes(s)
            // Using array pool is significant slower but might safe some allocations. 
            var b = new byte[maxByteSize].AsSpan();
            var bytesRead = encoding.GetBytes(stringSpan, b);
            buffer = b.Slice(0, bytesRead);
#else
            // I'm afraid this is the best performing we can get for long string in NET Framework.
            buffer = encoding.GetBytes(s).AsSpan();
#endif
        }
        else
        {
            var buff = stackalloc byte[maxByteSize];
            fixed (char* sp = s)
            {
                var bytesRead = encoding.GetBytes(sp, s.Length, buff, maxByteSize);
                buffer = new Span<byte>(buff, bytesRead);
            }
        }

        Span<byte> checksum = stackalloc byte[sizeof(Crc32)];
        System.IO.Hashing.Crc32.Hash(buffer, checksum);
        return new Crc32(checksum);
    }
}