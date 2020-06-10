using System;
using System.Runtime.Serialization;

namespace PG.StarWarsGame.Files.DAT.Commons.Exceptions
{
    [Serializable]
    public class IndexTableInvalidException : Exception
    {
        public IndexTableInvalidException()
        {
        }

        public IndexTableInvalidException(string message) : base(message)
        {
        }

        public IndexTableInvalidException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IndexTableInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
