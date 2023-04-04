// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using FluentValidation.Results;

namespace PG.Core.Data.Validation
{
    public interface IValidatable
    {
        ValidationResult Validate();

        void ValidateAndThrow();
    }
}
