// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.MEG;

/// <summary>
/// The exception that is thrown when a MEG data entry is not found.
/// </summary>
public class MegDataEntryNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MegDataEntryNotFoundException"/> class.
    /// </summary>
    public MegDataEntryNotFoundException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegDataEntryNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public MegDataEntryNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegDataEntryNotFoundException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a <see langword="null"/> if no inner exception is specified.</param>
    public MegDataEntryNotFoundException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}