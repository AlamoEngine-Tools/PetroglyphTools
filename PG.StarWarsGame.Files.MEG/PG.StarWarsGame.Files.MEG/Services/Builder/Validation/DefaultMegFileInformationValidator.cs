// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Linq;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Validates a specified <see cref="MegFileInformation"/> is compliant to the MEG specification.
/// </summary>
public sealed class DefaultMegFileInformationValidator : IMegFileInformationValidator
{
    /// <summary>
    /// Gets a singleton instance of the <see cref="DefaultMegFileInformationValidator"/> class.
    /// </summary>
    public static readonly DefaultMegFileInformationValidator Instance = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultMegFileInformationValidator"/> class with rules for ensuring MEG specification
    /// compliant <see cref="MegFileInformation"/>.
    /// </summary>
    /// <remarks>
    /// <see cref="MegFileInformation"/> are considered <b>not</b> to be compliant if the MEG is encrypted but the version is not V3.
    /// </remarks>
    private DefaultMegFileInformationValidator()
    {
    }

    /// <inheritdoc />
    public MegFileInfoValidationResult Validate(MegBuilderFileInformationValidationData infoValidationData)
    {
        if (infoValidationData.FileInformation is null || infoValidationData.DataEntries is null)
            return MegFileInfoValidationResult.Failed;

        var isEncrypted = infoValidationData.DataEntries.Any(e => e.Encrypted);
        var hasEncryptionData = infoValidationData.FileInformation.HasEncryption;

        if (isEncrypted && !hasEncryptionData)
        {
            return MegFileInfoValidationResult.FromFailed("Encryption data must be provided for encrypted MEG archives.");
        }

        if (!isEncrypted && hasEncryptionData)
        {
            return MegFileInfoValidationResult.FromFailed("No encryption data must be provided for non-encrypted MEG archives.");
        }

        return MegFileInfoValidationResult.Valid;
    }
}