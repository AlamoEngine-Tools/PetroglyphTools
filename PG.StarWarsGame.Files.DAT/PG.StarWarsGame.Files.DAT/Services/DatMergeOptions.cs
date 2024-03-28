// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.DAT.Services;

/// <summary>
/// Specifies how merging of CRC-sorted DAT models should treat existing keys.
/// </summary>
public enum SortedDatMergeOptions
{
    /// <summary>
    /// An existing key will be kept.
    /// </summary>
    KeepExisting,
    /// <summary>
    /// An existing key will be overwritten by the new value.
    /// </summary>
    Overwrite
}

/// <summary>
/// 
/// </summary>
public enum UnsortedDatMergeOptions
{
    /// <summary>
    /// Replaces all entries index by index, regardless of matching keys.
    /// Remaining keys get appended.
    /// </summary>
    ByIndex,
    /// <summary>
    /// Overwrites as many strings as possible based on matching names.
    /// If keys do not match, overwriting will resume at the first matching key.
    /// </summary>
    Overwrite,
    /// <summary>
    /// Adds all entries at the end.
    /// </summary>
    Append,
}