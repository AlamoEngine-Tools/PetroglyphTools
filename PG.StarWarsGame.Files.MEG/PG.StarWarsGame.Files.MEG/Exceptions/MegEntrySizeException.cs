// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.MEG;

/// <summary>
/// The exception that is thrown when reading or writing a MEG data entry that exceeds the maximum supported size limit
/// for the specific MEG format and read/write operation. 
/// </summary>
public class MegEntrySizeException : MegSizeException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MegEntrySizeException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MegEntrySizeException(string? message) : base(message)
    {
    }
}