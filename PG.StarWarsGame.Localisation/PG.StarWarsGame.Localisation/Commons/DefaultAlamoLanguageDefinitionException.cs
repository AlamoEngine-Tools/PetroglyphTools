// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Runtime.Serialization;

namespace PG.StarWarsGame.Localisation.Commons
{
    /// <inheritdoc />
    [Serializable]
    public sealed class DefaultAlamoLanguageDefinitionException : Exception
    {
        public DefaultAlamoLanguageDefinitionException()
        {
        }

        public DefaultAlamoLanguageDefinitionException(string message) : base(message)
        {
        }

        public DefaultAlamoLanguageDefinitionException(string message, Exception innerException) : base(message,
            innerException)
        {
        }

        public DefaultAlamoLanguageDefinitionException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}
