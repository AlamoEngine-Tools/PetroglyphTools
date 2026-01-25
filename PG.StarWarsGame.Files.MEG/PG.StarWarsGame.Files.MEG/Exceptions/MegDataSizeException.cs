namespace PG.StarWarsGame.Files.MEG;

/// <summary>
/// The exception that is thrown when reading or writing a MEG data entry that exceeds the maximum supported size limit
/// for the specific MEG format and read/write operation. 
/// </summary>
public class MegDataSizeException : MegSizeException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MegDataSizeException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MegDataSizeException(string? message) : base(message)
    {
    }
}