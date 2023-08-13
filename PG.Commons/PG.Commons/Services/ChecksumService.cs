// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Buffers;
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

        Span<byte> checksum = stackalloc byte[sizeof(Crc32)];

#if NETCOREAPP2_1_OR_GREATER
        var maxByteSize = encoding.GetMaxByteCount(s.Length);
        using var pooledMemory = MemoryPool<byte>.Shared.Rent(maxByteSize);
        var poolBuf = pooledMemory.Memory.Span;
        var readBytes = encoding.GetBytes(s, poolBuf);
        var buffer = poolBuf.Slice(0, readBytes);
#else
        var buffer = encoding.GetBytes(s).AsSpan();
#endif
        System.IO.Hashing.Crc32.Hash(buffer, checksum);
        return new Crc32(checksum);
    }
}