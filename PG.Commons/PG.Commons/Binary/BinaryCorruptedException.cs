// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.Commons.Binary;

/// <summary>
///  The exception that is thrown when a Petroglyph binary file is corrupted.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class BinaryCorruptedException : Exception
{
    /// <inheritdoc/>
    public BinaryCorruptedException()
    {
    }

    /// <inheritdoc/>
    public BinaryCorruptedException(string message) : base(message)
    {
    }

    /// <inheritdoc/>
    public BinaryCorruptedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
