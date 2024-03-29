// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.DAT.Services;

/// <summary>
/// Specifies which merging operation of unsorted DAT models should be performed.
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