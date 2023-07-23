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
    public Crc32 GetChecksum(string s, Encoding encoding)
    {
        if (s == null)
            throw new ArgumentNullException(nameof(s));
        if (encoding == null)
            throw new ArgumentNullException(nameof(encoding));

        Span<byte> checksum = stackalloc byte[8];
        System.IO.Hashing.Crc32.Hash(encoding.GetBytes(s), checksum);
        return new Crc32(checksum);
    }
}