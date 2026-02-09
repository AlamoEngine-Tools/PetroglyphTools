// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Represents the validation status of a MEG data entry.
/// </summary>
public enum MegDataEntryValidationStatus
{
    /// <summary>
    /// The entry is valid.
    /// </summary>
    Valid,
    /// <summary>
    /// The entry is invalid
    /// </summary>
    Invalid,
    /// <summary>
    /// The entry's origin does not exist.
    /// </summary>
    InvalidOriginNotFound,
    /// <summary>
    /// The entry path is not valid,
    /// </summary>
    InvalidPath,
    /// <summary>
    /// The entry is invalid, because its data size is too large.
    /// </summary>
    InvalidEntryTooLarge,
}