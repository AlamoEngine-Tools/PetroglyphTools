using System;
using System.Runtime.Serialization;

namespace PG.StarWarsGame.Files.MEG.Commons.Exceptions
{
    /// <summary>
    /// An <see cref="Exception"/> that is thrown when the specified <code>*.MEG</code> archive file is corrupted.
    /// </summary>
    [Serializable]
    public sealed class MegFileCorruptedException : Exception
    {
        public MegFileCorruptedException()
        {
        }

        public MegFileCorruptedException(string message) : base(message)
        {
        }

        public MegFileCorruptedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MegFileCorruptedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}