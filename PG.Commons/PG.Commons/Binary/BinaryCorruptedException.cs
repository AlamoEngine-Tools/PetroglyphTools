// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Commons.Binary;

/// <summary>
///  An <see cref="Exception" /> that is thrown when PG binary file is corrupted.
/// </summary>
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
