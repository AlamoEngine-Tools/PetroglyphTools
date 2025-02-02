// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.StarWarsGame.Files.MEG;

internal static class MegThrowHelper
{
    [DoesNotReturn]
    public static void ThrowDataEntryExceeds4GigabyteException(string? filePath)
    {
        var message = "Files larger than 4GB are not supported in MEG archives.";
        if (filePath is not null)
            message += $" File: '{filePath}'";

        throw new NotSupportedException(message);
    }

    [DoesNotReturn]
    public static void ThrowMegExceeds4GigabyteException(string? filePath)
    {
        var message = "MEG files larger than 4GB are not supported.";
        if (filePath is not null)
            message += $" File: '{filePath}'";
        throw new NotSupportedException(message);
    }
}