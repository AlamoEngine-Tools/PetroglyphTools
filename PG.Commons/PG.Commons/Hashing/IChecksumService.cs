// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Text;

namespace PG.Commons.Hashing;

/// <summary>
/// Provides necessary methods to compute CRC32 checksum from strings.
/// </summary>
public interface IChecksumService
{
    /// <summary>
    /// Computes the CRC-32 hash of the provided string.
    /// </summary>
    /// <param name="value">The string to get the CRC32 hash for.</param>
    /// <param name="encoding">The encoding to use for interpreting the string value.</param>
    /// <returns>The CRC32 hash of the provided string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> or <paramref name="encoding"/> is <see langword="null"/>.</exception>
    Crc32 GetChecksum(string value, Encoding encoding);

    /// <summary>
    /// Computes the CRC-32 hash of the provided data.
    /// </summary>
    /// <param name="data">The data to hash.</param>
    /// <returns>The CRC32 hash of the provided data.</returns>
    Crc32 GetChecksum(ReadOnlySpan<byte> data);
}
