﻿// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.DAT.Services.Builder;

/// <summary>
/// Represents the state of adding an entry to an <see cref="IDatBuilder"/>. 
/// </summary>
public enum AddEntryState
{
    /// <summary>
    /// The entry was successfully added.
    /// </summary>
    Added,
    /// <summary>
    /// The entry was successfully added but there are already entries with the same key.
    /// </summary>
    AddedDuplicate,
    /// <summary>
    /// The entry was not added because an entry of the key already exists.
    /// </summary>
    NotAddedDuplicate,
    /// <summary>
    /// The entry was not added because its key is invalid.
    /// </summary>
    InvalidKey,
}