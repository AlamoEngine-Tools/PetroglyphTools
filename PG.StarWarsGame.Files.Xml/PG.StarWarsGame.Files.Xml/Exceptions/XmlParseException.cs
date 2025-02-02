// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Runtime.Serialization;

namespace PG.StarWarsGame.Files.Xml.Exceptions;

[Serializable]
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
