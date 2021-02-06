using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace PG.Commons.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class ValueAlreadySetException : Exception
    {
        public ValueAlreadySetException()
        {
        }

        public ValueAlreadySetException(string message) : base(message)
        {
        }

        public ValueAlreadySetException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ValueAlreadySetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
