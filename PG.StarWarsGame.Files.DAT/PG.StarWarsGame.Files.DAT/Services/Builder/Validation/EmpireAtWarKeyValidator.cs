// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.DAT.Services.Builder.Validation;

/// <summary>
/// Validator for DAT keys using the rules of a Petroglyph Star Wars game.
/// </summary>
public sealed class EmpireAtWarKeyValidator : IDatKeyValidator
{
    /// <summary>
    /// Returns a singleton instance of the <see cref="EmpireAtWarKeyValidator"/>.
    /// </summary>
    public static readonly EmpireAtWarKeyValidator Instance = new();

    private EmpireAtWarKeyValidator()
    {
    }

    /// <inheritdoc />
    public bool Validate(string? key)
    {
        return Validate(key.AsSpan());
    }

    /// <inheritdoc />
    public bool Validate(ReadOnlySpan<char> key)
    {
        if (key.Length == 0 || key.IsWhiteSpace())
            return false;

        if (key[0] == ' ' || key[key.Length - 1] == ' ')
            return false;

        foreach (var c in key)
        {
            if ((uint)(c - '\x0020') > '\x007F' - '\x0020') // (c >= '\x0020' && c <= '\x007F')
                return false;
        }
        return true;
    }
}