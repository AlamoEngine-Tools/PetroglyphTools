// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Represents a validator for MEG data entries used when building MEG files.
/// </summary>
/// <remarks>
/// Note: The games may consume MEG files, created by other tools than this library.
/// An <see cref="IMegDataEntryValidator"/> may be more restrictive than those other tools.
/// </remarks>
public interface IMegDataEntryValidator
{
    /// <summary>
    /// Validates the specified MEG data entry.
    /// </summary>
    /// <param name="dataEntry">The MEG data entry to validate.</param>
    /// <returns>A <see cref="MegDataEntryValidationResult"/> indicating the outcome of the validation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="dataEntry"/> is <see langword="null"/>.</exception>
    MegDataEntryValidationResult Validate(MegDataEntryBuilderInfo dataEntry);
}