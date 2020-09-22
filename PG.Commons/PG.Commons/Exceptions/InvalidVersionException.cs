using System;
using System.Runtime.Serialization;

namespace PG.Commons.Exceptions
{
    [Serializable]
    public class InvalidVersionException : Exception
    {
        public InvalidVersionException()
        {
        }

        public InvalidVersionException(string message) : base(message)
        {
        }

        public InvalidVersionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        
    }
}