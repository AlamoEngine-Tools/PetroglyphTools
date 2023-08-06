// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.DAT.Common.Exceptions;

/// <inheritdoc />
public class IndexAndValueTableOutOfSyncException : System.Exception
{
    /// <inheritdoc />
    public IndexAndValueTableOutOfSyncException()
    {
    }

    /// <inheritdoc />
    public IndexAndValueTableOutOfSyncException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public IndexAndValueTableOutOfSyncException(string message, System.Exception inner)
        : base(message, inner)
    {
    }
}