// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Utilities.Validation;

namespace PG.StarWarsGame.Files.DAT.Services.Builder.Validation;

/// <summary>
/// A validator that checks the passed key is not <see langword="null"/>.
/// </summary>
public sealed class NotNullKeyValidator : NullableAbstractValidator<string>, IDatKeyValidator
{
    /// <summary>
    /// Gets a singleton instance of the <see cref="NotNullKeyValidator"/> class.
    /// </summary>
    public static readonly NotNullKeyValidator Instance = new();

    /// <inheritdoc />
    protected override bool IsValueNullable => false;
}