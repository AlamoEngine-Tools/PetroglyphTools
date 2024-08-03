// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

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