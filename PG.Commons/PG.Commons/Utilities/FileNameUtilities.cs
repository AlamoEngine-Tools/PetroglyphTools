// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace PG.Commons.Utilities;

/// <summary>
/// Provides helper methods to handle and validate file names for a Petroglyph Star Wars game.
/// </summary>
public static class FileNameUtilities
{
    /// <summary>
    /// Indicates the status of a file name validation according to the rules used by <see cref="FileNameUtilities.IsValidFileName"/>.
    /// </summary>
    public enum FileNameValidationResult
    {
        /// <summary>
        /// The file name is valid.
        /// </summary>
        Success,
        /// <summary>
        /// The file name is either <see langword="null"/> or empty.
        /// </summary>
        NullOrEmpty,
        /// <summary>
        /// The file name contains an illegal character.
        /// </summary>
        InvalidCharacter,
        /// <summary>
        /// The file name starts or ends with a white space (\u0020) character.
        /// </summary>
        LeadingOrTrailingWhiteSpace,
        /// <summary>
        /// The file name ends with a period ('.') character.
        /// </summary>
        TrailingPeriod,
        /// <summary>
        /// The file name is reserved by windows (such as 'CON') and thus cannot be used.
        /// </summary>
        WindowsReserved
    }

    private static readonly Regex RegexInvalidName =
        new("^(COM\\d|CLOCK\\$|LPT\\d|AUX|NUL|CON|PRN|)$", RegexOptions.IgnoreCase);

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
    ///     d) The filename contains a character that is illegal for Windows file names (<see href="https://learn.microsoft.com/en-us/windows/win32/fileio/naming-a-file">see here</see>),
    ///        such as TAB, or path separators, etc.
    /// </remarks>
    /// <param name="filename">The filename to check.</param>
    /// <param name="result">Detailed information status. Can be used for error message reporting.</param>
    /// <returns><see langword="true"/> when the filename is valid; <see langword="false"/> otherwise.</returns>
    public static bool IsValidFileName([NotNullWhen(true)] string? filename, out FileNameValidationResult result)
    {
        result = FileNameValidationResult.Success;

        if (filename is null)
        {
            result = FileNameValidationResult.NullOrEmpty;
            return false;
        }

        var filenameSpan = filename.AsSpan();

        if (filenameSpan.IsEmpty)
        {
            result = FileNameValidationResult.NullOrEmpty;
            return false;
        }

        // This also already handles whitespace-only names
        if (!EdgesValid(filenameSpan, out var whiteSpaceError))
        {
            result = whiteSpaceError ? FileNameValidationResult.LeadingOrTrailingWhiteSpace : FileNameValidationResult.TrailingPeriod;
            return false;
        }
        
        if (ContainsInvalidChars(filenameSpan))
        {
            result = FileNameValidationResult.InvalidCharacter;
            return false;
        }

        if (RegexInvalidName.IsMatch(filename))
        {
            result = FileNameValidationResult.WindowsReserved;
            return false;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ContainsInvalidChars(ReadOnlySpan<char> value)
    {
        foreach (var t in value)
            if (IsInvalidFileCharacter(t))
                return true;

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsInvalidFileCharacter(char c)
    {
        // Check if character is in bounds [32-126] in general
        if ((uint)(c - '\x0020') > '\x007F' - '\x0020')  // (c >= '\x0020' && c <= '\x007F')
            return true;

        // Additional check for invalid Windows file name characters
        foreach (var charToCheck in InvalidFileNameChars.AsSpan())
        {
            if (charToCheck == c)
                return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool EdgesValid(ReadOnlySpan<char> value, out bool whiteSpace)
    {
        whiteSpace = false;

        if (value[0] is '\x0020')
        {
            whiteSpace = true;
            return false;
        }


#if NETSTANDARD2_1_OR_GREATER
        var lastChar = value[^1];
#else
        var lastChar = value[value.Length - 1];
#endif
        if (lastChar is '\x0020')
        {
            whiteSpace = true;
            return false;
        }

        if (lastChar is '.')
            return false;

        return true;
    }
}
