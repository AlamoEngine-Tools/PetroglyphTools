using System;
using PG.Commons.Utilities;

namespace PG.StarWarsGame.Files.MEG.Utilities;

internal static class MegFilePathUtilities
{
    internal static ushort ValidateFilePathCharacterLength(string filePath)
    {
        return StringUtilities.ValidateStringCharLengthUInt16(filePath.AsSpan());
    }
}