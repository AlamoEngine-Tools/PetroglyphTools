// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using AnakinRaW.CommonUtilities.FileSystem.Validation;

namespace PG.Commons.Utilities;

/// <summary>
/// Provides helper methods to handle and validate file names for a Petroglyph Star Wars game.
/// </summary>
public static class FileNameUtilities
{ 
    /// <summary>
    /// Checks whether a given filename is can be used for a Petroglyph Star Wars game.
    /// </summary>
    /// <remarks>
    /// A filename is considered to be invalid under the following conditions: <br/>
    ///     a) The filename is <see langword="null"/>,<br/>
    ///     b) The filename is not valid under Windows in general,<br/>
    ///     c) The filename contains a non ASCII (> 0xFF) character,<br/>
    /// </remarks>
    /// <param name="filename">The filename to check.</param>
    /// <param name="result">Detailed information status. Can be used for error message reporting.</param>
    /// <returns><see langword="true"/> when the filename is valid; <see langword="false"/> otherwise.</returns>
    public static bool IsValidFileName([NotNullWhen(true)] string? filename, out FileNameValidationResult result)
    { 
        var validator = WindowsFileNameValidator.Instance;

        var filenameSpan = filename.AsSpan();

        result = validator.IsValidFileName(filenameSpan);

        if (result != FileNameValidationResult.Valid)
            return false;

        if (ContainsInvalidPGChars(filenameSpan))
        {
            result = FileNameValidationResult.InvalidCharacter;
            return false;
        }
        
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ContainsInvalidPGChars(ReadOnlySpan<char> value)
    {
        foreach (var t in value)
            if (IsInvalidFileCharacter(t))
                return true;

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsInvalidFileCharacter(char c)
    {
        // PG only allows plain 7-bit ASCII characters, so we just check for the numerical value.
        // Check if character is in bounds [32-126] in general
        return (uint)(c - '\x0020') > '\x007F' - '\x0020'; // (c >= '\x0020' && c <= '\x007F')
    }
}
