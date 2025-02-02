// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.DAT.Services;

/// <summary>
/// Informs about the merge operation that was performed on a specific key.
/// </summary>
public enum MergeOperation
{
    /// <summary>
    /// The key and its value have been added.
    /// </summary>
    Added,
    /// <summary>
    /// The key was already present and its value was overwritten.
    /// </summary>
    Overwritten
}