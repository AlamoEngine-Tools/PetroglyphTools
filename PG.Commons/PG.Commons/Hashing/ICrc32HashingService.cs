// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using System.Text;

namespace PG.Commons.Hashing;

/// <summary>
/// Provides necessary methods to compute CRC32 checksum from strings.
/// </summary>
public interface ICrc32HashingService
{
    /// <summary>
    /// Computes the CRC-32 hash of the provided string.
    /// </summary>
    /// <param name="value">The string to get the CRC32 hash for.</param>
    /// <param name="encoding">The encoding to use for interpreting the string value.</param>
    /// <returns>The CRC32 hash of the provided string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> or <paramref name="encoding"/> is <see langword="null"/>.</exception>
    Crc32 GetCrc32(string value, Encoding encoding);

    /// <summary>
    /// Computes the CRC-32 hash of the provided data.
    /// </summary>
    /// <param name="data">The data to hash.</param>
    /// <returns>The CRC32 hash of the provided data.</returns>
    Crc32 GetCrc32(ReadOnlySpan<byte> data);

    /// <summary>
    /// Computes the CRC-32 hash of the provided stream.
    /// </summary>
    /// <param name="data">The stream to hash.</param>
    /// <returns>The CRC32 hash of the provided stream.</returns>
    Crc32 GetCrc32(Stream data);

    /// <summary>
    /// Computes the CRC-32 hash of the provided character span.
    /// </summary>
    /// <param name="value">The string to get the CRC32 hash for.</param>
    /// <param name="encoding">The encoding to use for interpreting the string value.</param>
    /// <returns>The CRC32 hash of the provided string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> or <paramref name="encoding"/> is <see langword="null"/>.</exception>
    Crc32 GetCrc32(ReadOnlySpan<char> value, Encoding encoding);

    /// <summary>
    /// Computes the CRC-32 hash of the upper-cased provided character span. Upper-casing is performed using the invariant culture. 
    /// </summary>
    /// <param name="value">The string to get the CRC32 hash for.</param>
    /// <param name="encoding">The encoding to use for interpreting the string value.</param>
    /// <returns>The CRC32 hash of the provided string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> or <paramref name="encoding"/> is <see langword="null"/>.</exception>
    Crc32 GetCrc32Upper(ReadOnlySpan<char> value, Encoding encoding);
}
