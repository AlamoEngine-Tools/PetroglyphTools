// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.DAT.Common.Exceptions;

/// <inheritdoc />
public class CorruptDatFileException : Exception
{
    /// <inheritdoc />
    public CorruptDatFileException()
    {
    }

    /// <inheritdoc />
    public CorruptDatFileException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public CorruptDatFileException(string message, Exception inner)
        : base(message, inner)
    {
    }
}