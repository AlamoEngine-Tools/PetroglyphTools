// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.StarWarsGame.Components.Localisation.Exceptions;

/// <inheritdoc />
[ExcludeFromCodeCoverage]
public class InvalidDefaultLanguageDefinitionException : Exception
{
    /// <inheritdoc />
    public InvalidDefaultLanguageDefinitionException()
    {
    }

    /// <inheritdoc />
    public InvalidDefaultLanguageDefinitionException(string message) : base(message)
    {
    }

    /// <inheritdoc />
    public InvalidDefaultLanguageDefinitionException(string message, Exception inner) : base(message, inner)
    {
    }
}
