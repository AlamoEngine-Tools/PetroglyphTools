// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace PG.StarWarsGame.Files.DAT.Commons.Exceptions
{
    /// <summary>
    /// An exception thrown when a contained Key-Value pair is invalid, eg. a key has no text.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public sealed class DatFileContentInvalidException : Exception
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

        public DatFileContentInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
