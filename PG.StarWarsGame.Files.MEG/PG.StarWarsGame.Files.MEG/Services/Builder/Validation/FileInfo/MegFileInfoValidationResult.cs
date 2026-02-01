// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Represents the result of the validation of a MEG file information.
/// </summary>
public readonly struct MegFileInfoValidationResult
{
    internal static readonly MegFileInfoValidationResult Failed = default;

    internal static readonly MegFileInfoValidationResult Valid = new(true);

    /// <summary>
    /// Gets an optional reason why the validation failed or <see langword="null"/>.
    /// </summary>
    public string? FailReason { get; }

    /// <summary>
    /// Gets a value whether the validation was successful.
    /// </summary>
    public bool IsValid { get; }

    private MegFileInfoValidationResult(string? failReason)
    {
        IsValid = false;
        FailReason = failReason;
    }

    internal MegFileInfoValidationResult(bool isValid)
    {
        IsValid = isValid;
    }

    internal static MegFileInfoValidationResult FromFailed(string? message)
    {
        return new MegFileInfoValidationResult(message);
    }
}