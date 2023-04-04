// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace PG.StarWarsGame.Files.MEG.Commons.Exceptions
{
    /// <summary>
    /// An exception thrown when multiple files packaged in the MEG archive match the provided file filter. 
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public sealed class MultipleFilesWithMatchingNameInArchiveException : Exception
    {
        public MultipleFilesWithMatchingNameInArchiveException()
        {
        }

        public MultipleFilesWithMatchingNameInArchiveException(string message) : base(message)
        {
        }

        public MultipleFilesWithMatchingNameInArchiveException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MultipleFilesWithMatchingNameInArchiveException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        
    }
}
