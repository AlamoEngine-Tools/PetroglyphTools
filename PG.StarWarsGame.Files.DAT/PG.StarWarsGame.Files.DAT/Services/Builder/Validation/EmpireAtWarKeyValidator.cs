// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using AnakinRaW.CommonUtilities.Extensions;
using FluentValidation;
using PG.Commons.Utilities.Validation;
using PG.StarWarsGame.Files.DAT.Binary;

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
                var encoded = DatFileConstants.TextKeyEncoding.EncodeString(key);
                if (encoded != key)
                    return false;
                return true;
            })
            .Must(key =>
            {
                if (string.IsNullOrWhiteSpace(key))
                    return false;
                if (key.StartsWith(" ") || key.EndsWith(" "))
                    return false;
                return true;
            });
    }
}