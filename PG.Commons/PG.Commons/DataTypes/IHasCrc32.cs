// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Hashing;

namespace PG.Commons.DataTypes;

/// <summary>
/// An interface representing data that has a CRC32 checksum.
/// </summary>
/// <remarks>
/// It is up to the data type to decide which properties are considered for the CRC32 checksum.
/// </remarks>
public interface IHasCrc32
{
    /// <summary>
    /// Gets the CRC32 checksum of the data type.
    /// </summary>
    Crc32 Crc32 { get; }
}