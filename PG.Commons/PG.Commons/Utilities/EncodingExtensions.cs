﻿using AnakinRaW.CommonUtilities.Extensions;
using System;
using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace PG.Commons.Utilities;

/// <summary>
/// Provides PG specific extension methods to the <see cref="Encoding"/> type.
/// </summary>
public static class EncodingExtensions
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
    /// 
    /// </summary>
    /// <param name="e"></param>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <param name="bytesWritten"></param>
    public static unsafe void Encode(this Encoding e, ReadOnlySpan<char> source, Span<char> dest, out int bytesWritten)
    {
        var numBytes = e.GetMaxByteCount(source.Length);
        byte[]? pooledByteArray = null;
        try
        {
            var buffer = numBytes > 260
                ? pooledByteArray = ArrayPool<byte>.Shared.Rent(numBytes)
                : stackalloc byte[numBytes];

            var bytes = e.GetBytesReadOnly(source, buffer);

            fixed (byte* pBytes = bytes)
            fixed (char* pChar = dest)
                bytesWritten = e.GetChars(pBytes, bytes.Length, pChar, dest.Length);
        }
        finally
        {
            if (pooledByteArray is not null)
                ArrayPool<byte>.Shared.Return(pooledByteArray);
        }
    }
}