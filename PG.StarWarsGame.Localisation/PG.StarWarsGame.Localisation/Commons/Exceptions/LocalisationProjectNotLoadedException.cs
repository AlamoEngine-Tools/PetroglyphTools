using System;
using System.Runtime.Serialization;

namespace PG.StarWarsGame.Localisation.Commons.Exceptions
{
    [Serializable]
    public class LocalisationProjectNotLoadedException : Exception
    {
        public LocalisationProjectNotLoadedException()
        {
        }

        public LocalisationProjectNotLoadedException(string message) : base(message)
        {
        }

        public LocalisationProjectNotLoadedException(string message, Exception innerException) : base(message,
            innerException)
        {
        }

        protected LocalisationProjectNotLoadedException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}