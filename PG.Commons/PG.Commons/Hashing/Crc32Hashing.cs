// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using AnakinRaW.CommonUtilities.Hashing;

namespace PG.Commons.Hashing;

/// <summary>
/// Provides static properties used for hashing services provided by this library.
/// </summary>
public static class Crc32Hashing
{
    /// <summary>
    /// Gets a <see cref="HashTypeKey"/> that represents "CRC32".
    /// </summary>
    public static readonly unsafe HashTypeKey Crc32HashKey = new("CRC32", sizeof(Crc32));
}