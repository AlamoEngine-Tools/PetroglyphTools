// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.Binary;

/// <summary>
///  The exception that is thrown when a Petroglyph binary file is corrupted.
/// </summary>
public sealed class BinaryCorruptedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryCorruptedException"/> class.
    /// </summary>
    public BinaryCorruptedException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryCorruptedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public BinaryCorruptedException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryCorruptedException"/> class
    /// with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public BinaryCorruptedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
