// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Represents the result of the validation of a MEG file information.
/// </summary>
public readonly struct MegFileInfoValidationResult
{
    internal static readonly MegFileInfoValidationResult Valid = new(true, null);

    /// <summary>
    /// Gets an optional reason why the validation failed or <see langword="null"/>.
    /// </summary>
    public string? FailReason { get; }

    /// <summary>
    /// Gets a value whether the validation was successful.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegFileInfoValidationResult"/> struct.
    /// </summary>
    /// <param name="valid">A value indicating whether the validation result is valid.</param>
    /// <param name="failReason">The reason for validation failure, or <see langword="null"/> if the validation is successful.</param>
    public MegFileInfoValidationResult(bool valid, string? failReason)
    {
        IsValid = valid;
        FailReason = failReason;
    }
}