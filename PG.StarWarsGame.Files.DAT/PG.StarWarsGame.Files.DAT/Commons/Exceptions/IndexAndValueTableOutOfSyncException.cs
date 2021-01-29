using System;
using System.Runtime.Serialization;

namespace PG.StarWarsGame.Files.DAT.Commons.Exceptions
{
    /// <summary>
    /// An exception thrown when the <code>*.DAT</code> file's index table and value table are out of sync.
    /// </summary>
    [Serializable]
    public sealed class IndexAndValueTableOutOfSyncException : Exception
    {
        public IndexAndValueTableOutOfSyncException()
        {
        }

        public IndexAndValueTableOutOfSyncException(string message) : base(message)
        {
        }

        public IndexAndValueTableOutOfSyncException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public IndexAndValueTableOutOfSyncException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
