// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using FluentValidation;
using FluentValidation.Results;

namespace PG.Commons.Utilities.Validation;

/// <summary>
///     A validatable element.
/// </summary>
public interface IValidatable
{
    /// <summary>
    ///     Executes the validation and returns a <see cref="ValidationResult" />
    /// </summary>
    /// <returns></returns>
    public ValidationResult Validate();

    /// <summary>
    ///     Executes the validation and throws an <see cref="ValidationException" /> on validation failure detailing the reason
    ///     for the failure.
    /// </summary>
#if NET8_0_OR_GREATER
    public void ValidateAndThrow()
    {
        if (!Validate().IsValid)
        {
            throw new ValidationException($"The validation for {this} failed.", result.Errors);
        }
    }
#else
    public void ValidateAndThrow();
#endif
}