// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.MEG;

/// <summary>
/// An exception thrown when multiple files packaged in a MEG archive match a provided file filter. 
/// </summary>
public sealed class AmbiguousFileException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AmbiguousFileException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    public AmbiguousFileException(string message) : base(message)
    {
    }
}
