using System;

namespace PG.StarWarsGame.Files.MEG;

/// <summary>
/// The exception that is thrown when reading or writing a MEG file that exceeds the maximum supported size limit
/// for the specific MEG format and read/write operation.
/// </summary>
public class MegSizeException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MegSizeException"/> class with a specified error message.
    /// </summary><param name="message">The message that describes the error.</param>
    public MegSizeException(string? message) : base(message)
    {
    }
}