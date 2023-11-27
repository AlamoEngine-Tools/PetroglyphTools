// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Text;

namespace PG.Commons.Hashing;

/// <summary>
/// Provides necessary methods to compute CRC32 checksum from strings.
/// </summary>
public interface IChecksumService
{
    /// <summary>
    /// Computes the CRC32 checksum for a given not nullable <see langword="string"/> as <see langword="uint"/>. 
    /// </summary>
    /// <param name="s">The string to get the checksum for.</param>
    /// <param name="encoding">The encoding to be used for <paramref name="s"/>.</param>
    /// <returns>The CRC32 checksum.</returns>
    Crc32 GetChecksum(string s, Encoding encoding);
}
