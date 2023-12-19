using System;
using System.Runtime.CompilerServices;
using System.Text;
using PG.Commons.Utilities;

namespace PG.StarWarsGame.Files.MEG.Utilities;

internal static class MegFilePathUtilities
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string EncodeMegFilePath(string filePath, Encoding encoding)
    {
        var byteCount = encoding.GetByteCountPG(filePath.Length);
        return encoding.EncodeString(filePath, byteCount);
    }

    internal static ushort ValidateFilePathCharacterLength(string filePath)
    {
        return StringUtilities.ValidateStringCharLengthUInt16(filePath.AsSpan());
    }
}