// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using FluentValidation.Results;

namespace PG.Commons.Validation;

/// <summary>
///     Marks a class as self-validating via <see cref="FluentValidation" />
/// </summary>
public interface IValidatable
{
    /// <summary>
    ///     Execute the validation of the current object.
    /// </summary>
    /// <returns>The <see cref="ValidationResult" /> of the validation.</returns>
    ValidationResult Validate();

    /// <summary>
    ///     Validates an object, and throws an exception if it's invalid.
    /// </summary>
    void ValidateAndThrow();
}