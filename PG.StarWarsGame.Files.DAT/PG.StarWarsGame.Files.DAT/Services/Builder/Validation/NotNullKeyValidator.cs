// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.DAT.Services.Builder.Validation;

/// <summary>
/// A validator that checks the passed key is not <see langword="null"/>.
/// </summary>
public sealed class NotNullKeyValidator : IDatKeyValidator
{
    /// <summary>
    /// Gets a singleton instance of the <see cref="NotNullKeyValidator"/> class.
    /// </summary>
    public static readonly NotNullKeyValidator Instance = new();

    private NotNullKeyValidator()
    {
    }

    /// <inheritdoc />
    public bool Validate(string? key)
    {
        return key is not null;
    }

    /// <inheritdoc />
    public bool Validate(ReadOnlySpan<char> key)
    {
        return key != ReadOnlySpan<char>.Empty;
    }
}