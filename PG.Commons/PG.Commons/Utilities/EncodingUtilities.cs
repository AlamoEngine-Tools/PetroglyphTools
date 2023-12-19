﻿using System;
using System.Runtime.CompilerServices;
using System.Text;
#if !NETSTANDARD2_1_OR_GREATER
using System.Diagnostics;
#endif

namespace PG.Commons.Utilities;

/// <summary>
/// Provides PG specific extension methods to the <see cref="Encoding"/> type.
/// </summary>
public static class EncodingUtilities
{
    /// <summary>
    ///     Gets the number of bytes required for encoding a given number of characters.
    /// </summary>
    /// <remarks>
    ///     This method should <b>only</b> be used while reading a .DAT or .MEG file as it is probably not safe in other use cases
    ///     (given how complex the .NET source code is).
    /// <br/>
    /// <br/>
    ///     Note:
    /// <br/>
    ///     PG binary files are not consistent whether they store the number of characters (.DAT or .MEG)
    ///     or the required bytes (chunked files). Since most strings in PG games are ASCII-encoded
    ///     we do have a 1:1 relation between # char and # bytes, this does not matter most of the time.
    ///     But in DAT Values we use UTF16 (Unicode), which requires 2x more bytes than characters.
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetByteCountPG(this Encoding encoding, int numberOfChars)
    {
        if (encoding == null)
            throw new ArgumentNullException(nameof(encoding));
        if (numberOfChars < 0)
            throw new ArgumentOutOfRangeException(nameof(numberOfChars), "Number of chars is less than zero.");

        // Prevent checking for rogue subtypes
        var encodingType = encoding.GetType();

        if (encoding.IsSingleByte && IsOriginalAsciiEncoding(encodingType))
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

        throw new NotSupportedException($"Encoding {encoding} is currently not supported.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsOriginalAsciiEncoding(Type encodingType)
    {
        var asciiType = typeof(ASCIIEncoding);
        return encodingType == asciiType ||
               (asciiType.IsAssignableFrom(encodingType) && encodingType.Assembly == asciiType.Assembly);
    }
    
    /// <summary>
    /// Encodes a string value.
    /// </summary>
    /// <param name="value">The string to encode.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>The encoded string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="encoding"/> or <paramref name="value"/>is <see langword="null"/>.</exception>
    public static string EncodeString(this Encoding encoding, string value)
    {
        if (encoding is null)
            throw new ArgumentNullException(nameof(encoding));
        if (value == null)
            throw new ArgumentNullException(nameof(value));
        return EncodeString(value.AsSpan(), encoding, encoding.GetMaxByteCount(value.Length));
    }

    /// <summary>
    /// Encodes a string value.
    /// </summary>
    /// <param name="value">The string to encode.</param>
    /// <param name="maxByteCount">Maximum bytes required for encoding.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>The encoded string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="encoding"/> or <paramref name="value"/>is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="maxByteCount"/> is less than actually required.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxByteCount"/> is negative.</exception>
    public static string EncodeString(this Encoding encoding, string value, int maxByteCount)
    {
        if (encoding == null) 
            throw new ArgumentNullException(nameof(encoding));
        if (value == null)
            throw new ArgumentNullException(nameof(value));
        return EncodeString(value.AsSpan(), encoding, maxByteCount);
    }

    // We cannot use a public method taking a ReadOnlySpan<char> cause conversion from (string)null would give us an empty span.
    // Once that conversion is performed we cannot tell whether the string actually was null or empty. 
    // Since trying to encode a null-reference does not make any sense, exposing this method to public
    // could break expectations on how this method should behave.
    private static unsafe string EncodeString(ReadOnlySpan<char> value, Encoding encoding, int maxByteCount)
    {
        if (maxByteCount < 0)
            throw new ArgumentOutOfRangeException(nameof(maxByteCount), "value must not be negative.");

        if (value.IsEmpty)
            return string.Empty;

        var buffer = maxByteCount <= 256 ? stackalloc byte[maxByteCount] : new byte[maxByteCount];

#if NETSTANDARD2_1_OR_GREATER || NET
        var bytesWritten = encoding.GetBytes(value, buffer);
        return encoding.GetString(buffer.Slice(0, bytesWritten));
#else
        fixed (char* pFileName = value)
        fixed (byte* pBuffer = buffer)
        {
            var bytesWritten = encoding.GetBytes(pFileName, value.Length, pBuffer, maxByteCount);
            Debug.Assert(bytesWritten <= maxByteCount);
            return encoding.GetString(pBuffer, bytesWritten);
        }
#endif
    }
}