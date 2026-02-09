// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

/// <summary>
/// Represents the result of validating a MEG data entry.
/// </summary>
/// <remarks>
/// This struct encapsulates the validation status and an optional validation message
/// that provides additional details about the validation result. It is used to determine
/// whether a MEG data entry complies with the rules defined by the <see cref="IMegDataEntryValidator"/>.
/// </remarks>
public readonly struct MegDataEntryValidationResult
{
    /// <summary>
    /// Represents a validation result that indicates the data entry is valid.
    /// </summary>
    public static readonly MegDataEntryValidationResult Valid = default;

    /// <summary>
    /// Gets an optional reason why the validation failed or <see langword="null"/>.
    /// </summary>
    public string? ValidationMessage { get; }
    
    /// <summary>
    /// Gets the validation status of the MEG data entry.
    /// </summary>
    /// <value>
    /// One of the <see cref="MegDataEntryValidationStatus"/> values indicating whether the data entry is valid, invalid, or invalid due to specific reasons.
    /// </value>
    public MegDataEntryValidationStatus Status { get; }

    /// <summary>
    /// Gets a value whether the validation was successful.
    /// </summary>
    public bool IsValid => Status == MegDataEntryValidationStatus.Valid;

    /// <summary>
    /// Initializes a new instance of the <see cref="MegDataEntryValidationResult"/> struct with the specified validation status and message.
    /// </summary>
    /// <param name="status">The validation status of the MEG data entry.</param>
    /// <param name="validationMessage">The validation message providing additional details about the validation result.</param>
    public MegDataEntryValidationResult(MegDataEntryValidationStatus status, string? validationMessage)
    {
        Status = status;
        ValidationMessage = validationMessage;
    }
}