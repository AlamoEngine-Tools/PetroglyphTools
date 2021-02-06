using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace PG.Commons.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class XmlParseException : Exception
    {
        public XmlParseException()
        {
        }

        public XmlParseException(string message) : base(message)
        {
        }

        public XmlParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected XmlParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
