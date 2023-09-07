// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace PG.Commons.Utilities;

/// <summary>
/// Provides helper methods to handle and validate file names for a Petroglyph Star Wars game.
/// </summary>
public static class FileNameUtilities
{
    // From .NET Path.Windows.cs
    // Disabled all chars [0-31] cause we checked for them already when this is used.
    private static readonly char[] InvalidFileNameChars = {
        '\"', '<', '>', '|', ':', '*', '?', '\\', '/', '\0'
        //(char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
        //(char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
        //(char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
        //(char)31,
    };

    /// <summary>
    /// Checks whether a given filename is can be used for a Petroglyph Star Wars game.
    /// </summary>
    /// <remarks>
    /// A filename is considered to be invalid under the following conditions: <br/>
    ///     a) The filename is <see langword="null"/>,<br/>
    ///     b) The filename is empty or only contains whitespace,<br/>
    ///     c) The filename contains a non ASCII (> 0xFF) character,<br/>
    ///     d) The filename contains a character that is illegal for Windows file names, such as TAB, or path separators, etc.
    /// </remarks>
    /// <param name="filename">The filename to check.</param>
    /// <param name="reason">A reason message why the check failed or <see langword="null"/> if the check passed.</param>
    /// <returns><see langword="true"/> when the filename is valid; <see langword="false"/> otherwise.</returns>
    public static bool IsValidFileName(string? filename, [NotNullWhen(false)] out string? reason)
    {
        reason = null;

        if (filename is null)
        {
            reason = "File name must not be null.";
            return false;
        }

        var filenameSpan = filename.AsSpan();

        // This also covers empty strings
        if (IsNullOrWhiteSpaceFast(filenameSpan))
        {
            reason = "File name must not be null or empty or only contains space characters.";
            return false;
        }

        if (ContainsInvalidChars(filenameSpan, out var invalidChar))
        {
            reason = $"File name contains invalid characters: '{invalidChar}'.";
            return false;
        }

        return true;
    }

    private static bool ContainsInvalidChars(ReadOnlySpan<char> value, out char invalidChar)
    {
        invalidChar = default;

        foreach (var t in value)
        {
            if (IsInvalidFileCharacter(t))
            {
                invalidChar = t;
                return true;
            }
        }

        return false;
    }

    private static bool IsInvalidFileCharacter(char c)
    {
        // Check if character is in bounds [32-126] in general
        if ((uint)(c - '\x0020') > '\x007F' - '\x0020')  // (c >= '\x0020' && c <= '\x007F')
            return true;

        // Additional check for invalid Windows file name characters
        if (InvalidFileNameChars.Contains(c))
            return true;

        return false;
    }


    private static bool IsNullOrWhiteSpaceFast(ReadOnlySpan<char> value)
    {
        foreach (var t in value)
        {
            // We can skip any other space like characters, such as TAB, \u00A0, etc.
            // only because they are illegal characters for Petroglyph file name anyway and we check for them at some other point.
            // Thus we only check for ASCII char 0x20 which is the plain SPACE char.
            if (t != ' ')
                return false;
        }
        return true;
    }
}
