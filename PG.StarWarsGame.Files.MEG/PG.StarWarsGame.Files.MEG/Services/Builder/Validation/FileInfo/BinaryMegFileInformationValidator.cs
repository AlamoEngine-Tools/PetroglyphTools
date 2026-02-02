// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Validates whether a <see cref="MegFileInformation"/> is compliant to the MEG specification.
/// </summary>
public class BinaryMegFileInformationValidator : IMegFileInformationValidator
{
    /// <inheritdoc />
    /// <remarks>
    /// This method performs validation checks on the specified <see cref="MegFileInformation"/>
    /// based on the binary specification of MEG files, using <paramref name="dataEntries"/> as context for the validation.
    /// </remarks>
    public MegFileInfoValidationResult Validate(MegFileInformation fileInformation, IReadOnlyCollection<MegDataEntryBuilderInfo> dataEntries)
    {
        if (fileInformation == null) 
            throw new ArgumentNullException(nameof(fileInformation));
        if (dataEntries == null)
            throw new ArgumentNullException(nameof(dataEntries));

        var isEncrypted = dataEntries.Any(e => e.Encrypted);
        var hasEncryptionData = fileInformation.HasEncryption;

        if (isEncrypted && !hasEncryptionData)
            return new MegFileInfoValidationResult(false, "Encryption data must be provided for encrypted MEG archives.");

        if (!isEncrypted && hasEncryptionData)
            return new MegFileInfoValidationResult(false, "No encryption data must be provided for non-encrypted MEG archives.");

        return ValidateCore(fileInformation, dataEntries);
    }

    /// <summary>
    /// Validates the specified MEG file information.
    /// </summary>
    /// <param name="fileInformation">The MEG file information to validate.</param>
    /// <param name="dataEntries">The collection of data entries associated with the MEG file.</param>
    /// <returns>A <see cref="MegFileInfoValidationResult"/> indicating the outcome of the validation.</returns>
    /// <remarks>
    /// This method is intended to be overridden in derived classes to provide specific validation logic
    /// for MEG file data entries. By default, it returns a valid result.
    /// </remarks>
    protected virtual MegFileInfoValidationResult ValidateCore(MegFileInformation fileInformation, IReadOnlyCollection<MegDataEntryBuilderInfo> dataEntries)
    {
        return MegFileInfoValidationResult.Valid;
    }
}