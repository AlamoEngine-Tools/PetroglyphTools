// Copyright (c) Alamo Engine To.ols and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Linq;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Validates whether a <see cref="MegFileInformation"/> is compliant to the MEG specification.
/// </summary>
public sealed class DefaultMegFileInformationValidator : IMegFileInformationValidator
{
    /// <summary>
    /// Gets a singleton instance of the <see cref="DefaultMegFileInformationValidator"/> class.
    /// </summary>
    public static readonly DefaultMegFileInformationValidator Instance = new();

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
            return MegFileInfoValidationResult.FromFailed("Encryption data must be provided for encrypted MEG archives.");

        if (!isEncrypted && hasEncryptionData)
            return MegFileInfoValidationResult.FromFailed("No encryption data must be provided for non-encrypted MEG archives.");

        return MegFileInfoValidationResult.Valid;
    }
}