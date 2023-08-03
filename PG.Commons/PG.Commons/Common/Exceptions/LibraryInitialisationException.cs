// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Commons.Common.Exceptions;

/// <inheritdoc />
public class LibraryInitialisationException : Exception
{
    /// <inheritdoc />
    public LibraryInitialisationException()
    {
    }

    /// <inheritdoc />
    public LibraryInitialisationException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public LibraryInitialisationException(string message, Exception inner)
        : base(message, inner)
    {
    }
}