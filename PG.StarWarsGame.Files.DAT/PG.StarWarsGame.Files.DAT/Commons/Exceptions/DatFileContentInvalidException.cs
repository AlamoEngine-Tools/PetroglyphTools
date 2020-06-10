using System;
using System.Runtime.Serialization;

namespace PG.StarWarsGame.Files.DAT.Commons.Exceptions
{
    [Serializable]
    public class DatFileContentInvalidException : Exception
    {
        public DatFileContentInvalidException()
        {
        }

        public DatFileContentInvalidException(string message) : base(message)
        {
        }

        public DatFileContentInvalidException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DatFileContentInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
