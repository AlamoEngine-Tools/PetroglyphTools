// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.DAT.Services.Builder.Validation;

/// <summary>
/// Validates a <see cref="string"/> whether it is can be used as a DAT key.
/// </summary>
public sealed class EmpireAtWarKeyValidator : IDatKeyValidator
{ 
    /// <summary>
    /// Initializes a new instance of the <see cref="EmpireAtWarKeyValidator"/> class.
    /// </summary>
    public EmpireAtWarKeyValidator()
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