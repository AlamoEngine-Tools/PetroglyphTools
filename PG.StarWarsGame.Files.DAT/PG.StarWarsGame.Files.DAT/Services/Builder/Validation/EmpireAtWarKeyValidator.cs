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
        var span = key.AsSpan();

        if (span.Length == 0 || span.IsWhiteSpace())
            return false;

        if (span[0] == ' ' || span[span.Length -1] == ' ')
            return false;

        foreach (var c in span)
        {
            if ((uint)(c - '\x0020') > '\x007F' - '\x0020') // (c >= '\x0020' && c <= '\x007F')
                return false;
        }
        return true;
    }
}