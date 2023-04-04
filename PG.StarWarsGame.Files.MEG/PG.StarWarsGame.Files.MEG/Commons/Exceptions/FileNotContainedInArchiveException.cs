// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace PG.StarWarsGame.Files.MEG.Commons.Exceptions
{
    /// <summary>
    /// Exception thrown, if a file is not contained in the current archive.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public sealed class FileNotContainedInArchiveException : Exception
    {
        public FileNotContainedInArchiveException()
        {
        }

        public FileNotContainedInArchiveException(string message) : base(message)
        {
        }

        public FileNotContainedInArchiveException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public FileNotContainedInArchiveException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        
    }
}
