// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.Commons.Common.Exceptions;

/// <summary>
/// Represents an error that occurs when a PetroglyphTools library was not initialized correctly.
/// </summary>
[ExcludeFromCodeCoverage]
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

    /// <summary>
    /// Initializes a new instances of the <see cref="LibraryInitialisationException"/> class with type information
    /// about a requested service for which no implementation has been registered.
    /// </summary>
    /// <param name="serviceType">The type of the missing service.</param>
    public LibraryInitialisationException(Type serviceType) 
        : base($"No service implementation could be found for {serviceType.GetType().Name}.")
    {
    }
}