// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Runtime.Serialization;

namespace PG.StarWarsGame.Files.MTD.Commons.Exceptions;

[Serializable]
public sealed class InvalidIconNameException : Exception
{
    public InvalidIconNameException()
    {
    }

    public InvalidIconNameException(string message) : base(message)
    {
    }

    public InvalidIconNameException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public InvalidIconNameException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}