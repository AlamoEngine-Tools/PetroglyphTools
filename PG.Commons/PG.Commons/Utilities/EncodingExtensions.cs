﻿// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace PG.Commons.Utilities;

/// <summary>
/// Provides PG specific extension methods to the <see cref="Encoding"/> type.
/// </summary>
public static class EncodingExtensions
{
    private static readonly Type Latin1EncodingType = Encoding.GetEncoding(28591).GetType();
    private static readonly Type AsciiType = typeof(ASCIIEncoding);

    /// <summary>
    ///     Gets the number of bytes required for encoding a given number of characters.
    /// </summary>
    /// <remarks>
    ///     This method should <b>only</b> be used while reading a Petroglyph binary file as it is not safe in other use cases.
    /// <br/>
    /// <br/>
    ///     Note:
    /// <br/>
    ///     PG binary files are not consistent whether they store the number of characters (.DAT or .MEG)
    ///     or the required bytes (chunked files). Since most strings in PG games are ASCII-encoded,
    ///     which has a 1:1 relation between # chars and # bytes, this does not matter most of the time.
    ///     But DAT Values use UTF16 (Unicode), which requires 2x more bytes than characters.
    /// <br/>
    /// <br/>
    ///     The assumption made here for this method to work is that, who ever created the binary file, did proper encoding -
    ///     e.g. replacing non ASCII with '?' or other fallback characters. 
    /// </remarks>
    /// <param name="encoding">The encoding to be used.</param>
    /// <param name="numberOfChars">The number of characters to encode.</param>
    /// <returns>The number of bytes</returns>
    /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <see langowrd="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfChars"/> is less than zero.</exception>
    /// <exception cref="NotSupportedException"><paramref name="encoding"/> is not supported.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetByteCountPG(this Encoding encoding, int numberOfChars)
    {
        if (encoding == null)
            throw new ArgumentNullException(nameof(encoding));
        if (numberOfChars < 0)
            throw new ArgumentOutOfRangeException(nameof(numberOfChars), "Number of chars is less than zero.");

        // Prevent checking for rogue subtypes
        var encodingType = encoding.GetType();

        if (encoding.IsSingleByte && IsOriginalAsciiEncodingOrLatin1Encoding(encodingType))
        {
            // 1 : 1 relation for ASCII
            return numberOfChars;
        }

        // PG .DAT Values are encoded in UTF-16LE, thus UTF-16BE is not supported
        if (encodingType == typeof(UnicodeEncoding) && !encoding.Equals(Encoding.BigEndianUnicode))
        {
            // UTF-16 has double the bytes than characters. 
            // Shifting is slightly faster than multiplication
            return numberOfChars << 1;
        }

        throw new NotSupportedException($"Encoding {encoding} is not supported.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsOriginalAsciiEncodingOrLatin1Encoding(Type encodingType)
    {
        if (encodingType == Latin1EncodingType)
            return true;
        return encodingType == AsciiType ||
               (AsciiType.IsAssignableFrom(encodingType) && encodingType.Assembly == AsciiType.Assembly);
    }
}