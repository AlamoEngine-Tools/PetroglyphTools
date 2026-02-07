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
                MaxEntrySize = MegFileConstants.MegMaxFileSize,
                MaxFileSize = MegFileConstants.MegMaxEntrySize
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