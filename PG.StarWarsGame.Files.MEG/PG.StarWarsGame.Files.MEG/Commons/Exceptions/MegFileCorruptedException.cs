using System;
using System.Runtime.Serialization;

namespace PG.StarWarsGame.Files.MEG.Commons.Exceptions
{
    [Serializable]
    public class MegFileCorruptedException : Exception
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

        protected MegFileCorruptedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}