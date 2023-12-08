// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.StarWarsGame.Files.MEG;

internal static class ThrowHelper
{
    [DoesNotReturn]
    public static void ThrowFileExceeds4GigabyteException(string? filePath)
    {
        var message = "Files larger than 4GB are not supported in MEG archives.";
        if (filePath is not null)
            message += $"File: '{filePath}'";

        throw new NotSupportedException(message);
    }

    [DoesNotReturn]
    public static void ThrowArgumentNotSortedException(string paramName)
    {
        throw new ArgumentException("Data entries are not sorted.", paramName);
    }
}