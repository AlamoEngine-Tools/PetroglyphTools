// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Files;
using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Represents a validator for MEG file information used when building MEG files.
/// </summary>
public interface IMegFileInformationValidator
{
    /// <summary>
    /// Validates the specified MEG file information.
    /// </summary>
    /// <param name="fileInformation">The MEG file information to validate.</param>
    /// <param name="dataEntries">The collection of data entries associated with the MEG file.</param>
    /// <returns>A <see cref="MegFileInfoValidationResult"/> indicating the outcome of the validation.</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="fileInformation"/> or <paramref name="dataEntries"/> is <see langword="null"/>.</exception>
    MegFileInfoValidationResult Validate(MegFileInformation fileInformation, IReadOnlyCollection<MegDataEntryBuilderInfo> dataEntries);
}