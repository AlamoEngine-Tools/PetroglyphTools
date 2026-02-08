// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.MEG.Binary.Size;

internal static class MaxMegSizeProvider
{
    public static MaxMegSizes GetMegMaxSize(MaxMegSizeMode mode)
    {
        return mode switch
        {
            MaxMegSizeMode.Binary => new MaxMegSizes
            {
                MaxEntrySize = MegFileConstants.MegMaxEntrySize,
                MaxFileSize = MegFileConstants.MegMaxFileSize
            },
            MaxMegSizeMode.EawFoc => new MaxMegSizes
            {
                MaxEntrySize = MegFileConstants.EawMegMaxEntrySize,
                MaxFileSize = MegFileConstants.EawMegMaxFileSize
            },
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }
}