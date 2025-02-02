// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.DAT.Services;

/// <summary>
/// Specifies how merging of CRC-sorted DAT models should treat existing keys.
/// </summary>
public enum SortedDatMergeOptions
{
    /// <summary>
    /// An existing key is kept.
    /// </summary>
    KeepExisting,
    /// <summary>
    /// An existing key is overwritten by the new value.
    /// </summary>
    Overwrite
}