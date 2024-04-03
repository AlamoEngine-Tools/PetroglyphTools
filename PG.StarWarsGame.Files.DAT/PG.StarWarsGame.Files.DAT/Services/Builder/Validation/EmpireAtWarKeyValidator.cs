// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using FluentValidation;
using PG.Commons.Utilities;
using PG.Commons.Utilities.Validation;

namespace PG.StarWarsGame.Files.DAT.Services.Builder.Validation;

/// <summary>
/// Validates a <see cref="string"/> whether it is can be used as a DAT key.
/// </summary>
public sealed class EmpireAtWarKeyValidator : NullableAbstractValidator<string>, IDatKeyValidator
{
    /// <inheritdoc />
    protected override bool IsValueNullable => false;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmpireAtWarKeyValidator"/> class.
    /// </summary>
    public EmpireAtWarKeyValidator()
    {
        RuleFor(key => key)
            .Must(key =>
            {
                var span = key.AsSpan();
                if (span.IsWhiteSpace())
                    return false;

                if (span.StartsWith(" ".AsSpan()) || span.EndsWith(" ".AsSpan()))
                    return false;

                foreach (var c in span)
                {
                    if ((uint)(c - '\x0020') > '\x007F' - '\x0020') // (c >= '\x0020' && c <= '\x007F')
                        return false;
                }
                return true;
            });
    }
}